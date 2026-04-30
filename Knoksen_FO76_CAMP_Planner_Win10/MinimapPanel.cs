using System.Drawing.Drawing2D;

namespace FO76CampPlanner;

public sealed class MinimapPanel : Control
{
    private PlannerProject? _project;
    private IReadOnlyCollection<Guid> _selectedItemIds = Array.Empty<Guid>();
    private Point? _hoverGrid;

    public PlannerProject? Project
    {
        get => _project;
        set
        {
            _project = value;
            Invalidate();
        }
    }

    public IReadOnlyCollection<Guid> SelectedItemIds
    {
        get => _selectedItemIds;
        set
        {
            _selectedItemIds = value ?? Array.Empty<Guid>();
            Invalidate();
        }
    }

    public Point? HoverGrid
    {
        get => _hoverGrid;
        set
        {
            _hoverGrid = value;
            Invalidate();
        }
    }

    public MinimapPanel()
    {
        DoubleBuffered = true;
        BackColor = Color.FromArgb(27, 33, 41);
        ForeColor = Color.FromArgb(235, 239, 244);
        Margin = new Padding(0, 6, 0, 0);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(BackColor);

        using var borderPen = new Pen(Color.FromArgb(58, 68, 82), 1f);
        using var gridPen = new Pen(Color.FromArgb(44, 52, 63), 1f);
        using var textBrush = new SolidBrush(Color.FromArgb(170, 180, 192));
        using var titleFont = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
        using var textFont = new Font("Segoe UI", 8f, FontStyle.Regular);

        var header = new Rectangle(0, 0, Width - 1, 28);
        using (var headerBrush = new SolidBrush(Color.FromArgb(34, 40, 49)))
        {
            e.Graphics.FillRectangle(headerBrush, header);
        }
        e.Graphics.DrawRectangle(borderPen, header);
        e.Graphics.DrawString("Minimap", titleFont, Brushes.White, 10, 7);

        if (_project is null || _project.GridWidth <= 0 || _project.GridHeight <= 0)
        {
            e.Graphics.DrawString("No project loaded.", textFont, textBrush, 10, 40);
            return;
        }

        var viewport = new Rectangle(10, 36, Width - 20, Height - 46);
        e.Graphics.DrawRectangle(borderPen, viewport);

        var scaleX = viewport.Width / (float)_project.GridWidth;
        var scaleY = viewport.Height / (float)_project.GridHeight;
        var scale = Math.Min(scaleX, scaleY);
        var mapWidth = _project.GridWidth * scale;
        var mapHeight = _project.GridHeight * scale;
        var mapRect = new RectangleF(
            viewport.X + (viewport.Width - mapWidth) / 2f,
            viewport.Y + (viewport.Height - mapHeight) / 2f,
            mapWidth,
            mapHeight);

        for (var x = 0; x <= _project.GridWidth; x++)
        {
            var px = mapRect.X + (x * scale);
            e.Graphics.DrawLine(gridPen, px, mapRect.Y, px, mapRect.Bottom);
        }

        for (var y = 0; y <= _project.GridHeight; y++)
        {
            var py = mapRect.Y + (y * scale);
            e.Graphics.DrawLine(gridPen, mapRect.X, py, mapRect.Right, py);
        }

        foreach (var item in _project.Items)
        {
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var def))
            {
                continue;
            }

            var size = GetSize(def, item.Rotation);
            var rect = new RectangleF(
                mapRect.X + (item.X * scale),
                mapRect.Y + (item.Y * scale),
                Math.Max(2f, size.Width * scale),
                Math.Max(2f, size.Height * scale));

            using var fill = new SolidBrush(Color.FromArgb(190, def.DisplayColor));
            e.Graphics.FillRectangle(fill, rect);

            if (_selectedItemIds.Contains(item.Id))
            {
                using var selectedPen = new Pen(Color.FromArgb(246, 188, 57), Math.Max(1f, scale * 0.18f));
                e.Graphics.DrawRectangle(selectedPen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        var centerX = _project.CampCenterX >= 0 ? _project.CampCenterX : _project.GridWidth / 2;
        var centerY = _project.CampCenterY >= 0 ? _project.CampCenterY : _project.GridHeight / 2;
        var campCenterPixelX = mapRect.X + ((centerX + 0.5f) * scale);
        var campCenterPixelY = mapRect.Y + ((centerY + 0.5f) * scale);
        using (var centerPen = new Pen(Color.FromArgb(90, 255, 255, 255), 1.2f))
        {
            e.Graphics.DrawEllipse(centerPen, campCenterPixelX - 4, campCenterPixelY - 4, 8, 8);
            e.Graphics.DrawLine(centerPen, campCenterPixelX - 6, campCenterPixelY, campCenterPixelX + 6, campCenterPixelY);
            e.Graphics.DrawLine(centerPen, campCenterPixelX, campCenterPixelY - 6, campCenterPixelX, campCenterPixelY + 6);
        }

        if (_hoverGrid.HasValue)
        {
            var hover = _hoverGrid.Value;
            var hoverRect = new RectangleF(
                mapRect.X + (hover.X * scale),
                mapRect.Y + (hover.Y * scale),
                Math.Max(2f, scale),
                Math.Max(2f, scale));
            using var hoverPen = new Pen(Color.FromArgb(120, 116, 232, 171), 1.1f);
            e.Graphics.DrawRectangle(hoverPen, hoverRect.X, hoverRect.Y, hoverRect.Width, hoverRect.Height);
        }

        var footer = $"Grid {_project.GridWidth}×{_project.GridHeight}   Items {_project.Items.Count}";
        e.Graphics.DrawString(footer, textFont, textBrush, 10, Height - 18);
    }

    private static Size GetSize(ItemDefinition definition, int rotation)
    {
        var horizontal = rotation % 180 == 0;
        return horizontal ? new Size(definition.Width, definition.Height) : new Size(definition.Height, definition.Width);
    }
}
