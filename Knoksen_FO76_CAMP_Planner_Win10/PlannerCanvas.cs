using System.Drawing.Drawing2D;
using System.Text.Json;

namespace FO76CampPlanner;

public sealed class PlannerCanvas : Control
{
    private const int CanvasPadding = 16;

    private PlannerProject _project = new();
    private readonly HashSet<Guid> _selectedIds = new();
    private bool _draggingSelection;
    private bool _dragChanged;
    private Point _dragStartGrid;
    private PlannerProject? _dragStartSnapshot;
    private bool _marqueeSelecting;
    private Point _marqueeStartGrid;
    private Point _marqueeCurrentGrid;
    private int _zoomPercent = 100;
    private readonly Stack<PlannerProject> _undoStack = new();
    private readonly Stack<PlannerProject> _redoStack = new();
    private BlueprintModule? _loadedBlueprint;
    private bool _hoverGridValid;
    private Point _hoverGrid;
    private Guid? _draggingMarkerId;
    private PlannerProject? _dragMarkerSnapshot;
    private bool _dragMarkerChanged;
    private Guid? _draggingTrapZoneId;
    private bool _resizingTrapZone;
    private PlannerProject? _dragTrapZoneSnapshot;
    private bool _dragTrapZoneChanged;
    private Point _dragTrapStartGrid;
    private Rectangle _dragTrapStartRect;
    private string? _placementPreviewMessage;

    public PlannerCanvas()
    {
        DoubleBuffered = true;
        BackColor = Color.FromArgb(28, 31, 37);
        Dock = DockStyle.Fill;
        TabStop = true;
    }

    public PlannerProject Project
    {
        get => _project;
        set
        {
            _project = value ?? new PlannerProject();
            _selectedIds.Clear();
            ClearHistory();
            Invalidate();
            RaiseSelectionChanged();
            RaiseProjectChanged();
        }
    }

    public ToolType CurrentTool { get; set; } = ToolType.Select;

    public int ZoomPercent
    {
        get => _zoomPercent;
        set
        {
            _zoomPercent = Math.Max(50, Math.Min(250, value));
            Invalidate();
        }
    }

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public PlacedItem? SelectedItem => SelectedItems.FirstOrDefault();
    public IReadOnlyList<PlacedItem> SelectedItems => _project.Items.Where(x => _selectedIds.Contains(x.Id)).ToList();
    public BlueprintModule? LoadedBlueprint => _loadedBlueprint;
    public Point? HoverGridPoint => _hoverGridValid ? _hoverGrid : null;

    public event EventHandler? ProjectChanged;
    public event EventHandler? SelectionChanged;
    public event EventHandler? HistoryChanged;
    public event EventHandler? BlueprintChanged;
    public event EventHandler<string>? StatusMessage;

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var cell = GetScaledCellSize();
        var totalWidth = _project.GridWidth * cell;
        var totalHeight = _project.GridHeight * cell;
        var origin = new Point(CanvasPadding, CanvasPadding);

        using var canvasBrush = new SolidBrush(Color.FromArgb(36, 40, 46));
        e.Graphics.FillRectangle(canvasBrush, origin.X, origin.Y, totalWidth, totalHeight);

        DrawGrid(e.Graphics, origin, cell);
        DrawCampRadiusOverlay(e.Graphics, origin, cell);
        DrawTurretCoverage(e.Graphics, origin, cell);
        DrawVisitorFlow(e.Graphics, origin, cell);
        DrawTrapZones(e.Graphics, origin, cell);
        DrawItems(e.Graphics, origin, cell);
        DrawSelection(e.Graphics, origin, cell);
        DrawPlacementPreview(e.Graphics, origin, cell);
        DrawBlueprintGhost(e.Graphics, origin, cell);
        DrawHeader(e.Graphics, origin, totalWidth);
        DrawFooterHints(e.Graphics, origin, totalWidth, totalHeight);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();

        if (!TryGetGridPoint(e.Location, out var gridPoint))
        {
            return;
        }

        var ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;

        if (CurrentTool == ToolType.Select)
        {
            if (e.Button == MouseButtons.Left && TryHitVisitorMarker(e.Location, out var markerHit))
            {
                _draggingMarkerId = markerHit.Id;
                _dragMarkerSnapshot = CloneProject(_project);
                _dragMarkerChanged = false;
                StatusMessage?.Invoke(this, $"Dragging marker '{markerHit.Label}'.");
                return;
            }

            if (e.Button == MouseButtons.Left && TryHitTrapZoneResizeHandle(e.Location, out var trapResizeHit))
            {
                _draggingTrapZoneId = trapResizeHit.Id;
                _resizingTrapZone = true;
                _dragTrapZoneSnapshot = CloneProject(_project);
                _dragTrapZoneChanged = false;
                _dragTrapStartGrid = gridPoint;
                _dragTrapStartRect = new Rectangle(trapResizeHit.X, trapResizeHit.Y, trapResizeHit.Width, trapResizeHit.Height);
                StatusMessage?.Invoke(this, $"Resizing trap zone '{trapResizeHit.Label}'.");
                return;
            }

            if (e.Button == MouseButtons.Left && TryHitTrapZoneBody(e.Location, out var trapMoveHit))
            {
                _draggingTrapZoneId = trapMoveHit.Id;
                _resizingTrapZone = false;
                _dragTrapZoneSnapshot = CloneProject(_project);
                _dragTrapZoneChanged = false;
                _dragTrapStartGrid = gridPoint;
                _dragTrapStartRect = new Rectangle(trapMoveHit.X, trapMoveHit.Y, trapMoveHit.Width, trapMoveHit.Height);
                StatusMessage?.Invoke(this, $"Dragging trap zone '{trapMoveHit.Label}'.");
                return;
            }

            var hit = HitTest(gridPoint);
            var lockedHit = hit ?? HitTest(gridPoint, includeLocked: true);
            if (hit is null && lockedHit is not null && IsItemLocked(lockedHit))
            {
                var lockedName = Catalog.ById.TryGetValue(lockedHit.DefinitionId, out var lockedDefinition)
                    ? lockedDefinition.Name
                    : "Item";
                StatusMessage?.Invoke(this, $"{lockedName} is on a locked layer.");
                return;
            }

            if (hit is not null)
            {
                if (ctrlPressed)
                {
                    ToggleSelection(hit.Id);
                    StatusMessage?.Invoke(this, $"Selection count: {_selectedIds.Count}.");
                    return;
                }

                if (!_selectedIds.Contains(hit.Id))
                {
                    _selectedIds.Clear();
                    _selectedIds.Add(hit.Id);
                    RaiseSelectionChanged();
                }

                if (e.Button == MouseButtons.Left)
                {
                    _draggingSelection = true;
                    _dragChanged = false;
                    _dragStartGrid = gridPoint;
                    _dragStartSnapshot = CloneProject(_project);
                }

                Invalidate();
                return;
            }

            _marqueeSelecting = true;
            _marqueeStartGrid = gridPoint;
            _marqueeCurrentGrid = gridPoint;
            if (!ctrlPressed)
            {
                _selectedIds.Clear();
                RaiseSelectionChanged();
            }
            Invalidate();
            return;
        }

        if (CurrentTool == ToolType.Erase)
        {
            var hit = HitTest(gridPoint);
            var lockedHit = hit ?? HitTest(gridPoint, includeLocked: true);
            if (hit is null && lockedHit is not null && IsItemLocked(lockedHit))
            {
                var lockedName = Catalog.ById.TryGetValue(lockedHit.DefinitionId, out var lockedDefinition)
                    ? lockedDefinition.Name
                    : "Item";
                StatusMessage?.Invoke(this, $"{lockedName} is on a locked layer and cannot be erased.");
                return;
            }

            if (hit is not null)
            {
                PushUndoSnapshot();
                _project.Items.Remove(hit);
                _selectedIds.Remove(hit.Id);
                Invalidate();
                RaiseSelectionChanged();
                RaiseProjectChanged();
                StatusMessage?.Invoke(this, "Item removed.");
            }
            return;
        }

        var definition = Catalog.GetForTool(CurrentTool);
        if (definition is null)
        {
            return;
        }

        if (IsLayerLocked(definition.Layer))
        {
            StatusMessage?.Invoke(this, $"Layer {definition.Layer} is locked.");
            return;
        }

        var item = new PlacedItem
        {
            DefinitionId = definition.Id,
            X = gridPoint.X,
            Y = gridPoint.Y,
            Rotation = 0,
            Note = definition.Name
        };

        item = ApplySmartSnap(item, definition);
        var reason = ValidatePlacement(item, definition);
        if (reason is null)
        {
            PushUndoSnapshot();
            _project.Items.Add(item);
            _selectedIds.Clear();
            _selectedIds.Add(item.Id);
            RaiseSelectionChanged();
            RaiseProjectChanged();
            Invalidate();
            StatusMessage?.Invoke(this, $"Placed {definition.Name}.");
        }
        else
        {
            StatusMessage?.Invoke(this, reason);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (TryGetGridPoint(e.Location, out var hoverGrid))
        {
            _hoverGridValid = true;
            _hoverGrid = hoverGrid;
        }
        else
        {
            _hoverGridValid = false;
        }

        if (_marqueeSelecting)
        {
            if (_hoverGridValid)
            {
                _marqueeCurrentGrid = _hoverGrid;
                Invalidate();
            }
            return;
        }

        if (_draggingMarkerId is Guid markerId)
        {
            if (!_hoverGridValid)
            {
                return;
            }

            var marker = _project.VisitorMarkers.FirstOrDefault(x => x.Id == markerId);
            if (marker is null)
            {
                return;
            }

            var clampedX = Math.Clamp(_hoverGrid.X, 0, _project.GridWidth - 1);
            var clampedY = Math.Clamp(_hoverGrid.Y, 0, _project.GridHeight - 1);
            if (marker.X == clampedX && marker.Y == clampedY)
            {
                return;
            }

            marker.X = clampedX;
            marker.Y = clampedY;
            _dragMarkerChanged = true;
            Invalidate();
            return;
        }

        if (_draggingTrapZoneId is Guid trapZoneId && _dragTrapZoneSnapshot is not null)
        {
            if (!_hoverGridValid)
            {
                return;
            }

            var trapZone = _project.TrapZones.FirstOrDefault(x => x.Id == trapZoneId);
            if (trapZone is null)
            {
                return;
            }

            var deltaX = _hoverGrid.X - _dragTrapStartGrid.X;
            var deltaY = _hoverGrid.Y - _dragTrapStartGrid.Y;

            if (_resizingTrapZone)
            {
                var width = Math.Max(1, _dragTrapStartRect.Width + deltaX);
                var height = Math.Max(1, _dragTrapStartRect.Height + deltaY);
                width = Math.Min(width, _project.GridWidth - _dragTrapStartRect.X);
                height = Math.Min(height, _project.GridHeight - _dragTrapStartRect.Y);
                width = Math.Max(1, width);
                height = Math.Max(1, height);

                if (trapZone.Width == width && trapZone.Height == height)
                {
                    return;
                }

                trapZone.Width = width;
                trapZone.Height = height;
                _dragTrapZoneChanged = true;
                Invalidate();
                return;
            }

            var x = Math.Clamp(_dragTrapStartRect.X + deltaX, 0, Math.Max(0, _project.GridWidth - trapZone.Width));
            var y = Math.Clamp(_dragTrapStartRect.Y + deltaY, 0, Math.Max(0, _project.GridHeight - trapZone.Height));
            if (trapZone.X == x && trapZone.Y == y)
            {
                return;
            }

            trapZone.X = x;
            trapZone.Y = y;
            _dragTrapZoneChanged = true;
            Invalidate();
            return;
        }

        if (!_draggingSelection || _dragStartSnapshot is null)
        {
            return;
        }

        if (!TryGetGridPoint(e.Location, out var currentGrid))
        {
            return;
        }

        var delta = new Point(currentGrid.X - _dragStartGrid.X, currentGrid.Y - _dragStartGrid.Y);
        var snapshotSelectedById = _dragStartSnapshot.Items
            .Where(x => _selectedIds.Contains(x.Id))
            .ToDictionary(x => x.Id);

        if (snapshotSelectedById.Count == 0)
        {
            return;
        }

        foreach (var item in _project.Items.Where(x => _selectedIds.Contains(x.Id)))
        {
            if (!snapshotSelectedById.TryGetValue(item.Id, out var original))
            {
                continue;
            }

            item.X = original.X + delta.X;
            item.Y = original.Y + delta.Y;
            item.Rotation = original.Rotation;
        }

        if (_selectedIds.Count == 1)
        {
            var selected = SelectedItem;
            if (selected is not null && Catalog.ById.TryGetValue(selected.DefinitionId, out var definition))
            {
                var snapped = ApplySmartSnap(selected, definition);
                selected.X = snapped.X;
                selected.Y = snapped.Y;
                selected.Rotation = snapped.Rotation;
            }
        }

        var reason = ValidateGroupPlacement(SelectedItems, _selectedIds);
        if (reason is not null)
        {
            RestoreSelectionFromSnapshot(_dragStartSnapshot);
        }
        else
        {
            _dragChanged = delta.X != 0 || delta.Y != 0;
        }

        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (_marqueeSelecting)
        {
            _marqueeSelecting = false;
            var rect = GetMarqueeGridRect();
            foreach (var item in _project.Items)
            {
                if (!Catalog.ById.TryGetValue(item.DefinitionId, out var def) || !_project.LayerVisibility.IsVisible(def.Layer) || IsLayerLocked(def.Layer))
                {
                    continue;
                }

                var itemRect = new Rectangle(item.X, item.Y, GetSize(def, item.Rotation).Width, GetSize(def, item.Rotation).Height);
                if (rect.IntersectsWith(itemRect))
                {
                    _selectedIds.Add(item.Id);
                }
            }

            RaiseSelectionChanged();
            Invalidate();
            StatusMessage?.Invoke(this, $"Selection count: {_selectedIds.Count}.");
            return;
        }

        if (_draggingMarkerId is not null)
        {
            if (_dragMarkerChanged && _dragMarkerSnapshot is not null)
            {
                PushUndoSnapshot(_dragMarkerSnapshot);
                RaiseProjectChanged();
                StatusMessage?.Invoke(this, "Visitor marker moved.");
            }

            _draggingMarkerId = null;
            _dragMarkerSnapshot = null;
            _dragMarkerChanged = false;
            return;
        }

        if (_draggingTrapZoneId is not null)
        {
            if (_dragTrapZoneChanged && _dragTrapZoneSnapshot is not null)
            {
                PushUndoSnapshot(_dragTrapZoneSnapshot);
                RaiseProjectChanged();
                StatusMessage?.Invoke(this, _resizingTrapZone ? "Trap zone resized." : "Trap zone moved.");
            }

            _draggingTrapZoneId = null;
            _dragTrapZoneSnapshot = null;
            _dragTrapZoneChanged = false;
            _resizingTrapZone = false;
            return;
        }

        if (_draggingSelection)
        {
            _draggingSelection = false;
            if (_dragChanged && _dragStartSnapshot is not null)
            {
                PushUndoSnapshot(_dragStartSnapshot);
                RaiseProjectChanged();
                StatusMessage?.Invoke(this, _selectedIds.Count > 1 ? "Selection moved." : "Item moved.");
            }

            _dragStartSnapshot = null;
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoverGridValid = false;

        if (_draggingMarkerId is not null)
        {
            _draggingMarkerId = null;
            _dragMarkerSnapshot = null;
            _dragMarkerChanged = false;
        }

        if (_draggingTrapZoneId is not null)
        {
            _draggingTrapZoneId = null;
            _dragTrapZoneSnapshot = null;
            _dragTrapZoneChanged = false;
            _resizingTrapZone = false;
        }

        Invalidate();
    }

    protected override bool IsInputKey(Keys keyData)
        => keyData is Keys.Delete or Keys.R or Keys.Add or Keys.Subtract or Keys.Left or Keys.Right or Keys.Up or Keys.Down
           || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Control && e.KeyCode == Keys.Z)
        {
            Undo();
            return;
        }

        if (e.Control && e.KeyCode == Keys.Y)
        {
            Redo();
            return;
        }

        if (e.KeyCode == Keys.Delete && _selectedIds.Count > 0)
        {
            DeleteSelection();
            return;
        }

        if (e.KeyCode == Keys.R && _selectedIds.Count > 0)
        {
            RotateSelected();
            return;
        }

        if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus)
        {
            ZoomPercent += 10;
            return;
        }

        if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
        {
            ZoomPercent -= 10;
            return;
        }

        if (_selectedIds.Count > 0 && e.KeyCode is Keys.Left or Keys.Right or Keys.Up or Keys.Down)
        {
            MoveSelectionByKeyboard(e.KeyCode);
        }
    }

    public void RotateSelected()
    {
        if (_selectedIds.Count == 0)
        {
            return;
        }

        var snapshot = CloneProject(_project);

        var selectedItems = _project.Items.Where(x => _selectedIds.Contains(x.Id)).ToList();
        if (selectedItems.Count == 1)
        {
            var item = selectedItems[0];
            item.Rotation = (item.Rotation + 90) % 360;
            if (Catalog.ById.TryGetValue(item.DefinitionId, out var definition))
            {
                var snapped = ApplySmartSnap(item, definition);
                item.X = snapped.X;
                item.Y = snapped.Y;
                item.Rotation = snapped.Rotation;
            }
        }
        else
        {
            var pivot = GetSelectionPivot(selectedItems);
            foreach (var item in selectedItems)
            {
                if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition))
                {
                    continue;
                }

                var currentSize = GetSize(definition, item.Rotation);
                var newRotation = (item.Rotation + 90) % 360;
                var rotatedSize = GetSize(definition, newRotation);

                var centerX = item.X + (currentSize.Width / 2d);
                var centerY = item.Y + (currentSize.Height / 2d);

                var rotatedCenterX = pivot.X + (centerY - pivot.Y);
                var rotatedCenterY = pivot.Y - (centerX - pivot.X);

                item.X = (int)Math.Round(rotatedCenterX - (rotatedSize.Width / 2d));
                item.Y = (int)Math.Round(rotatedCenterY - (rotatedSize.Height / 2d));
                item.Rotation = newRotation;
            }
        }

        var reason = ValidateGroupPlacement(SelectedItems, _selectedIds);
        if (reason is not null)
        {
            _project = snapshot;
            RaiseProjectChanged();
            RaiseSelectionChanged();
            Invalidate();
            StatusMessage?.Invoke(this, reason);
            return;
        }

        PushUndoSnapshot(snapshot);
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, selectedItems.Count > 1 ? "Selection rotated around group pivot." : "Selection rotated.");
    }

    private static PointF GetSelectionPivot(IReadOnlyList<PlacedItem> selectedItems)
    {
        var centers = selectedItems
            .Where(item => Catalog.ById.ContainsKey(item.DefinitionId))
            .Select(item =>
            {
                var definition = Catalog.ById[item.DefinitionId];
                var size = GetSize(definition, item.Rotation);
                return new PointF((float)(item.X + (size.Width / 2d)), (float)(item.Y + (size.Height / 2d)));
            })
            .ToList();

        if (centers.Count == 0)
        {
            return new PointF(0f, 0f);
        }

        var pivotX = centers.Average(point => point.X);
        var pivotY = centers.Average(point => point.Y);
        return new PointF((float)pivotX, (float)pivotY);
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            StatusMessage?.Invoke(this, "Nothing to undo.");
            return;
        }

        var currentSelection = _selectedIds.ToHashSet();
        _redoStack.Push(CloneProject(_project));
        _project = _undoStack.Pop();
        RestoreSelectionAfterProjectSwap(currentSelection);
        Invalidate();
        RaiseProjectChanged();
        RaiseSelectionChanged();
        RaiseHistoryChanged();
        StatusMessage?.Invoke(this, "Undo complete.");
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            StatusMessage?.Invoke(this, "Nothing to redo.");
            return;
        }

        var currentSelection = _selectedIds.ToHashSet();
        _undoStack.Push(CloneProject(_project));
        _project = _redoStack.Pop();
        RestoreSelectionAfterProjectSwap(currentSelection);
        Invalidate();
        RaiseProjectChanged();
        RaiseSelectionChanged();
        RaiseHistoryChanged();
        StatusMessage?.Invoke(this, "Redo complete.");
    }

    public void DuplicateSelection()
    {
        var selected = SelectedItems;
        if (selected.Count == 0)
        {
            StatusMessage?.Invoke(this, "Select items before duplicating.");
            return;
        }

        var clones = selected.Select(item => new PlacedItem
        {
            Id = Guid.NewGuid(),
            DefinitionId = item.DefinitionId,
            X = item.X + 1,
            Y = item.Y + 1,
            Rotation = item.Rotation,
            Note = item.Note
        }).ToList();

        var reason = ValidateGroupPlacement(clones, new HashSet<Guid>());
        if (reason is not null)
        {
            StatusMessage?.Invoke(this, reason);
            return;
        }

        PushUndoSnapshot();
        _project.Items.AddRange(clones);
        _selectedIds.Clear();
        foreach (var clone in clones)
        {
            _selectedIds.Add(clone.Id);
        }

        RaiseSelectionChanged();
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, clones.Count == 1 ? "Selection duplicated." : $"{clones.Count} items duplicated.");
    }

    public void QuickDuplicateZone(int deltaX, int deltaY)
    {
        var selected = SelectedItems;
        if (selected.Count == 0)
        {
            StatusMessage?.Invoke(this, "Select items before quick-duplicating.");
            return;
        }

        // Calculate selection bounds to determine spacing
        var minX = selected.Min(x => x.X);
        var minY = selected.Min(x => x.Y);
        var maxX = selected.Max(x =>
        {
            var def = Catalog.ById[x.DefinitionId];
            var size = GetSize(def, x.Rotation);
            return x.X + size.Width;
        });
        var maxY = selected.Max(y =>
        {
            var def = Catalog.ById[y.DefinitionId];
            var size = GetSize(def, y.Rotation);
            return y.Y + size.Height;
        });

        var width = maxX - minX;
        var height = maxY - minY;

        // Determine offset based on direction and selection size
        var offsetX = deltaX == 0 ? 0 : (deltaX > 0 ? width : -width);
        var offsetY = deltaY == 0 ? 0 : (deltaY > 0 ? height : -height);

        var clones = selected.Select(item => new PlacedItem
        {
            Id = Guid.NewGuid(),
            DefinitionId = item.DefinitionId,
            X = item.X + offsetX,
            Y = item.Y + offsetY,
            Rotation = item.Rotation,
            Note = item.Note
        }).ToList();

        var reason = ValidateGroupPlacement(clones, new HashSet<Guid>());
        if (reason is not null)
        {
            StatusMessage?.Invoke(this, $"Cannot quick-duplicate: {reason}");
            return;
        }

        PushUndoSnapshot();
        _project.Items.AddRange(clones);
        _selectedIds.Clear();
        foreach (var clone in clones)
        {
            _selectedIds.Add(clone.Id);
        }

        RaiseSelectionChanged();
        RaiseProjectChanged();
        Invalidate();
        var direction = (deltaX, deltaY) switch
        {
            (1, 0) => "right",
            (-1, 0) => "left",
            (0, 1) => "down",
            (0, -1) => "up",
            _ => "zone"
        };
        StatusMessage?.Invoke(this, clones.Count == 1 ? $"Duplicated {direction}." : $"{clones.Count} items duplicated {direction}.");
    }

    public void QuickDuplicateTrapZone(Guid trapZoneId, int deltaX, int deltaY)
    {
        var trapZone = _project.TrapZones.FirstOrDefault(x => x.Id == trapZoneId);
        if (trapZone is null)
        {
            StatusMessage?.Invoke(this, "Select a valid trap zone first.");
            return;
        }

        var offsetX = deltaX == 0 ? 0 : (deltaX > 0 ? Math.Max(1, trapZone.Width) : -Math.Max(1, trapZone.Width));
        var offsetY = deltaY == 0 ? 0 : (deltaY > 0 ? Math.Max(1, trapZone.Height) : -Math.Max(1, trapZone.Height));

        var clone = new TrapZonePlan
        {
            Id = Guid.NewGuid(),
            Label = trapZone.Label,
            Severity = trapZone.Severity,
            Notes = trapZone.Notes,
            X = trapZone.X + offsetX,
            Y = trapZone.Y + offsetY,
            Width = Math.Max(1, trapZone.Width),
            Height = Math.Max(1, trapZone.Height)
        };

        if (clone.X < 0 || clone.Y < 0 || clone.X + clone.Width > _project.GridWidth || clone.Y + clone.Height > _project.GridHeight)
        {
            StatusMessage?.Invoke(this, "Cannot quick-duplicate trap zone: outside build grid.");
            return;
        }

        var trapZoneRuleFailure = ValidateShelterTrapZoneMutation(clone.Severity, true);
        if (trapZoneRuleFailure is not null)
        {
            StatusMessage?.Invoke(this, trapZoneRuleFailure);
            return;
        }

        PushUndoSnapshot();
        _project.TrapZones.Add(clone);
        RaiseProjectChanged();
        Invalidate();

        var direction = (deltaX, deltaY) switch
        {
            (1, 0) => "right",
            (-1, 0) => "left",
            (0, 1) => "down",
            (0, -1) => "up",
            _ => "zone"
        };

        StatusMessage?.Invoke(this, $"Trap zone duplicated {direction}.");
    }

    public void NudgeSelection(int deltaX, int deltaY)
    {
        var selected = SelectedItems;
        if (selected.Count == 0)
        {
            StatusMessage?.Invoke(this, "Nothing selected.");
            return;
        }

        var moved = selected.Select(item => new PlacedItem
        {
            Id = item.Id,
            DefinitionId = item.DefinitionId,
            X = item.X + deltaX,
            Y = item.Y + deltaY,
            Rotation = item.Rotation,
            Note = item.Note
        }).ToList();

        var ignoreIds = selected.Select(x => x.Id).ToHashSet();
        var reason = ValidateGroupPlacement(moved, ignoreIds);
        if (reason is not null)
        {
            StatusMessage?.Invoke(this, reason);
            return;
        }

        PushUndoSnapshot();
        var movedById = moved.ToDictionary(item => item.Id, item => item);
        foreach (var item in _project.Items)
        {
            if (movedById.TryGetValue(item.Id, out var updated))
            {
                item.X = updated.X;
                item.Y = updated.Y;
            }
        }

        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, selected.Count == 1 ? "Item nudged." : "Selection nudged.");
    }

    public void DeleteSelection()
    {
        if (_selectedIds.Count == 0)
        {
            StatusMessage?.Invoke(this, "Nothing selected.");
            return;
        }

        PushUndoSnapshot();
        var count = _selectedIds.Count;
        _project.Items.RemoveAll(x => _selectedIds.Contains(x.Id));
        _selectedIds.Clear();
        RaiseSelectionChanged();
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, count == 1 ? "Selection deleted." : $"{count} items deleted.");
    }

    public void SetCampCenterFromSelection()
    {
        var selected = SelectedItems;
        if (selected.Count == 0)
        {
            StatusMessage?.Invoke(this, "Select one or more items first.");
            return;
        }

        var minX = selected.Min(x => x.X);
        var minY = selected.Min(x => x.Y);
        var maxX = selected.Max(x =>
        {
            var def = Catalog.ById[x.DefinitionId];
            var size = GetSize(def, x.Rotation);
            return x.X + size.Width - 1;
        });
        var maxY = selected.Max(y =>
        {
            var def = Catalog.ById[y.DefinitionId];
            var size = GetSize(def, y.Rotation);
            return y.Y + size.Height - 1;
        });

        PushUndoSnapshot();
        _project.CampCenterX = (minX + maxX) / 2;
        _project.CampCenterY = (minY + maxY) / 2;
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"CAMP center set to ({_project.CampCenterX}, {_project.CampCenterY}).");
    }

    public void UpdateSingleSelectedItem(string? note, int x, int y, int rotation)
    {
        if (_selectedIds.Count != 1)
        {
            StatusMessage?.Invoke(this, "Inspector edits require exactly one selected item.");
            return;
        }

        var item = SelectedItem;
        if (item is null || !Catalog.ById.TryGetValue(item.DefinitionId, out var definition))
        {
            StatusMessage?.Invoke(this, "Unknown selection.");
            return;
        }

        rotation = ((rotation % 360) + 360) % 360;
        rotation = rotation switch
        {
            < 45 => 0,
            < 135 => 90,
            < 225 => 180,
            < 315 => 270,
            _ => 0
        };

        var snapshot = CloneProject(_project);
        item.X = Math.Max(0, Math.Min(_project.GridWidth - 1, x));
        item.Y = Math.Max(0, Math.Min(_project.GridHeight - 1, y));
        item.Rotation = rotation;
        item.Note = string.IsNullOrWhiteSpace(note) ? definition.Name : note.Trim();

        if (definition.Id is "wall" or "door" or "stairs" or "roof")
        {
            var snapped = ApplySmartSnap(item, definition);
            item.X = snapped.X;
            item.Y = snapped.Y;
            item.Rotation = snapped.Rotation;
        }

        var reason = ValidatePlacement(item, definition);
        if (reason is not null)
        {
            _project = snapshot;
            RaiseProjectChanged();
            RaiseSelectionChanged();
            Invalidate();
            StatusMessage?.Invoke(this, reason);
            return;
        }

        PushUndoSnapshot(snapshot);
        RaiseProjectChanged();
        RaiseSelectionChanged();
        Invalidate();
        StatusMessage?.Invoke(this, "Inspector changes applied.");
    }

    public void SelectItem(Guid itemId)
        => SelectItems(new[] { itemId });

    public void SelectItems(IEnumerable<Guid> itemIds)
    {
        _selectedIds.Clear();
        foreach (var id in itemIds)
        {
            var item = _project.Items.FirstOrDefault(x => x.Id == id);
            if (item is not null && !IsItemLocked(item))
            {
                _selectedIds.Add(id);
            }
        }

        RaiseSelectionChanged();
        Invalidate();
    }

    public void ClearSelection()
    {
        _selectedIds.Clear();
        RaiseSelectionChanged();
        Invalidate();
    }

    public void SetLayerVisibility(LayerType layer, bool visible)
    {
        _project.LayerVisibility.SetVisible(layer, visible);
        PruneInvisibleSelection();
        PruneLockedSelection();
        Invalidate();
        RaiseProjectChanged();
    }

    public void SetLayerLocked(LayerType layer, bool locked)
    {
        _project.LayerLocks.SetLocked(layer, locked);
        PruneLockedSelection();
        Invalidate();
        RaiseProjectChanged();
        StatusMessage?.Invoke(this, locked ? $"Layer {layer} locked." : $"Layer {layer} unlocked.");
    }

    public void ShowAllLayers()
    {
        _project.LayerVisibility.ShowAll();
        Invalidate();
        RaiseProjectChanged();
    }

    public void UnlockAllLayers()
    {
        _project.LayerLocks.UnlockAll();
        Invalidate();
        RaiseProjectChanged();
        StatusMessage?.Invoke(this, "All layers unlocked.");
    }

    public void ExportSelectionAsBlueprint(string path, string blueprintName)
    {
        var selected = SelectedItems;
        if (selected.Count == 0)
        {
            StatusMessage?.Invoke(this, "Select items before exporting a blueprint.");
            return;
        }

        var minX = selected.Min(x => x.X);
        var minY = selected.Min(y => y.Y);

        var blueprint = new BlueprintModule
        {
            Name = string.IsNullOrWhiteSpace(blueprintName) ? "Selection Blueprint" : blueprintName.Trim(),
            Description = $"Exported from {_project.Name} on {DateTime.Now:yyyy-MM-dd HH:mm}",
            RecommendedMode = _project.Mode,
            Items = selected.Select(x => new BlueprintItem
            {
                DefinitionId = x.DefinitionId,
                X = x.X - minX,
                Y = x.Y - minY,
                Rotation = x.Rotation,
                Note = x.Note
            }).ToList()
        };

        var json = JsonSerializer.Serialize(blueprint, AppJson.Default);
        File.WriteAllText(path, json);
        StatusMessage?.Invoke(this, $"Blueprint saved: {blueprint.Name}.");
    }

    public void LoadBlueprint(string path)
    {
        var json = File.ReadAllText(path);
        _loadedBlueprint = JsonSerializer.Deserialize<BlueprintModule>(json, AppJson.Default) ?? throw new InvalidOperationException("Invalid blueprint file.");
        RaiseBlueprintChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Loaded blueprint: {_loadedBlueprint.Name}.");
    }

    public void LoadBlueprintModule(BlueprintModule module, string? sourceName = null)
    {
        if (module.Items.Count == 0)
        {
            StatusMessage?.Invoke(this, "Blueprint module is empty.");
            return;
        }

        _loadedBlueprint = CloneBlueprintModule(module);
        RaiseBlueprintChanged();
        Invalidate();
        var suffix = string.IsNullOrWhiteSpace(sourceName) ? string.Empty : $" (source: {sourceName})";
        StatusMessage?.Invoke(this, $"Loaded blueprint: {_loadedBlueprint.Name}{suffix}.");
    }

    public void ClearLoadedBlueprint()
    {
        _loadedBlueprint = null;
        RaiseBlueprintChanged();
        Invalidate();
        StatusMessage?.Invoke(this, "Loaded blueprint cleared.");
    }

    public void PasteLoadedBlueprint()
    {
        if (_loadedBlueprint is null || _loadedBlueprint.Items.Count == 0)
        {
            StatusMessage?.Invoke(this, "Load a blueprint first.");
            return;
        }

        var anchor = FindBlueprintAnchor();
        var pasted = new List<PlacedItem>();
        foreach (var item in _loadedBlueprint.Items)
        {
            pasted.Add(new PlacedItem
            {
                Id = Guid.NewGuid(),
                DefinitionId = item.DefinitionId,
                X = anchor.X + item.X,
                Y = anchor.Y + item.Y,
                Rotation = item.Rotation,
                Note = item.Note
            });
        }

        var reason = ValidateGroupPlacement(pasted, new HashSet<Guid>());
        if (reason is not null)
        {
            StatusMessage?.Invoke(this, reason);
            return;
        }

        PushUndoSnapshot();
        _project.Items.AddRange(pasted);
        _selectedIds.Clear();
        foreach (var item in pasted)
        {
            _selectedIds.Add(item.Id);
        }

        RaiseSelectionChanged();
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Blueprint pasted: {_loadedBlueprint.Name}.");
    }

    public void TagSelectionAsTrapZone(bool enabled, string? zoneLabel = null, TrapZoneSeverity? severity = null, string? zoneNotes = null)
    {
        var selected = SelectedItems;
        if (selected.Count == 0)
        {
            StatusMessage?.Invoke(this, "Select one or more items first.");
            return;
        }

        if (enabled)
        {
            var selectedSeverity = severity ?? TrapZoneSeverity.Medium;
            var createNewZone = SelectionWouldCreateNewTrapZone();
            var trapZoneRuleFailure = ValidateShelterTrapZoneMutation(selectedSeverity, createNewZone);
            if (trapZoneRuleFailure is not null)
            {
                StatusMessage?.Invoke(this, trapZoneRuleFailure);
                return;
            }
        }

        var snapshot = CloneProject(_project);
        var changed = 0;
        var trapZoneCountBefore = _project.TrapZones.Count;

        foreach (var item in selected)
        {
            var current = item.Note ?? string.Empty;
            var hasTag = current.Contains("[TRAP]", StringComparison.OrdinalIgnoreCase);
            var trapPrefixIndex = current.IndexOf("[TRAP:", StringComparison.OrdinalIgnoreCase);
            if (trapPrefixIndex >= 0)
            {
                hasTag = true;
            }

            if (enabled && !hasTag)
            {
                var label = string.IsNullOrWhiteSpace(current) ? "Zone" : current.Trim();
                item.Note = string.IsNullOrWhiteSpace(zoneLabel)
                    ? $"[TRAP] {label}"
                    : $"[TRAP:{zoneLabel}] {label}";
                changed++;
            }

            if (enabled && hasTag && !string.IsNullOrWhiteSpace(zoneLabel) && !current.Contains($"[TRAP:{zoneLabel}]", StringComparison.OrdinalIgnoreCase))
            {
                var stripped = current.Replace("[TRAP]", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
                item.Note = $"[TRAP:{zoneLabel}] {stripped}".Trim();
                changed++;
            }

            if (!enabled && hasTag)
            {
                var cleaned = RemoveTrapPrefix(current);
                item.Note = string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
                changed++;
            }
        }

        if (enabled)
        {
            UpsertTrapZoneFromSelection(zoneLabel, severity ?? TrapZoneSeverity.Medium, zoneNotes);
        }
        else
        {
            RemoveTrapZonesOverlappingSelection();
        }

        var structuredTrapZonesChanged = trapZoneCountBefore != _project.TrapZones.Count
            || (enabled && _project.TrapZones.Count > 0);

        if (changed == 0 && !structuredTrapZonesChanged)
        {
            StatusMessage?.Invoke(this, enabled ? "Selection is already tagged as trap zone." : "Selection had no trap tags.");
            return;
        }

        PushUndoSnapshot(snapshot);
        RaiseProjectChanged();
        RaiseSelectionChanged();
        Invalidate();
        StatusMessage?.Invoke(this, enabled
            ? $"Tagged {changed} item(s) as trap zone."
            : $"Cleared trap tag on {changed} item(s).");
    }

    public void AddVisitorMarker(VisitorMarkerType type)
    {
        var point = GetMarkerAnchorPoint();
        if (point is null)
        {
            StatusMessage?.Invoke(this, "Move the pointer over the grid or select an item first.");
            return;
        }

        var markerRuleFailure = ValidateShelterVisitorMarkerAddition();
        if (markerRuleFailure is not null)
        {
            StatusMessage?.Invoke(this, markerRuleFailure);
            return;
        }

        PushUndoSnapshot();
        var marker = new VisitorMarker
        {
            Type = type,
            Order = GetNextVisitorMarkerOrder(),
            X = point.Value.X,
            Y = point.Value.Y,
            Label = GetDefaultMarkerLabel(type)
        };

        _project.VisitorMarkers.Add(marker);
        NormalizeVisitorMarkerOrder();
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"{marker.Label} marker added at ({marker.X}, {marker.Y}).");
    }

    public void UpdateVisitorMarker(Guid markerId, VisitorMarkerType type, string label)
    {
        var marker = _project.VisitorMarkers.FirstOrDefault(x => x.Id == markerId);
        if (marker is null)
        {
            StatusMessage?.Invoke(this, "Select a valid visitor marker first.");
            return;
        }

        PushUndoSnapshot();
        marker.Type = type;
        marker.Label = string.IsNullOrWhiteSpace(label) ? GetDefaultMarkerLabel(type) : label.Trim();
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Updated route marker '{marker.Label}'.");
    }

    public void ReorderVisitorMarker(Guid markerId, int direction)
    {
        if (direction == 0)
        {
            return;
        }

        var ordered = _project.VisitorMarkers.OrderBy(x => x.Order).ThenBy(x => x.Id).ToList();
        var index = ordered.FindIndex(x => x.Id == markerId);
        if (index < 0)
        {
            StatusMessage?.Invoke(this, "Select a valid visitor marker first.");
            return;
        }

        var targetIndex = Math.Clamp(index + direction, 0, ordered.Count - 1);
        if (targetIndex == index)
        {
            StatusMessage?.Invoke(this, "Marker is already at that position.");
            return;
        }

        PushUndoSnapshot();
        (ordered[index], ordered[targetIndex]) = (ordered[targetIndex], ordered[index]);
        for (var i = 0; i < ordered.Count; i++)
        {
            ordered[i].Order = i + 1;
        }

        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Moved '{ordered[targetIndex].Label}' to step {ordered[targetIndex].Order}.");
    }

    public void RemoveVisitorMarker(Guid markerId)
    {
        var marker = _project.VisitorMarkers.FirstOrDefault(x => x.Id == markerId);
        if (marker is null)
        {
            StatusMessage?.Invoke(this, "Select a valid visitor marker first.");
            return;
        }

        PushUndoSnapshot();
        _project.VisitorMarkers.Remove(marker);
        NormalizeVisitorMarkerOrder();
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Removed {marker.Label} marker.");
    }

    public void RemoveNearestVisitorMarker()
    {
        if (_project.VisitorMarkers.Count == 0)
        {
            StatusMessage?.Invoke(this, "No visitor markers to remove.");
            return;
        }

        var anchor = GetMarkerAnchorPoint() ?? new Point(GetCampCenter().X, GetCampCenter().Y);
        var marker = _project.VisitorMarkers
            .OrderBy(x => DistanceSquared(x.X, x.Y, anchor.X, anchor.Y))
            .First();

        PushUndoSnapshot();
        _project.VisitorMarkers.Remove(marker);
        NormalizeVisitorMarkerOrder();
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Removed {marker.Label} marker.");
    }

    public void ClearVisitorMarkers()
    {
        if (_project.VisitorMarkers.Count == 0)
        {
            StatusMessage?.Invoke(this, "No visitor markers to clear.");
            return;
        }

        var count = _project.VisitorMarkers.Count;
        PushUndoSnapshot();
        _project.VisitorMarkers.Clear();
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Cleared {count} visitor marker(s).");
    }

    public void AddTrapZoneFromSelection(string? zoneLabel, TrapZoneSeverity severity, string? zoneNotes)
    {
        if (SelectedItems.Count == 0)
        {
            StatusMessage?.Invoke(this, "Select one or more items first.");
            return;
        }

        var createNewZone = SelectionWouldCreateNewTrapZone();
        var trapZoneRuleFailure = ValidateShelterTrapZoneMutation(severity, createNewZone);
        if (trapZoneRuleFailure is not null)
        {
            StatusMessage?.Invoke(this, trapZoneRuleFailure);
            return;
        }

        PushUndoSnapshot();
        UpsertTrapZoneFromSelection(zoneLabel, severity, zoneNotes);
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Trap zone saved as {severity}.");
    }

    public void UpdateTrapZone(Guid trapZoneId, string? zoneLabel, TrapZoneSeverity severity, string? zoneNotes)
    {
        var trapZone = _project.TrapZones.FirstOrDefault(x => x.Id == trapZoneId);
        if (trapZone is null)
        {
            StatusMessage?.Invoke(this, "Select a valid trap zone first.");
            return;
        }

        var trapZoneRuleFailure = ValidateShelterTrapZoneMutation(severity, false);
        if (trapZoneRuleFailure is not null)
        {
            StatusMessage?.Invoke(this, trapZoneRuleFailure);
            return;
        }

        PushUndoSnapshot();
        trapZone.Label = string.IsNullOrWhiteSpace(zoneLabel) ? "Zone" : zoneLabel.Trim();
        trapZone.Severity = severity;
        trapZone.Notes = string.IsNullOrWhiteSpace(zoneNotes) ? string.Empty : zoneNotes.Trim();
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Updated trap zone '{trapZone.Label}'.");
    }

    public void RemoveTrapZone(Guid trapZoneId)
    {
        var trapZone = _project.TrapZones.FirstOrDefault(x => x.Id == trapZoneId);
        if (trapZone is null)
        {
            StatusMessage?.Invoke(this, "Select a valid trap zone first.");
            return;
        }

        PushUndoSnapshot();
        _project.TrapZones.Remove(trapZone);
        RaiseProjectChanged();
        Invalidate();
        StatusMessage?.Invoke(this, $"Removed trap zone '{trapZone.Label}'.");
    }

    private Point FindBlueprintAnchor()
    {
        var selected = SelectedItems;
        if (selected.Count > 0)
        {
            var right = selected.Max(x => x.X) + 2;
            var top = selected.Min(x => x.Y);
            return new Point(Math.Min(Math.Max(0, right), Math.Max(0, _project.GridWidth - 1)), Math.Max(0, top));
        }

        var maxX = _loadedBlueprint!.Items.Max(x => x.X);
        var maxY = _loadedBlueprint!.Items.Max(x => x.Y);
        return new Point(Math.Max(0, (_project.GridWidth - (maxX + 1)) / 2), Math.Max(0, (_project.GridHeight - (maxY + 1)) / 2));
    }

    private void ToggleSelection(Guid id)
    {
        if (!_selectedIds.Add(id))
        {
            _selectedIds.Remove(id);
        }

        RaiseSelectionChanged();
        Invalidate();
    }

    private void PruneInvisibleSelection()
    {
        var invisibleIds = _project.Items
            .Where(x => Catalog.ById.TryGetValue(x.DefinitionId, out var definition) && !_project.LayerVisibility.IsVisible(definition.Layer))
            .Select(x => x.Id)
            .ToHashSet();

        if (invisibleIds.Count == 0)
        {
            return;
        }

        _selectedIds.RemoveWhere(id => invisibleIds.Contains(id));
        RaiseSelectionChanged();
    }

    private void PruneLockedSelection()
    {
        var lockedIds = _project.Items
            .Where(IsItemLocked)
            .Select(x => x.Id)
            .ToHashSet();

        if (lockedIds.Count == 0)
        {
            return;
        }

        _selectedIds.RemoveWhere(id => lockedIds.Contains(id));
        RaiseSelectionChanged();
    }

    private Rectangle GetMarqueeGridRect()
    {
        var minX = Math.Min(_marqueeStartGrid.X, _marqueeCurrentGrid.X);
        var minY = Math.Min(_marqueeStartGrid.Y, _marqueeCurrentGrid.Y);
        var maxX = Math.Max(_marqueeStartGrid.X, _marqueeCurrentGrid.X);
        var maxY = Math.Max(_marqueeStartGrid.Y, _marqueeCurrentGrid.Y);
        return new Rectangle(minX, minY, (maxX - minX) + 1, (maxY - minY) + 1);
    }

    private void RestoreSelectionFromSnapshot(PlannerProject snapshot)
    {
        foreach (var item in _project.Items.Where(x => _selectedIds.Contains(x.Id)).ToList())
        {
            var original = snapshot.Items.FirstOrDefault(x => x.Id == item.Id);
            if (original is null)
            {
                continue;
            }

            item.X = original.X;
            item.Y = original.Y;
            item.Rotation = original.Rotation;
        }
    }

    private void MoveSelectionByKeyboard(Keys keyCode)
    {
        var delta = keyCode switch
        {
            Keys.Left => new Point(-1, 0),
            Keys.Right => new Point(1, 0),
            Keys.Up => new Point(0, -1),
            Keys.Down => new Point(0, 1),
            _ => Point.Empty
        };

        if (delta == Point.Empty)
        {
            return;
        }

        var snapshot = CloneProject(_project);
        foreach (var item in _project.Items.Where(x => _selectedIds.Contains(x.Id)))
        {
            item.X += delta.X;
            item.Y += delta.Y;
        }

        var reason = ValidateGroupPlacement(SelectedItems, _selectedIds);
        if (reason is not null)
        {
            _project = snapshot;
            RestoreSelectionAfterProjectSwap(_selectedIds);
            RaiseProjectChanged();
            RaiseSelectionChanged();
            Invalidate();
            StatusMessage?.Invoke(this, reason);
            return;
        }

        PushUndoSnapshot(snapshot);
        RaiseProjectChanged();
        Invalidate();
    }

    private void DrawGrid(Graphics g, Point origin, int cell)
    {
        using var pen = new Pen(Color.FromArgb(58, 63, 72), 1f);
        for (var x = 0; x <= _project.GridWidth; x++)
        {
            g.DrawLine(pen, origin.X + x * cell, origin.Y, origin.X + x * cell, origin.Y + _project.GridHeight * cell);
        }

        for (var y = 0; y <= _project.GridHeight; y++)
        {
            g.DrawLine(pen, origin.X, origin.Y + y * cell, origin.X + _project.GridWidth * cell, origin.Y + y * cell);
        }
    }

    private void DrawCampRadiusOverlay(Graphics g, Point origin, int cell)
    {
        if (!_project.ShowCampRadiusOverlay || _project.Mode != BuildMode.SurfaceCamp)
        {
            return;
        }

        var (centerX, centerY) = GetCampCenter();
        const float radiusCells = 11.5f;
        var diameter = radiusCells * 2f * cell;
        var topLeftX = origin.X + ((centerX + 0.5f) * cell) - (diameter / 2f);
        var topLeftY = origin.Y + ((centerY + 0.5f) * cell) - (diameter / 2f);
        var rect = new RectangleF(topLeftX, topLeftY, diameter, diameter);

        using var fill = new SolidBrush(Color.FromArgb(22, 90, 180, 255));
        using var pen = new Pen(Color.FromArgb(140, 90, 180, 255), 2f) { DashStyle = DashStyle.Dash };
        using var centerPen = new Pen(Color.FromArgb(190, 160, 220, 255), 1.5f);
        g.FillEllipse(fill, rect);
        g.DrawEllipse(pen, rect);

        var centerPx = origin.X + (centerX * cell) + (cell / 2f);
        var centerPy = origin.Y + (centerY * cell) + (cell / 2f);
        g.DrawLine(centerPen, centerPx - 8, centerPy, centerPx + 8, centerPy);
        g.DrawLine(centerPen, centerPx, centerPy - 8, centerPx, centerPy + 8);
    }

    private void DrawTurretCoverage(Graphics g, Point origin, int cell)
    {
        if (!_project.ShowTurretCoverage)
        {
            return;
        }

        const float rangeCells = 6.5f;
        const float sweepAngle = 120f;

        foreach (var item in _project.Items)
        {
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition) || definition.Id != "turret")
            {
                continue;
            }

            if (!_project.LayerVisibility.IsVisible(definition.Layer))
            {
                continue;
            }

            var centerPx = origin.X + (item.X * cell) + (cell / 2f);
            var centerPy = origin.Y + (item.Y * cell) + (cell / 2f);
            var diameter = rangeCells * 2f * cell;
            var rect = new RectangleF(centerPx - (diameter / 2f), centerPy - (diameter / 2f), diameter, diameter);
            var centerAngle = item.Rotation switch
            {
                0 => -90f,
                90 => 0f,
                180 => 90f,
                270 => 180f,
                _ => -90f
            };
            var startAngle = centerAngle - (sweepAngle / 2f);

            using var fill = new SolidBrush(Color.FromArgb(28, 255, 90, 90));
            using var pen = new Pen(Color.FromArgb(150, 255, 90, 90), 1.5f) { DashStyle = DashStyle.Dot };
            g.FillPie(fill, rect, startAngle, sweepAngle);
            g.DrawArc(pen, rect, startAngle, sweepAngle);
        }
    }

    private void DrawVisitorFlow(Graphics g, Point origin, int cell)
    {
        if (!_project.ShowVisitorFlowOverlay)
        {
            return;
        }

        var (centerX, centerY) = GetCampCenter();
        var centerPx = new PointF(origin.X + ((centerX + 0.5f) * cell), origin.Y + ((centerY + 0.5f) * cell));

        var targets = _project.Items
            .Where(item => Catalog.ById.TryGetValue(item.DefinitionId, out var definition)
                           && _project.LayerVisibility.IsVisible(definition.Layer)
                           && definition.Id is "door" or "vendor" or "workbench" or "ally")
            .Take(24)
            .ToList();

        using var linePen = new Pen(Color.FromArgb(140, 98, 204, 214), 1.8f) { DashStyle = DashStyle.Dash };
        using var targetBrush = new SolidBrush(Color.FromArgb(195, 98, 204, 214));
        using var centerBrush = new SolidBrush(Color.FromArgb(220, 130, 228, 238));

        g.FillEllipse(centerBrush, centerPx.X - 3f, centerPx.Y - 3f, 6f, 6f);

        foreach (var target in targets)
        {
            if (!Catalog.ById.TryGetValue(target.DefinitionId, out var definition))
            {
                continue;
            }

            var size = GetSize(definition, target.Rotation);
            var targetPx = new PointF(
                origin.X + ((target.X + (size.Width / 2f)) * cell),
                origin.Y + ((target.Y + (size.Height / 2f)) * cell));

            g.DrawLine(linePen, centerPx, targetPx);
            g.FillEllipse(targetBrush, targetPx.X - 2.5f, targetPx.Y - 2.5f, 5f, 5f);
        }

        DrawVisitorMarkers(g, origin, cell, centerPx);
    }

    private void DrawTrapZones(Graphics g, Point origin, int cell)
    {
        if (!_project.ShowTrapZonesOverlay)
        {
            return;
        }

        DrawStructuredTrapZones(g, origin, cell);

        using var fill = new SolidBrush(Color.FromArgb(18, 255, 84, 84));
        using var pen = new Pen(Color.FromArgb(120, 255, 120, 100), 1.3f) { DashStyle = DashStyle.Dot };

        foreach (var item in _project.Items)
        {
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition) || !_project.LayerVisibility.IsVisible(definition.Layer))
            {
                continue;
            }

            if (!IsTrapZoneItem(item, definition))
            {
                continue;
            }

            var size = GetSize(definition, item.Rotation);
            var trapRect = new Rectangle(
                origin.X + ((item.X - 1) * cell),
                origin.Y + ((item.Y - 1) * cell),
                (size.Width + 2) * cell,
                (size.Height + 2) * cell);

            g.FillRectangle(fill, trapRect);
            g.DrawRectangle(pen, trapRect);
        }
    }

    private static bool IsTrapZoneItem(PlacedItem item, ItemDefinition definition)
    {
        if (definition.Layer == LayerType.Defense)
        {
            return true;
        }

        var note = item.Note ?? string.Empty;
        return note.Contains("[TRAP]", StringComparison.OrdinalIgnoreCase)
               || note.Contains("trap zone", StringComparison.OrdinalIgnoreCase)
               || note.Contains("trap", StringComparison.OrdinalIgnoreCase);
    }

    private void DrawVisitorMarkers(Graphics g, Point origin, int cell, PointF centerPx)
    {
        if (_project.VisitorMarkers.Count == 0)
        {
            return;
        }

        using var borderPen = new Pen(Color.FromArgb(230, 242, 242, 242), 1.3f);
        using var labelBrush = new SolidBrush(Color.FromArgb(228, 238, 242));
        using var labelFont = new Font("Segoe UI", Math.Max(7.5f, cell / 5.5f), FontStyle.Bold);

        var orderedMarkers = _project.VisitorMarkers
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Id)
            .ToList();

        PointF? previousPoint = centerPx;
        VisitorMarker? previousMarker = null;
        foreach (var marker in orderedMarkers)
        {
            var px = origin.X + ((marker.X + 0.5f) * cell);
            var py = origin.Y + ((marker.Y + 0.5f) * cell);
            var rect = new RectangleF(px - 6f, py - 6f, 12f, 12f);
            var ingressCovered = marker.Type != VisitorMarkerType.Ingress || IsMarkerCoveredByAnyTurret(marker);
            using var brush = GetMarkerBrush(marker, ingressCovered);

            g.FillEllipse(brush, rect);
            g.DrawEllipse(borderPen, rect);
            g.DrawString(marker.Type switch { VisitorMarkerType.Ingress => "I", VisitorMarkerType.Egress => "E", _ => "C" }, labelFont, labelBrush, px - 4f, py - 7f);

            var point = GetMarkerPixel(origin, cell, marker);
            if (previousPoint is not null)
            {
                using var routePen = new Pen(GetRouteSegmentColor(previousMarker, marker), 1.8f) { DashStyle = DashStyle.DashDot };
                g.DrawLine(routePen, previousPoint.Value, point);
            }

            var orderText = marker.Order > 0 ? marker.Order.ToString() : "?";
            g.DrawString(orderText, labelFont, labelBrush, px + 7f, py - 10f);
            if (!string.IsNullOrWhiteSpace(marker.Label))
            {
                g.DrawString(marker.Label, labelFont, labelBrush, px + 7f, py + 4f);
            }

            previousPoint = point;
            previousMarker = marker;
        }
    }

    private SolidBrush GetMarkerBrush(VisitorMarker marker, bool ingressCovered)
    {
        if (marker.Type == VisitorMarkerType.Ingress)
        {
            return ingressCovered
                ? new SolidBrush(Color.FromArgb(228, 92, 214, 128))
                : new SolidBrush(Color.FromArgb(232, 245, 110, 98));
        }

        return marker.Type switch
        {
            VisitorMarkerType.Egress => new SolidBrush(Color.FromArgb(224, 92, 168, 255)),
            _ => new SolidBrush(Color.FromArgb(224, 255, 210, 92))
        };
    }

    private Color GetRouteSegmentColor(VisitorMarker? fromMarker, VisitorMarker toMarker)
    {
        var fromSeverity = fromMarker is null ? TrapZoneSeverity.Low : GetTrapSeverityAtPoint(fromMarker.X, fromMarker.Y);
        var toSeverity = GetTrapSeverityAtPoint(toMarker.X, toMarker.Y);
        var severity = (TrapZoneSeverity)Math.Max((int)fromSeverity, (int)toSeverity);

        return severity switch
        {
            TrapZoneSeverity.Critical => Color.FromArgb(212, 255, 92, 92),
            TrapZoneSeverity.High => Color.FromArgb(204, 255, 148, 102),
            TrapZoneSeverity.Medium => Color.FromArgb(194, 235, 198, 108),
            _ => Color.FromArgb(180, 160, 232, 196)
        };
    }

    private TrapZoneSeverity GetTrapSeverityAtPoint(int x, int y)
    {
        var zone = _project.TrapZones
            .Where(z => x >= z.X && y >= z.Y && x < z.X + Math.Max(1, z.Width) && y < z.Y + Math.Max(1, z.Height))
            .OrderByDescending(z => z.Severity)
            .FirstOrDefault();

        return zone?.Severity ?? TrapZoneSeverity.Low;
    }

    private bool IsMarkerCoveredByAnyTurret(VisitorMarker marker)
        => _project.Items.Any(item => IsTurret(item) && IsMarkerCoveredByTurret(marker, item));

    private static bool IsTurret(PlacedItem item)
        => Catalog.ById.TryGetValue(item.DefinitionId, out var definition) && definition.Id == "turret";

    private static bool IsMarkerCoveredByTurret(VisitorMarker marker, PlacedItem turret)
    {
        const double rangeCells = 6.5;
        const double halfSweep = 60d;

        var dx = (marker.X + 0.5) - (turret.X + 0.5);
        var dy = (marker.Y + 0.5) - (turret.Y + 0.5);
        var distance = Math.Sqrt((dx * dx) + (dy * dy));
        if (distance > rangeCells)
        {
            return false;
        }

        var markerAngle = Math.Atan2(dy, dx) * 180d / Math.PI;
        var turretAngle = turret.Rotation switch
        {
            0 => -90d,
            90 => 0d,
            180 => 90d,
            270 => 180d,
            _ => -90d
        };

        var delta = NormalizeAngle(markerAngle - turretAngle);
        return Math.Abs(delta) <= halfSweep;
    }

    private static double NormalizeAngle(double angle)
    {
        while (angle > 180d)
        {
            angle -= 360d;
        }

        while (angle < -180d)
        {
            angle += 360d;
        }

        return angle;
    }

    private Point? GetMarkerAnchorPoint()
    {
        if (_hoverGridValid)
        {
            return _hoverGrid;
        }

        var bounds = GetSelectionBounds();
        if (bounds is not null)
        {
            return new Point(bounds.Value.X + (bounds.Value.Width / 2), bounds.Value.Y + (bounds.Value.Height / 2));
        }

        var (centerX, centerY) = GetCampCenter();
        return new Point(centerX, centerY);
    }

    private static PointF GetMarkerPixel(Point origin, int cell, VisitorMarker marker)
        => new(origin.X + ((marker.X + 0.5f) * cell), origin.Y + ((marker.Y + 0.5f) * cell));

    private bool TryHitVisitorMarker(Point pixel, out VisitorMarker marker)
    {
        marker = null!;
        if (_project.VisitorMarkers.Count == 0)
        {
            return false;
        }

        var cell = GetScaledCellSize();
        var origin = new Point(CanvasPadding, CanvasPadding);
        var threshold = Math.Max(8f, cell * 0.22f);
        foreach (var candidate in _project.VisitorMarkers.OrderByDescending(x => x.Order))
        {
            var center = GetMarkerPixel(origin, cell, candidate);
            var dx = pixel.X - center.X;
            var dy = pixel.Y - center.Y;
            if ((dx * dx) + (dy * dy) <= threshold * threshold)
            {
                marker = candidate;
                return true;
            }
        }

        return false;
    }

    private bool TryHitTrapZoneBody(Point pixel, out TrapZonePlan trapZone)
    {
        trapZone = null!;
        if (_project.TrapZones.Count == 0)
        {
            return false;
        }

        var cell = GetScaledCellSize();
        var origin = new Point(CanvasPadding, CanvasPadding);
        foreach (var candidate in _project.TrapZones)
        {
            var rect = GetTrapZonePixelRect(origin, cell, candidate);
            if (rect.Contains(pixel))
            {
                trapZone = candidate;
                return true;
            }
        }

        return false;
    }

    private bool TryHitTrapZoneResizeHandle(Point pixel, out TrapZonePlan trapZone)
    {
        trapZone = null!;
        if (_project.TrapZones.Count == 0)
        {
            return false;
        }

        var cell = GetScaledCellSize();
        var origin = new Point(CanvasPadding, CanvasPadding);
        foreach (var candidate in _project.TrapZones)
        {
            var handleRect = GetTrapZoneResizeHandleRect(origin, cell, candidate);
            if (handleRect.Contains(pixel))
            {
                trapZone = candidate;
                return true;
            }
        }

        return false;
    }

    private void DrawStructuredTrapZones(Graphics g, Point origin, int cell)
    {
        if (_project.TrapZones.Count == 0)
        {
            return;
        }

        using var labelFont = new Font("Segoe UI", Math.Max(7.5f, cell / 5.4f), FontStyle.Bold);
        using var labelBrush = new SolidBrush(Color.FromArgb(232, 242, 242, 242));

        foreach (var trapZone in _project.TrapZones)
        {
            var rect = new Rectangle(
                origin.X + (trapZone.X * cell),
                origin.Y + (trapZone.Y * cell),
                Math.Max(1, trapZone.Width) * cell,
                Math.Max(1, trapZone.Height) * cell);

            var (fillColor, borderColor) = GetTrapZoneColors(trapZone.Severity);
            using var fill = new SolidBrush(fillColor);
            using var pen = new Pen(borderColor, 1.8f) { DashStyle = DashStyle.Dash };
            g.FillRectangle(fill, rect);
            g.DrawRectangle(pen, rect);
            var title = string.IsNullOrWhiteSpace(trapZone.Label) ? trapZone.Severity.ToString() : $"{trapZone.Label} ({trapZone.Severity})";
            g.DrawString(title, labelFont, labelBrush, rect.X + 6f, rect.Y + 4f);

            var handleRect = GetTrapZoneResizeHandleRect(origin, cell, trapZone);
            using var handleBrush = new SolidBrush(Color.FromArgb(220, borderColor));
            g.FillRectangle(handleBrush, handleRect);
            g.DrawRectangle(Pens.Black, handleRect);
        }
    }

    private void DrawItems(Graphics g, Point origin, int cell)
    {
        foreach (var item in _project.Items)
        {
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition))
            {
                continue;
            }

            if (!_project.LayerVisibility.IsVisible(definition.Layer))
            {
                continue;
            }

            var size = GetSize(definition, item.Rotation);
            var rect = new Rectangle(origin.X + item.X * cell, origin.Y + item.Y * cell, size.Width * cell, size.Height * cell);

            using var brush = new SolidBrush(Color.FromArgb(195, definition.DisplayColor));
            using var pen = new Pen(Color.FromArgb(235, definition.DisplayColor), 2f);
            g.FillRectangle(brush, rect);
            g.DrawRectangle(pen, rect);
            if (IsItemLocked(item))
            {
                using var lockedBrush = new SolidBrush(Color.FromArgb(38, 255, 214, 84));
                using var lockedPen = new Pen(Color.FromArgb(225, 255, 214, 84), 2f) { DashStyle = DashStyle.Dot };
                g.FillRectangle(lockedBrush, rect);
                g.DrawRectangle(lockedPen, rect);
            }
            DrawGlyphs(g, rect, definition, item.Rotation);
        }
    }

    private static void DrawGlyphs(Graphics g, Rectangle rect, ItemDefinition definition, int rotation)
    {
        using var font = new Font("Segoe UI", Math.Max(8f, rect.Height / 5f), FontStyle.Bold);
        using var brush = new SolidBrush(Color.FromArgb(28, 28, 28));
        var shortLabel = definition.Name switch
        {
            "Foundation" => "F",
            "Wall" => rotation is 90 or 270 ? "W│" : "W─",
            "Door" => rotation is 90 or 270 ? "D│" : "D─",
            "Stairs" => rotation is 90 or 270 ? "S⇄" : "S⇅",
            "Roof" => "R",
            "Workbench" => "WB",
            "Turret" => "T",
            "Power" => "P",
            "Light" => "L",
            "Decor" => "DEC",
            "Vendor" => "V",
            "Resource" => "RES",
            "Display" => "DIS",
            "Ally" => "A",
            _ => definition.Name[..Math.Min(2, definition.Name.Length)].ToUpperInvariant()
        };

        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString(shortLabel, font, brush, rect, format);
    }

    private void DrawSelection(Graphics g, Point origin, int cell)
    {
        foreach (var item in SelectedItems)
        {
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition) || !_project.LayerVisibility.IsVisible(definition.Layer))
            {
                continue;
            }

            var size = GetSize(definition, item.Rotation);
            var rect = new Rectangle(origin.X + item.X * cell, origin.Y + item.Y * cell, size.Width * cell, size.Height * cell);
            using var pen = new Pen(Color.FromArgb(255, 214, 84), 3) { DashStyle = DashStyle.Dash };
            g.DrawRectangle(pen, rect);
        }

        if (_selectedIds.Count > 1)
        {
            var bounds = GetSelectionBounds();
            if (bounds is not null)
            {
                var rect = new Rectangle(origin.X + bounds.Value.X * cell, origin.Y + bounds.Value.Y * cell, bounds.Value.Width * cell, bounds.Value.Height * cell);
                using var pen = new Pen(Color.FromArgb(255, 120, 60), 2) { DashStyle = DashStyle.Dot };
                g.DrawRectangle(pen, rect);
            }
        }

        if (_marqueeSelecting)
        {
            var marquee = GetMarqueeGridRect();
            var rect = new Rectangle(origin.X + marquee.X * cell, origin.Y + marquee.Y * cell, marquee.Width * cell, marquee.Height * cell);
            using var fill = new SolidBrush(Color.FromArgb(48, 255, 214, 84));
            using var pen = new Pen(Color.FromArgb(235, 255, 214, 84), 2.2f) { DashStyle = DashStyle.Dash };
            g.FillRectangle(fill, rect);
            g.DrawRectangle(pen, rect);
        }
    }

    private void DrawPlacementPreview(Graphics g, Point origin, int cell)
    {
        if (!_hoverGridValid || CurrentTool is ToolType.Select or ToolType.Erase)
        {
            _placementPreviewMessage = null;
            return;
        }

        var definition = Catalog.GetForTool(CurrentTool);
        if (definition is null)
        {
            _placementPreviewMessage = null;
            return;
        }

        var preview = new PlacedItem
        {
            DefinitionId = definition.Id,
            X = _hoverGrid.X,
            Y = _hoverGrid.Y,
            Rotation = 0,
            Note = definition.Name
        };

        preview = ApplySmartSnap(preview, definition);
        var reason = ValidatePlacement(preview, definition);
        var size = GetSize(definition, preview.Rotation);
        var rect = new Rectangle(origin.X + preview.X * cell, origin.Y + preview.Y * cell, size.Width * cell, size.Height * cell);
        var ok = reason is null;
        _placementPreviewMessage = ok ? "Placement valid." : reason;

        using var fill = new SolidBrush(Color.FromArgb(ok ? 74 : 96, ok ? 90 : 220, ok ? 210 : 90, ok ? 130 : 90));
        using var pen = new Pen(Color.FromArgb(ok ? 235 : 250, ok ? 90 : 220, ok ? 210 : 90, ok ? 130 : 90), 2.2f) { DashStyle = DashStyle.Dash };
        g.FillRectangle(fill, rect);
        g.DrawRectangle(pen, rect);

        using var labelBrush = new SolidBrush(Color.FromArgb(245, 245, 245));
        using var font = new Font("Segoe UI", Math.Max(8f, cell / 5.2f), FontStyle.Bold);
        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(ok ? "+" : "!", font, labelBrush, rect, sf);
    }

    private void DrawFooterHints(Graphics g, Point origin, int totalWidth, int totalHeight)
    {
        var y = origin.Y + totalHeight + 8;
        using var font = new Font("Segoe UI", 8.5f, FontStyle.Regular);
        using var softBrush = new SolidBrush(Color.FromArgb(170, 184, 196));
        using var accentBrush = new SolidBrush(Color.FromArgb(245, 188, 57));

        var left = _hoverGridValid
            ? $"Hover: ({_hoverGrid.X}, {_hoverGrid.Y})   •   Active tool: {CurrentTool}"
            : $"Active tool: {CurrentTool}";
        var right = _project.Items.Count == 0
            ? "First run: place foundations, then move through Layout → Envelope → Systems → Defense → Polish."
            : "Ctrl+Click multi-select   •   R rotate   •   Del remove   •   +/- zoom";
        if (!string.IsNullOrWhiteSpace(_placementPreviewMessage) && CurrentTool is not ToolType.Select and not ToolType.Erase)
        {
            right = _placementPreviewMessage;
        }
        g.DrawString(left, font, accentBrush, new RectangleF(origin.X, y, totalWidth * 0.5f, 18));

        var sf = new StringFormat { Alignment = StringAlignment.Far };
        g.DrawString(right, font, softBrush, new RectangleF(origin.X + totalWidth * 0.5f, y, totalWidth * 0.5f, 18), sf);
    }

    private void DrawBlueprintGhost(Graphics g, Point origin, int cell)
    {
        if (_loadedBlueprint is null || _loadedBlueprint.Items.Count == 0)
        {
            return;
        }

        var anchor = FindBlueprintAnchor();
        using var fill = new SolidBrush(Color.FromArgb(40, 120, 190, 255));
        using var pen = new Pen(Color.FromArgb(130, 120, 190, 255), 1.5f) { DashStyle = DashStyle.Dot };
        foreach (var item in _loadedBlueprint.Items)
        {
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition))
            {
                continue;
            }

            var size = GetSize(definition, item.Rotation);
            var rect = new Rectangle(origin.X + (anchor.X + item.X) * cell, origin.Y + (anchor.Y + item.Y) * cell, size.Width * cell, size.Height * cell);
            g.FillRectangle(fill, rect);
            g.DrawRectangle(pen, rect);
        }
    }

    private Rectangle? GetSelectionBounds()
    {
        var selected = SelectedItems;
        if (selected.Count == 0)
        {
            return null;
        }

        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var item in selected)
        {
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition))
            {
                continue;
            }

            var size = GetSize(definition, item.Rotation);
            minX = Math.Min(minX, item.X);
            minY = Math.Min(minY, item.Y);
            maxX = Math.Max(maxX, item.X + size.Width);
            maxY = Math.Max(maxY, item.Y + size.Height);
        }

        if (minX == int.MaxValue)
        {
            return null;
        }

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    private void DrawHeader(Graphics g, Point origin, int totalWidth)
    {
        var preset = PresetLibrary.GetById(_project.PresetId).Name;
        var loadedBlueprint = _loadedBlueprint is null ? "No blueprint loaded" : $"Blueprint: {_loadedBlueprint.Name}";
        var primary = $"{_project.Name}  •  {preset}  •  {_project.Mode}  •  {_project.RuleProfile}  •  Snap {_project.SnapMode}";
        var secondary = $"Grid {_project.GridWidth}x{_project.GridHeight}  •  Zoom {ZoomPercent}%  •  Tool {CurrentTool}  •  Sel {_selectedIds.Count}  •  {loadedBlueprint}";

        using var titleFont = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
        using var metaFont = new Font("Segoe UI", 8.4f, FontStyle.Regular);
        using var titleBrush = new SolidBrush(Color.FromArgb(236, 240, 244));
        using var metaBrush = new SolidBrush(Color.FromArgb(170, 184, 196));
        using var accentBrush = new SolidBrush(Color.FromArgb(245, 188, 57));

        g.DrawString(primary, titleFont, titleBrush, new RectangleF(origin.X, 0, totalWidth * 0.75f, 18));
        var sf = new StringFormat { Alignment = StringAlignment.Far };
        g.DrawString(CurrentTool.ToString(), metaFont, accentBrush, new RectangleF(origin.X + totalWidth * 0.76f, 0, totalWidth * 0.24f, 18), sf);
        g.DrawString(secondary, metaFont, metaBrush, new RectangleF(origin.X, 16, totalWidth, 16));
    }

    private string? ValidatePlacement(PlacedItem item, ItemDefinition definition, Guid? ignoreId = null)
        => ValidateGroupPlacement(new[] { item }, ignoreId is null ? new HashSet<Guid>() : new HashSet<Guid> { ignoreId.Value });

    private string? ValidateGroupPlacement(IEnumerable<PlacedItem> items, HashSet<Guid> ignoreIds)
    {
        var candidateItems = items.ToList();
        var candidateMap = candidateItems.ToDictionary(x => x.Id, x => x);

        foreach (var item in candidateItems)
        {
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition))
            {
                return "Unknown item definition encountered.";
            }

            var allowed = _project.Mode switch
            {
                BuildMode.SurfaceCamp => definition.AllowedInSurface,
                BuildMode.Shelter => definition.AllowedInShelter,
                _ => false
            };

            if (!allowed)
            {
                return $"{definition.Name} is not allowed in {_project.Mode}.";
            }

            var size = GetSize(definition, item.Rotation);
            if (item.X < 0 || item.Y < 0 || item.X + size.Width > _project.GridWidth || item.Y + size.Height > _project.GridHeight)
            {
                return "Placement is outside the build grid.";
            }

            if (!HasRequiredSupport(item, definition, ignoreIds, candidateMap))
            {
                return GetSupportMessage(definition);
            }
        }

        for (var i = 0; i < candidateItems.Count; i++)
        {
            var itemA = candidateItems[i];
            var defA = Catalog.ById[itemA.DefinitionId];

            for (var j = i + 1; j < candidateItems.Count; j++)
            {
                var itemB = candidateItems[j];
                var defB = Catalog.ById[itemB.DefinitionId];
                if (ItemsOverlap(itemA, defA, itemB, defB) && !AllowsOverlap(defA, defB))
                {
                    return $"{defA.Name} overlaps {defB.Name}.";
                }
            }

            foreach (var other in _project.Items)
            {
                if (ignoreIds.Contains(other.Id))
                {
                    continue;
                }

                if (!Catalog.ById.TryGetValue(other.DefinitionId, out var otherDefinition))
                {
                    continue;
                }

                if (!ItemsOverlap(itemA, defA, other, otherDefinition))
                {
                    continue;
                }

                if (AllowsOverlap(defA, otherDefinition))
                {
                    continue;
                }

                return $"{defA.Name} overlaps {otherDefinition.Name}.";
            }
        }

        var existingBudget = _project.Items
            .Where(x => !ignoreIds.Contains(x.Id))
            .Select(x => Catalog.ById.TryGetValue(x.DefinitionId, out var def) ? def.BudgetCost : 0)
            .Sum();

        var candidateBudget = candidateItems.Select(x => Catalog.ById[x.DefinitionId].BudgetCost).Sum();
        var projectedBudget = existingBudget + candidateBudget + _project.StoredBudget;
        if (projectedBudget > _project.BudgetLimit)
        {
            return "Budget exceeded. Increase limit or remove items.";
        }

        if (TryGetActiveShelterRuleSpec(out var shelterRule))
        {
            var existingTurretCount = _project.Items.Count(x =>
                !ignoreIds.Contains(x.Id)
                && Catalog.ById.TryGetValue(x.DefinitionId, out var def)
                && def.Id == "turret");

            var candidateTurretCount = candidateItems.Count(x => Catalog.ById[x.DefinitionId].Id == "turret");
            if (existingTurretCount + candidateTurretCount > shelterRule.MaxTurrets)
            {
                var presetName = PresetLibrary.GetById(_project.PresetId).Name;
                return $"{presetName} allows up to {shelterRule.MaxTurrets} turrets.";
            }
        }

        return null;
    }

    private bool HasRequiredSupport(PlacedItem item, ItemDefinition definition, HashSet<Guid> ignoreIds, IReadOnlyDictionary<Guid, PlacedItem> candidates)
    {
        if (_project.Mode == BuildMode.Shelter)
        {
            return true;
        }

        return definition.Id switch
        {
            "foundation" => true,
            "wall" or "door" => AdjacentToFoundation(item, definition, ignoreIds, candidates),
            "roof" => OverlapsFoundation(item, definition, ignoreIds, candidates),
            "stairs" => AdjacentToFoundation(item, definition, ignoreIds, candidates),
            _ => true
        };
    }

    private static string GetSupportMessage(ItemDefinition definition)
        => definition.Id switch
        {
            "wall" => "Wall must snap to a foundation edge in surface mode.",
            "door" => "Door must snap to a foundation edge in surface mode.",
            "roof" => "Roof must be placed over a foundation footprint in surface mode.",
            "stairs" => "Stairs must connect to a foundation edge in surface mode.",
            _ => "Placement support is invalid."
        };

    private bool AllowsOverlap(ItemDefinition a, ItemDefinition b)
    {
        if (a.Id == "foundation" && b.Id != "foundation")
        {
            return true;
        }

        if (b.Id == "foundation" && a.Id != "foundation")
        {
            return true;
        }

        return _project.RuleProfile switch
        {
            RuleProfile.Strict => false,
            RuleProfile.Relaxed => IsSoftLayer(a.Layer) || IsSoftLayer(b.Layer),
            RuleProfile.Shelter => !(a.Layer == LayerType.Structure && b.Layer == LayerType.Structure),
            _ => false
        };
    }

    private static bool IsSoftLayer(LayerType layer)
        => layer is LayerType.Aesthetic or LayerType.Power;

    private bool OverlapsFoundation(PlacedItem item, ItemDefinition definition, HashSet<Guid> ignoreIds, IReadOnlyDictionary<Guid, PlacedItem> candidates)
    {
        foreach (var other in EnumerateFoundations(ignoreIds, candidates))
        {
            if (ItemsOverlap(item, definition, other, Catalog.ById[other.DefinitionId]))
            {
                return true;
            }
        }

        return false;
    }

    private bool AdjacentToFoundation(PlacedItem item, ItemDefinition definition, HashSet<Guid> ignoreIds, IReadOnlyDictionary<Guid, PlacedItem> candidates)
    {
        var size = GetSize(definition, item.Rotation);
        var rect = new Rectangle(item.X, item.Y, size.Width, size.Height);
        foreach (var other in EnumerateFoundations(ignoreIds, candidates))
        {
            var foundationRect = new Rectangle(other.X, other.Y, 1, 1);
            var expanded = Rectangle.Inflate(foundationRect, 1, 1);
            if (expanded.IntersectsWith(rect) && !foundationRect.IntersectsWith(rect))
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerable<PlacedItem> EnumerateFoundations(HashSet<Guid> ignoreIds, IReadOnlyDictionary<Guid, PlacedItem> candidates)
    {
        foreach (var item in _project.Items)
        {
            if (ignoreIds.Contains(item.Id))
            {
                continue;
            }

            if (Catalog.ById.TryGetValue(item.DefinitionId, out var def) && def.Id == "foundation")
            {
                yield return item;
            }
        }

        foreach (var item in candidates.Values)
        {
            if (Catalog.ById.TryGetValue(item.DefinitionId, out var def) && def.Id == "foundation")
            {
                yield return item;
            }
        }
    }

    private static bool ItemsOverlap(PlacedItem a, ItemDefinition defA, PlacedItem b, ItemDefinition defB)
    {
        var sizeA = GetSize(defA, a.Rotation);
        var sizeB = GetSize(defB, b.Rotation);
        var rectA = new Rectangle(a.X, a.Y, sizeA.Width, sizeA.Height);
        var rectB = new Rectangle(b.X, b.Y, sizeB.Width, sizeB.Height);
        return rectA.IntersectsWith(rectB);
    }

    private static Size GetSize(ItemDefinition definition, int rotation)
    {
        var horizontal = rotation % 180 == 0;
        return horizontal ? new Size(definition.Width, definition.Height) : new Size(definition.Height, definition.Width);
    }

    private PlacedItem ApplySmartSnap(PlacedItem sourceItem, ItemDefinition definition)
    {
        var item = ClonePlacedItem(sourceItem);
        if (_project.SnapMode == SnapMode.Off)
        {
            return item;
        }

        var relaxed = _project.SnapMode == SnapMode.Relaxed;

        if (_project.Items.Count == 0)
        {
            return item;
        }

        if (definition.Id is "wall" or "door")
        {
            var candidate = FindNearestFoundationEdgeAnchor(item.X, item.Y);
            if (candidate is not null && (!relaxed || DistanceSquared(candidate.Value.X, candidate.Value.Y, sourceItem.X, sourceItem.Y) <= 9))
            {
                item.X = candidate.Value.X;
                item.Y = candidate.Value.Y;
                item.Rotation = candidate.Value.Rotation;
            }
        }
        else if (definition.Id == "roof")
        {
            var candidate = FindNearestFoundationCell(item.X, item.Y);
            if (candidate is not null && (!relaxed || DistanceSquared(candidate.Value.X, candidate.Value.Y, sourceItem.X, sourceItem.Y) <= 9))
            {
                item.X = candidate.Value.X;
                item.Y = candidate.Value.Y;
            }
        }
        else if (definition.Id == "stairs")
        {
            var candidate = FindNearestStairAnchor(item.X, item.Y, definition, item.Rotation);
            if (candidate is not null && (!relaxed || DistanceSquared(candidate.Value.X, candidate.Value.Y, sourceItem.X, sourceItem.Y) <= 9))
            {
                item.X = candidate.Value.X;
                item.Y = candidate.Value.Y;
                item.Rotation = candidate.Value.Rotation;
            }
        }

        return item;
    }

    private EdgeAnchor? FindNearestFoundationEdgeAnchor(int x, int y)
    {
        var candidates = new List<EdgeAnchor>();
        foreach (var foundation in _project.Items.Where(i => Catalog.ById.TryGetValue(i.DefinitionId, out var def) && def.Id == "foundation"))
        {
            if (!HasFoundationAt(foundation.X, foundation.Y - 1))
            {
                candidates.Add(new EdgeAnchor(foundation.X, foundation.Y - 1, 0));
            }
            if (!HasFoundationAt(foundation.X + 1, foundation.Y))
            {
                candidates.Add(new EdgeAnchor(foundation.X + 1, foundation.Y, 90));
            }
            if (!HasFoundationAt(foundation.X, foundation.Y + 1))
            {
                candidates.Add(new EdgeAnchor(foundation.X, foundation.Y + 1, 180));
            }
            if (!HasFoundationAt(foundation.X - 1, foundation.Y))
            {
                candidates.Add(new EdgeAnchor(foundation.X - 1, foundation.Y, 270));
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        var valid = candidates
            .Where(c => c.X >= 0 && c.Y >= 0 && c.X < _project.GridWidth && c.Y < _project.GridHeight)
            .OrderBy(c => DistanceSquared(c.X, c.Y, x, y))
            .ToList();

        return valid.Count == 0 ? null : valid[0];
    }

    private Point? FindNearestFoundationCell(int x, int y)
    {
        var candidates = _project.Items
            .Where(i => Catalog.ById.TryGetValue(i.DefinitionId, out var def) && def.Id == "foundation")
            .Select(i => new Point(i.X, i.Y))
            .ToList();

        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates.OrderBy(p => DistanceSquared(p.X, p.Y, x, y)).First();
    }

    private StairAnchor? FindNearestStairAnchor(int x, int y, ItemDefinition definition, int currentRotation)
    {
        var candidates = new List<StairAnchor>();
        foreach (var foundation in _project.Items.Where(i => Catalog.ById.TryGetValue(i.DefinitionId, out var def) && def.Id == "foundation"))
        {
            candidates.Add(new StairAnchor(foundation.X, foundation.Y - 2, 0));
            candidates.Add(new StairAnchor(foundation.X, foundation.Y + 1, 180));
            candidates.Add(new StairAnchor(foundation.X - 2, foundation.Y, 270));
            candidates.Add(new StairAnchor(foundation.X + 1, foundation.Y, 90));
        }

        StairAnchor? best = null;
        var bestScore = int.MaxValue;
        foreach (var candidate in candidates)
        {
            var size = GetSize(definition, candidate.Rotation);
            if (candidate.X < 0 || candidate.Y < 0 || candidate.X + size.Width > _project.GridWidth || candidate.Y + size.Height > _project.GridHeight)
            {
                continue;
            }

            var score = DistanceSquared(candidate.X, candidate.Y, x, y) + Math.Abs(candidate.Rotation - currentRotation);
            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private bool IsLayerLocked(LayerType layer)
        => _project.LayerLocks?.IsLocked(layer) ?? false;

    private bool IsItemLocked(PlacedItem item)
        => Catalog.ById.TryGetValue(item.DefinitionId, out var definition) && IsLayerLocked(definition.Layer);

    private (int X, int Y) GetCampCenter()
    {
        var centerX = _project.CampCenterX >= 0 ? _project.CampCenterX : _project.GridWidth / 2;
        var centerY = _project.CampCenterY >= 0 ? _project.CampCenterY : _project.GridHeight / 2;
        centerX = Math.Max(0, Math.Min(_project.GridWidth - 1, centerX));
        centerY = Math.Max(0, Math.Min(_project.GridHeight - 1, centerY));
        return (centerX, centerY);
    }

    private bool HasFoundationAt(int x, int y)
        => _project.Items.Any(item => Catalog.ById.TryGetValue(item.DefinitionId, out var definition) && definition.Id == "foundation" && item.X == x && item.Y == y);

    private PlacedItem? HitTest(Point gridPoint, bool includeLocked = false)
    {
        for (var i = _project.Items.Count - 1; i >= 0; i--)
        {
            var item = _project.Items[i];
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var def) || !_project.LayerVisibility.IsVisible(def.Layer))
            {
                continue;
            }

            if (!includeLocked && IsLayerLocked(def.Layer))
            {
                continue;
            }

            var size = GetSize(def, item.Rotation);
            var rect = new Rectangle(item.X, item.Y, size.Width, size.Height);
            if (rect.Contains(gridPoint))
            {
                return item;
            }
        }

        return null;
    }

    private bool TryGetGridPoint(Point point, out Point gridPoint)
    {
        var cell = GetScaledCellSize();
        var origin = new Point(CanvasPadding, CanvasPadding);
        var x = point.X - origin.X;
        var y = point.Y - origin.Y;

        if (x < 0 || y < 0)
        {
            gridPoint = Point.Empty;
            return false;
        }

        gridPoint = new Point(x / cell, y / cell);
        return gridPoint.X < _project.GridWidth && gridPoint.Y < _project.GridHeight;
    }

    private int GetScaledCellSize()
        => Math.Max(18, _project.CellSize * _zoomPercent / 100);

    public Bitmap RenderToBitmap()
    {
        var cell = GetScaledCellSize();
        var width = (_project.GridWidth * cell) + 32;
        var height = (_project.GridHeight * cell) + 48;
        var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(BackColor);
        var args = new PaintEventArgs(graphics, new Rectangle(Point.Empty, bitmap.Size));
        OnPaint(args);
        return bitmap;
    }

    private void PushUndoSnapshot()
        => PushUndoSnapshot(CloneProject(_project));

    private void PushUndoSnapshot(PlannerProject snapshot)
    {
        _undoStack.Push(snapshot);
        _redoStack.Clear();
        RaiseHistoryChanged();
    }

    private void RestoreSelectionAfterProjectSwap(IEnumerable<Guid> preferredSelection)
    {
        _selectedIds.Clear();
        var preferred = preferredSelection.ToHashSet();
        foreach (var item in _project.Items)
        {
            if (preferred.Contains(item.Id))
            {
                _selectedIds.Add(item.Id);
            }
        }
    }

    private void ClearHistory()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        RaiseHistoryChanged();
    }

    private static PlannerProject CloneProject(PlannerProject project)
    {
        var json = JsonSerializer.Serialize(project);
        return JsonSerializer.Deserialize<PlannerProject>(json) ?? new PlannerProject();
    }

    private static BlueprintModule CloneBlueprintModule(BlueprintModule module)
    {
        var json = JsonSerializer.Serialize(module, AppJson.Default);
        return JsonSerializer.Deserialize<BlueprintModule>(json, AppJson.Default) ?? new BlueprintModule();
    }

    private static PlacedItem ClonePlacedItem(PlacedItem item)
        => new()
        {
            Id = item.Id,
            DefinitionId = item.DefinitionId,
            X = item.X,
            Y = item.Y,
            Rotation = item.Rotation,
            Note = item.Note
        };

    private static int DistanceSquared(int x1, int y1, int x2, int y2)
    {
        var dx = x1 - x2;
        var dy = y1 - y2;
        return (dx * dx) + (dy * dy);
    }

    private bool TryGetActiveShelterRuleSpec(out ShelterRuleLibrary.ShelterRuleSpec spec)
    {
        if (_project.Mode == BuildMode.Shelter && ShelterRuleLibrary.TryGetForPreset(_project.PresetId, out spec))
        {
            return true;
        }

        spec = null!;
        return false;
    }

    private string? ValidateShelterVisitorMarkerAddition()
    {
        if (!TryGetActiveShelterRuleSpec(out var shelterRule))
        {
            return null;
        }

        if (_project.VisitorMarkers.Count >= shelterRule.MaxVisitorMarkers)
        {
            var presetName = PresetLibrary.GetById(_project.PresetId).Name;
            return $"{presetName} allows up to {shelterRule.MaxVisitorMarkers} route markers.";
        }

        return null;
    }

    private string? ValidateShelterTrapZoneMutation(TrapZoneSeverity severity, bool createNewZone)
    {
        if (!TryGetActiveShelterRuleSpec(out var shelterRule))
        {
            return null;
        }

        if (severity > shelterRule.MaxTrapSeverity)
        {
            var presetName = PresetLibrary.GetById(_project.PresetId).Name;
            return $"{presetName} allows trap severity up to {shelterRule.MaxTrapSeverity}.";
        }

        if (createNewZone && _project.TrapZones.Count >= shelterRule.MaxTrapZones)
        {
            var presetName = PresetLibrary.GetById(_project.PresetId).Name;
            return $"{presetName} allows up to {shelterRule.MaxTrapZones} trap zones.";
        }

        return null;
    }

    private bool SelectionWouldCreateNewTrapZone()
    {
        var bounds = GetSelectionBounds();
        if (bounds is null)
        {
            return false;
        }

        return !_project.TrapZones.Any(zone =>
            zone.X == bounds.Value.X
            && zone.Y == bounds.Value.Y
            && zone.Width == bounds.Value.Width
            && zone.Height == bounds.Value.Height);
    }

    private void UpsertTrapZoneFromSelection(string? zoneLabel, TrapZoneSeverity severity, string? zoneNotes)
    {
        var bounds = GetSelectionBounds();
        if (bounds is null)
        {
            return;
        }

        var existing = _project.TrapZones.FirstOrDefault(zone =>
            zone.X == bounds.Value.X
            && zone.Y == bounds.Value.Y
            && zone.Width == bounds.Value.Width
            && zone.Height == bounds.Value.Height);

        if (existing is null)
        {
            _project.TrapZones.Add(new TrapZonePlan
            {
                Label = string.IsNullOrWhiteSpace(zoneLabel) ? "Zone" : zoneLabel.Trim(),
                Severity = severity,
                Notes = string.IsNullOrWhiteSpace(zoneNotes) ? string.Empty : zoneNotes.Trim(),
                X = bounds.Value.X,
                Y = bounds.Value.Y,
                Width = Math.Max(1, bounds.Value.Width),
                Height = Math.Max(1, bounds.Value.Height)
            });
            return;
        }

        existing.Label = string.IsNullOrWhiteSpace(zoneLabel) ? existing.Label : zoneLabel.Trim();
        existing.Severity = severity;
        existing.Notes = string.IsNullOrWhiteSpace(zoneNotes) ? existing.Notes : zoneNotes.Trim();
    }

    private void RemoveTrapZonesOverlappingSelection()
    {
        var bounds = GetSelectionBounds();
        if (bounds is null)
        {
            return;
        }

        var selectionRect = new Rectangle(bounds.Value.X, bounds.Value.Y, Math.Max(1, bounds.Value.Width), Math.Max(1, bounds.Value.Height));
        _project.TrapZones.RemoveAll(zone => selectionRect.IntersectsWith(new Rectangle(zone.X, zone.Y, Math.Max(1, zone.Width), Math.Max(1, zone.Height))));
    }

    private int GetNextVisitorMarkerOrder()
        => _project.VisitorMarkers.Count == 0 ? 1 : _project.VisitorMarkers.Max(x => x.Order) + 1;

    private void NormalizeVisitorMarkerOrder()
    {
        var ordered = _project.VisitorMarkers.OrderBy(x => x.Order).ThenBy(x => x.Id).ToList();
        for (var i = 0; i < ordered.Count; i++)
        {
            ordered[i].Order = i + 1;
        }
    }

    private static string GetDefaultMarkerLabel(VisitorMarkerType type)
        => type switch
        {
            VisitorMarkerType.Ingress => "Ingress",
            VisitorMarkerType.Checkpoint => "Checkpoint",
            VisitorMarkerType.Egress => "Egress",
            _ => type.ToString()
        };

    private static string RemoveTrapPrefix(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var cleaned = value.Replace("[TRAP]", string.Empty, StringComparison.OrdinalIgnoreCase);
        var startIndex = cleaned.IndexOf("[TRAP:", StringComparison.OrdinalIgnoreCase);
        while (startIndex >= 0)
        {
            var endIndex = cleaned.IndexOf(']', startIndex);
            if (endIndex < 0)
            {
                break;
            }

            cleaned = cleaned.Remove(startIndex, endIndex - startIndex + 1);
            startIndex = cleaned.IndexOf("[TRAP:", StringComparison.OrdinalIgnoreCase);
        }

        return cleaned.Trim();
    }

    private static (Color Fill, Color Border) GetTrapZoneColors(TrapZoneSeverity severity)
        => severity switch
        {
            TrapZoneSeverity.Low => (Color.FromArgb(28, 255, 224, 120), Color.FromArgb(170, 255, 224, 120)),
            TrapZoneSeverity.High => (Color.FromArgb(38, 255, 128, 96), Color.FromArgb(182, 255, 128, 96)),
            TrapZoneSeverity.Critical => (Color.FromArgb(48, 255, 74, 74), Color.FromArgb(195, 255, 74, 74)),
            _ => (Color.FromArgb(32, 255, 188, 84), Color.FromArgb(176, 255, 188, 84))
        };

    private static Rectangle GetTrapZonePixelRect(Point origin, int cell, TrapZonePlan trapZone)
        => new(
            origin.X + (trapZone.X * cell),
            origin.Y + (trapZone.Y * cell),
            Math.Max(1, trapZone.Width) * cell,
            Math.Max(1, trapZone.Height) * cell);

    private static Rectangle GetTrapZoneResizeHandleRect(Point origin, int cell, TrapZonePlan trapZone)
    {
        var zoneRect = GetTrapZonePixelRect(origin, cell, trapZone);
        var handleSize = Math.Clamp(cell / 4, 7, 13);
        return new Rectangle(zoneRect.Right - handleSize - 2, zoneRect.Bottom - handleSize - 2, handleSize, handleSize);
    }


    private void RaiseProjectChanged() => ProjectChanged?.Invoke(this, EventArgs.Empty);
    private void RaiseSelectionChanged() => SelectionChanged?.Invoke(this, EventArgs.Empty);
    private void RaiseHistoryChanged() => HistoryChanged?.Invoke(this, EventArgs.Empty);
    private void RaiseBlueprintChanged() => BlueprintChanged?.Invoke(this, EventArgs.Empty);

    private readonly record struct StairAnchor(int X, int Y, int Rotation);
    private readonly record struct EdgeAnchor(int X, int Y, int Rotation);
}
