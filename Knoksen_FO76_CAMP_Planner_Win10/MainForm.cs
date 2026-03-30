using System.Text.Json;

namespace FO76CampPlanner;

public sealed class MainForm : Form
{
    private readonly PlannerCanvas _canvas = new();
    private readonly ListBox _itemList = new();
    private readonly Label _selectionLabel = new();
    private readonly Label _modeLabel = new();
    private readonly Label _budgetLabel = new();
    private readonly Label _presetDescriptionLabel = new();
    private readonly Label _historyLabel = new();
    private readonly Label _blueprintLabel = new();
    private readonly Label _analysisLabel = new();
    private readonly Label _budgetProfileLabel = new();
    private readonly Label _routePlanningLabel = new();
    private readonly Label _workflowLabel = new();
    private readonly Label _focusLabel = new();
    private readonly Label _quickStartLabel = new();
    private readonly Label _inspectorHintLabel = new();
    private readonly Label _overviewHeroLabel = new();
    private readonly Label _overviewMetaLabel = new();
    private readonly Label _statusCardLabel = new();
    private readonly ProgressBar _budgetProgress = new();
    private readonly NumericUpDown _budgetLimitInput = new();
    private readonly NumericUpDown _storedBudgetInput = new();
    private readonly NumericUpDown _gridWidthInput = new();
    private readonly NumericUpDown _gridHeightInput = new();
    private readonly NumericUpDown _zoomInput = new();
    private readonly NumericUpDown _inspectorXInput = new();
    private readonly NumericUpDown _inspectorYInput = new();
    private readonly NumericUpDown _inspectorRotationInput = new();
    private readonly ComboBox _modeCombo = new();
    private readonly ComboBox _ruleCombo = new();
    private readonly ComboBox _presetCombo = new();
    private readonly ComboBox _budgetProfileCombo = new();
    private readonly ComboBox _overlayPresetCombo = new();
    private readonly ComboBox _markerTypeCombo = new();
    private readonly ComboBox _trapZoneSeverityCombo = new();
    private readonly CheckBox _snapEnabledCheck = new();
    private readonly ComboBox _campSlotCombo = new();
    private readonly ListBox _blueprintLibraryList = new();
    private readonly ListBox _visitorMarkerList = new();
    private readonly ListBox _trapZoneList = new();
    private readonly ComboBox _itemLayerFilterCombo = new();
    private readonly TextBox _projectNameText = new();
    private readonly TextBox _itemFilterText = new();
    private readonly TextBox _inspectorNoteText = new();
    private readonly TextBox _markerLabelText = new();
    private readonly TextBox _trapZoneLabelText = new();
    private readonly TextBox _trapZoneNotesText = new();
    private readonly TextBox _defenseReviewNotesText = new();
    private readonly MinimapPanel _minimap = new();
    private readonly StatusStrip _statusStrip = new();
    private readonly ToolStripStatusLabel _statusLabel = new("Ready.");
    private readonly Dictionary<ToolType, ToolStripButton> _toolButtons = new();
    private readonly Dictionary<LayerType, CheckBox> _layerChecks = new();
    private readonly Dictionary<LayerType, CheckBox> _layerLockChecks = new();
    private readonly Dictionary<string, Button> _workflowButtons = new();

    private ToolStripButton? _undoButton;
    private ToolStripButton? _redoButton;
    private CheckBox? _showCampRadiusCheck;
    private CheckBox? _showTurretCoverageCheck;
    private CheckBox? _showVisitorFlowCheck;
    private CheckBox? _showTrapZonesCheck;
    private string? _currentPath;
    private bool _suppressUiEvents;
    private bool _isDirty;

    private static readonly Color BgApp = Color.FromArgb(18, 22, 28);
    private static readonly Color BgPanel = Color.FromArgb(27, 33, 41);
    private static readonly Color BgCard = Color.FromArgb(34, 40, 49);
    private static readonly Color BgCanvasFrame = Color.FromArgb(21, 26, 33);
    private static readonly Color Accent = Color.FromArgb(246, 188, 57);
    private static readonly Color AccentSoft = Color.FromArgb(76, 246, 188, 57);
    private static readonly Color TextPrimary = Color.FromArgb(235, 239, 244);
    private static readonly Color TextSecondary = Color.FromArgb(170, 180, 192);
    private static readonly Color Border = Color.FromArgb(58, 68, 82);
    private static readonly Color BgPanelHeader = Color.FromArgb(39, 46, 56);
    private static readonly Color BgHover = Color.FromArgb(46, 54, 66);
    private static readonly Color BgPressed = Color.FromArgb(41, 48, 58);
    private static readonly Color StatusOk = Color.FromArgb(112, 211, 138);

    public MainForm()
    {
        Text = "FO76 CAMP Planner";
        MinimumSize = new Size(1420, 920);
        Width = 1680;
        Height = 980;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = BgApp;
        ForeColor = TextPrimary;
        Font = new Font("Segoe UI", 9f);

        BuildUi();
        WireEvents();
        ApplyTheme(this);
        NewProject();
    }

    private void BuildUi()
    {
        var mainContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            BackColor = BgApp,
            ForeColor = TextPrimary,
            FixedPanel = FixedPanel.Panel2,
            IsSplitterFixed = false,
            SplitterWidth = 8,
            Panel1MinSize = 0,
            Panel2MinSize = 0,
            Width = 1280  // Set a reasonable width to avoid validation errors
        };

        var leftLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = BgApp,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        leftLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        leftLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        leftLayout.Controls.Add(BuildTopHeader(), 0, 0);
        leftLayout.Controls.Add(BuildToolStrip(), 0, 1);

        var canvasPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10, 10, 10, 8),
            BackColor = BgApp
        };
        var canvasFrame = BuildCardPanel();
        canvasFrame.Padding = new Padding(10);
        canvasFrame.Dock = DockStyle.Fill;
        canvasFrame.BackColor = BgCanvasFrame;
        canvasFrame.Controls.Add(_canvas);
        canvasPanel.Controls.Add(canvasFrame);
        leftLayout.Controls.Add(canvasPanel, 0, 2);

        mainContainer.Panel1.Controls.Add(leftLayout);
        mainContainer.Panel2.Controls.Add(BuildRightPanel());

        _statusStrip.BackColor = BgPanel;
        _statusStrip.ForeColor = TextSecondary;
        _statusStrip.SizingGrip = false;
        _statusStrip.Items.Add(_statusLabel);

        Controls.Add(mainContainer);
        Controls.Add(_statusStrip);
        
        // Configure splitter constraints after adding to form
        mainContainer.Panel1MinSize = 900;
        mainContainer.Panel2MinSize = 360;
        mainContainer.SplitterDistance = 1120;
    }

    private Control BuildTopHeader()
    {
        var shell = new Panel
        {
            Dock = DockStyle.Top,
            Height = 132,
            Padding = new Padding(10, 10, 10, 0),
            BackColor = BgApp
        };

        var card = BuildCardPanel();
        card.Height = 118;
        card.Dock = DockStyle.Top;
        card.Padding = new Padding(16, 12, 16, 12);

        _overviewHeroLabel.Text = "CAMP Planner";
        _overviewHeroLabel.Font = new Font("Segoe UI Semibold", 17f, FontStyle.Bold);
        _overviewHeroLabel.AutoSize = true;
        _overviewHeroLabel.ForeColor = TextPrimary;
        _overviewHeroLabel.Location = new Point(12, 10);

        _overviewMetaLabel.Text = "New here? Start with foundations, then follow Layout → Envelope → Systems → Defense → Polish.";
        _overviewMetaLabel.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        _overviewMetaLabel.AutoSize = true;
        _overviewMetaLabel.ForeColor = TextSecondary;
        _overviewMetaLabel.Location = new Point(14, 44);

        _statusCardLabel.Text = "First-run guide active • Step-by-step workflow • Blueprint + zone-ready";
        _statusCardLabel.AutoSize = false;
        _statusCardLabel.TextAlign = ContentAlignment.MiddleRight;
        _statusCardLabel.ForeColor = Accent;
        _statusCardLabel.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
        _statusCardLabel.Dock = DockStyle.Right;
        _statusCardLabel.Width = 410;

        var workflowBar = BuildWorkflowHeaderBar();
        workflowBar.Location = new Point(12, 72);

        card.Controls.Add(_overviewHeroLabel);
        card.Controls.Add(_overviewMetaLabel);
        card.Controls.Add(_statusCardLabel);
        card.Controls.Add(workflowBar);
        shell.Controls.Add(card);
        return shell;
    }

    private Control BuildToolStrip()
    {
        var host = new Panel
        {
            Dock = DockStyle.Top,
            Height = 116,
            Padding = new Padding(10, 2, 10, 2),
            BackColor = BgApp
        };

        var strip = new ToolStrip
        {
            Dock = DockStyle.Top,
            GripStyle = ToolStripGripStyle.Hidden,
            RenderMode = ToolStripRenderMode.System,
            Padding = new Padding(8),
            AutoSize = false,
            Height = 102,
            BackColor = BgPanel,
            ForeColor = TextPrimary,
            CanOverflow = true,
            LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow
        };

        var newBtn = BuildToolAction("New", (_, _) => NewProject());
        var openBtn = BuildToolAction("Open", (_, _) => OpenProject());
        var saveBtn = BuildToolAction("Save", (_, _) => SaveProject(false));
        var saveAsBtn = BuildToolAction("Save As", (_, _) => SaveProject(true));
        var exportBtn = BuildToolAction("Export PNG", (_, _) => ExportPng());
        var rotateBtn = BuildToolAction("Rotate", (_, _) => _canvas.RotateSelected(), "Rotate selection (R)");
        var pasteBlueprintBtn = BuildToolAction("Paste Blueprint", (_, _) => _canvas.PasteLoadedBlueprint());

        _undoButton = BuildToolAction("Undo", (_, _) => _canvas.Undo(), "Undo (Ctrl+Z)");
        _undoButton.Enabled = false;
        _redoButton = BuildToolAction("Redo", (_, _) => _canvas.Redo(), "Redo (Ctrl+Y)");
        _redoButton.Enabled = false;

        strip.Items.AddRange(new ToolStripItem[]
        {
            newBtn, openBtn, saveBtn, saveAsBtn, new ToolStripSeparator(),
            exportBtn, rotateBtn, pasteBlueprintBtn,
            BuildToolAction("Duplicate", (_, _) => _canvas.DuplicateSelection(), "Duplicate selection"),
            BuildToolAction("Dup →", (_, _) => _canvas.QuickDuplicateZone(1, 0), "Quick duplicate right"),
            BuildToolAction("Dup ↓", (_, _) => _canvas.QuickDuplicateZone(0, 1), "Quick duplicate down"),
            BuildToolAction("Snap", (_, _) => ToggleSnapMode(), "Toggle smart snap on/off"),
            BuildToolAction("Delete", (_, _) => _canvas.DeleteSelection(), "Delete selection"),
            _undoButton, _redoButton, new ToolStripSeparator()
        });

        AddToolButton(strip, ToolType.Select, "Select", "Select / move / multiselect");
        AddToolButton(strip, ToolType.Foundation, "Foundation", "Foundation cell");
        AddToolButton(strip, ToolType.Wall, "Wall", "Snap to foundation edge");
        AddToolButton(strip, ToolType.Door, "Door", "Snap to foundation edge");
        AddToolButton(strip, ToolType.Stairs, "Stairs", "Snap to open edge");
        AddToolButton(strip, ToolType.Roof, "Roof", "Snap to supported foundation");
        AddToolButton(strip, ToolType.Workbench, "Workbench", "Utility object");
        AddToolButton(strip, ToolType.Turret, "Turret", "Defense coverage preview");
        AddToolButton(strip, ToolType.Power, "Power", "Generators / power nodes");
        AddToolButton(strip, ToolType.Light, "Light", "Lighting layer");
        AddToolButton(strip, ToolType.Decor, "Decor", "Aesthetic object");
        AddToolButton(strip, ToolType.Vendor, "Vendor", "Commerce object");
        AddToolButton(strip, ToolType.Resource, "Resource", "Resource object");
        AddToolButton(strip, ToolType.Display, "Display", "Display object");
        AddToolButton(strip, ToolType.Ally, "Ally", "Ally station");
        AddToolButton(strip, ToolType.Erase, "Erase", "Delete clicked object");

        host.Controls.Add(strip);
        return host;
    }

    private ToolStripButton BuildToolAction(string text, EventHandler click, string? tooltip = null)
    {
        var button = new ToolStripButton(text)
        {
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            AutoSize = false,
            Width = text.Length > 10 ? 112 : 86,
            Height = 32,
            Margin = new Padding(2, 2, 2, 6),
            BackColor = BgCard,
            ForeColor = TextPrimary,
            ToolTipText = tooltip ?? text
        };
        button.Click += click;
        return button;
    }

    private void AddToolButton(ToolStrip strip, ToolType tool, string text, string tooltip)
    {
        var button = new ToolStripButton(text)
        {
            CheckOnClick = true,
            Tag = tool,
            ToolTipText = tooltip,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            AutoSize = false,
            Width = text.Length > 9 ? 92 : 74,
            Height = 32,
            Margin = new Padding(2, 2, 2, 6),
            BackColor = BgCard,
            ForeColor = TextPrimary
        };

        button.Click += (_, _) => SetActiveTool(tool);
        _toolButtons[tool] = button;
        strip.Items.Add(button);
    }

    private Control BuildRightPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            BackColor = BgApp
        };

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            DrawMode = TabDrawMode.OwnerDrawFixed,
            SizeMode = TabSizeMode.Fixed,
            ItemSize = new Size(88, 30),
            Padding = new Point(18, 6),
            Appearance = TabAppearance.Normal
        };
        tabs.DrawItem += DrawTab;

        var overviewPage = MakeTabPage("Overview");
        var buildPage = MakeTabPage("Build");
        var libraryPage = MakeTabPage("Library");
        var inspectPage = MakeTabPage("Inspect");

        overviewPage.Controls.Add(BuildOverviewTab());
        buildPage.Controls.Add(BuildBuildTab());
        libraryPage.Controls.Add(BuildLibraryTab());
        inspectPage.Controls.Add(BuildInspectTab());

        tabs.TabPages.Add(overviewPage);
        tabs.TabPages.Add(buildPage);
        tabs.TabPages.Add(libraryPage);
        tabs.TabPages.Add(inspectPage);

        panel.Controls.Add(tabs);
        return panel;
    }

    private TabPage MakeTabPage(string text)
        => new()
        {
            Text = text,
            BackColor = BgApp,
            ForeColor = TextPrimary,
            Padding = new Padding(0, 10, 0, 0)
        };

    private Control BuildOverviewTab()
    {
        var layout = BuildTabLayout();
        layout.Controls.Add(MakeSection("Project", BuildProjectSection()), 0, 0);
        layout.Controls.Add(MakeSection("Quick Start", BuildQuickStartSection()), 0, 1);
        layout.Controls.Add(MakeSection("Preset", BuildPresetSection()), 0, 2);
        layout.Controls.Add(MakeSection("Minimap", BuildMinimapSection()), 0, 3);
        layout.Controls.Add(MakeSection("Budget", BuildBudgetSection()), 0, 4);
        layout.Controls.Add(MakeSection("Workflow", BuildWorkflowSection()), 0, 5);
        layout.Controls.Add(MakeSection("Focus", BuildFocusSection()), 0, 6);
        layout.Controls.Add(MakeSection("Analysis", BuildAnalysisSection()), 0, 7);
        layout.Controls.Add(MakeSection("Route & Trap Planning", BuildRoutePlanningSection()), 0, 8);
        layout.Controls.Add(MakeSection("Notes", BuildNotesSection()), 0, 9);
        return layout;
    }

    private Control BuildBuildTab()
    {
        var layout = BuildTabLayout();
        layout.Controls.Add(MakeSection("Mode & Rules", BuildRulesSection()), 0, 0);
        layout.Controls.Add(MakeSection("Workflow", BuildWorkflowSection()), 0, 1);
        layout.Controls.Add(MakeSection("Grid", BuildGridSection()), 0, 2);
        layout.Controls.Add(MakeSection("Focus", BuildFocusSection()), 0, 3);
        layout.Controls.Add(MakeSection("Layers", BuildLayersSection()), 0, 4);
        return layout;
    }

    private Control BuildLibraryTab()
    {
        var layout = BuildTabLayout();
        layout.Controls.Add(MakeSection("Blueprints", BuildBlueprintSection()), 0, 0);
        layout.Controls.Add(MakeSection("Blueprint Library (per CAMP slot)", BuildBlueprintLibrarySection()), 0, 1);
        layout.Controls.Add(MakeSection("Placed Items", BuildItemsSection()), 0, 2);
        return layout;
    }

    private Control BuildInspectTab()
    {
        var layout = BuildTabLayout();
        layout.Controls.Add(MakeSection("Selection", BuildSelectionSection()), 0, 0);
        layout.Controls.Add(MakeSection("Inspector", BuildInspectorSection()), 0, 1);
        layout.Controls.Add(MakeSection("Quick Actions", BuildInspectorActionsSection()), 0, 2);
        return layout;
    }

    private static TableLayoutPanel BuildTabLayout()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            AutoScroll = true,
            BackColor = BgApp,
            Padding = new Padding(0, 0, 0, 20)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        return layout;
    }


    private Control BuildWorkflowHeaderBar()
    {
        var flow = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };

        foreach (var stage in new[] { "Layout", "Envelope", "Systems", "Defense", "Polish" })
        {
            var button = new Button
            {
                Text = stage,
                Width = 94,
                Height = 28,
                BackColor = BgCard,
                ForeColor = TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 6, 0),
                Tag = stage
            };
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = BgHover;
            button.FlatAppearance.MouseDownBackColor = BgPressed;
            button.Click += (_, _) => ActivateWorkflowStage(stage);
            _workflowButtons[stage] = button;
            flow.Controls.Add(button);
        }

        return flow;
    }

    private Control BuildWorkflowSection()
    {
        var flow = BuildVerticalFlow();
        _workflowLabel.AutoSize = true;
        _workflowLabel.MaximumSize = new Size(300, 0);
        _workflowLabel.ForeColor = TextSecondary;

        var buttonRow1 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        buttonRow1.Controls.Add(BuildGhostButton("1. Layout", (_, _) => ActivateWorkflowStage("Layout")));
        buttonRow1.Controls.Add(BuildGhostButton("2. Envelope", (_, _) => ActivateWorkflowStage("Envelope")));

        var buttonRow2 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        buttonRow2.Controls.Add(BuildGhostButton("3. Systems", (_, _) => ActivateWorkflowStage("Systems")));
        buttonRow2.Controls.Add(BuildGhostButton("4. Defense", (_, _) => ActivateWorkflowStage("Defense")));

        var buttonRow3 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        buttonRow3.Controls.Add(BuildActionButton("5. Polish / Presentation", (_, _) => ActivateWorkflowStage("Polish"), true));

        flow.Controls.Add(BuildInfoPill(_workflowLabel));
        flow.Controls.Add(buttonRow1);
        flow.Controls.Add(buttonRow2);
        flow.Controls.Add(buttonRow3);
        return flow;
    }

    private Control BuildFocusSection()
    {
        var flow = BuildVerticalFlow();
        _focusLabel.AutoSize = true;
        _focusLabel.MaximumSize = new Size(300, 0);
        _focusLabel.ForeColor = TextSecondary;

        var row1 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        row1.Controls.Add(BuildGhostButton("All layers", (_, _) => ApplyFocusPreset("All")));
        row1.Controls.Add(BuildGhostButton("Structure", (_, _) => ApplyFocusPreset("Structure")));

        var row2 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        row2.Controls.Add(BuildGhostButton("Systems", (_, _) => ApplyFocusPreset("Systems")));
        row2.Controls.Add(BuildGhostButton("Defense", (_, _) => ApplyFocusPreset("Defense")));

        var row3 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        row3.Controls.Add(BuildActionButton("Presentation view", (_, _) => ApplyFocusPreset("Presentation"), true));

        flow.Controls.Add(BuildInfoPill(_focusLabel));
        flow.Controls.Add(row1);
        flow.Controls.Add(row2);
        flow.Controls.Add(row3);
        return flow;
    }


    private Control BuildQuickStartSection()
    {
        var flow = BuildVerticalFlow();
        _quickStartLabel.AutoSize = true;
        _quickStartLabel.MaximumSize = new Size(300, 0);
        _quickStartLabel.ForeColor = TextSecondary;

        var actions = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        actions.Controls.Add(BuildActionButton("Duplicate selection", (_, _) => _canvas.DuplicateSelection()));
        actions.Controls.Add(BuildGhostButton("Delete selection", (_, _) => _canvas.DeleteSelection()));

        var actions2 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        actions2.Controls.Add(BuildGhostButton("Set CAMP center", (_, _) => _canvas.SetCampCenterFromSelection()));
        actions2.Controls.Add(BuildGhostButton("Paste blueprint", (_, _) => _canvas.PasteLoadedBlueprint()));

        var actions3 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        actions3.Controls.Add(BuildGhostButton("Dup →", (_, _) => _canvas.QuickDuplicateZone(1, 0)));
        actions3.Controls.Add(BuildGhostButton("Dup ←", (_, _) => _canvas.QuickDuplicateZone(-1, 0)));
        actions3.Controls.Add(BuildGhostButton("Dup ↓", (_, _) => _canvas.QuickDuplicateZone(0, 1)));
        actions3.Controls.Add(BuildGhostButton("Dup ↑", (_, _) => _canvas.QuickDuplicateZone(0, -1)));

        flow.Controls.Add(BuildInfoPill(_quickStartLabel));
        flow.Controls.Add(actions);
        flow.Controls.Add(actions2);
        flow.Controls.Add(actions3);
        return flow;
    }

    private Control BuildMinimapSection()
    {
        var flow = BuildVerticalFlow();
        _minimap.Width = 300;
        _minimap.Height = 220;
        _minimap.Margin = new Padding(0, 0, 0, 0);
        flow.Controls.Add(_minimap);
        return flow;
    }

    private Control BuildProjectSection()
    {
        var flow = BuildVerticalFlow();
        _projectNameText.Width = 300;
        _projectNameText.PlaceholderText = "Project name";
        flow.Controls.Add(MakeLabel("Project name"));
        flow.Controls.Add(_projectNameText);
        flow.Controls.Add(BuildInfoPill(_historyLabel));
        return flow;
    }

    private Control BuildPresetSection()
    {
        var flow = BuildVerticalFlow();
        _presetCombo.Width = 300;
        _presetCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _presetCombo.DataSource = PresetLibrary.All.ToList();
        _presetCombo.DisplayMember = nameof(ProjectPreset.Name);
        _presetCombo.ValueMember = nameof(ProjectPreset.Id);

        _presetDescriptionLabel.AutoSize = true;
        _presetDescriptionLabel.MaximumSize = new Size(300, 0);
        _presetDescriptionLabel.ForeColor = TextSecondary;

        var buttons = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        buttons.Controls.Add(BuildActionButton("Apply preset", (_, _) => ApplySelectedPreset(), true));
        buttons.Controls.Add(BuildGhostButton("New custom", (_, _) => NewProject()));

        flow.Controls.Add(MakeLabel("Choose preset"));
        flow.Controls.Add(_presetCombo);
        flow.Controls.Add(_presetDescriptionLabel);
        flow.Controls.Add(buttons);
        return flow;
    }

    private Control BuildRulesSection()
    {
        var flow = BuildVerticalFlow();
        _modeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _modeCombo.Width = 300;
        _modeCombo.DataSource = Enum.GetValues<BuildMode>();

        _ruleCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _ruleCombo.Width = 300;
        _ruleCombo.DataSource = Enum.GetValues<RuleProfile>();

        _snapEnabledCheck.Text = "Smart snap enabled";
        _snapEnabledCheck.Checked = true;
        _snapEnabledCheck.AutoSize = true;
        _snapEnabledCheck.ForeColor = TextPrimary;
        _snapEnabledCheck.BackColor = Color.Transparent;
        _snapEnabledCheck.Margin = new Padding(0, 4, 0, 2);
        _snapEnabledCheck.CheckedChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.SnapEnabled = _snapEnabledCheck.Checked;
            _canvas.Invalidate();
            RefreshProjectUi();
        };

        _zoomInput.Minimum = 50;
        _zoomInput.Maximum = 250;
        _zoomInput.Increment = 10;
        _zoomInput.Width = 140;

        flow.Controls.Add(MakeLabel("Build mode"));
        flow.Controls.Add(_modeCombo);
        flow.Controls.Add(MakeLabel("Rule profile"));
        flow.Controls.Add(_ruleCombo);
        flow.Controls.Add(_snapEnabledCheck);
        flow.Controls.Add(MakeLabel("Zoom %"));
        flow.Controls.Add(_zoomInput);
        flow.Controls.Add(BuildInfoPill(_modeLabel));
        return flow;
    }

    private Control BuildBudgetSection()
    {
        var flow = BuildVerticalFlow();

        _budgetProfileCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _budgetProfileCombo.Width = 300;
        _budgetProfileCombo.DataSource = Enum.GetValues<BudgetPlaystyleProfile>();

        _budgetProfileLabel.AutoSize = true;
        _budgetProfileLabel.MaximumSize = new Size(300, 0);
        _budgetProfileLabel.ForeColor = TextSecondary;

        _budgetLimitInput.Minimum = 100;
        _budgetLimitInput.Maximum = 100000;
        _budgetLimitInput.Increment = 50;
        _budgetLimitInput.Width = 160;

        _storedBudgetInput.Minimum = 0;
        _storedBudgetInput.Maximum = 100000;
        _storedBudgetInput.Increment = 10;
        _storedBudgetInput.Width = 160;

        _budgetProgress.Width = 300;
        _budgetProgress.Height = 18;
        _budgetLabel.AutoSize = true;
        _budgetLabel.MaximumSize = new Size(300, 0);
        _budgetLabel.ForeColor = TextSecondary;

        flow.Controls.Add(MakeLabel("Budget profile"));
        flow.Controls.Add(_budgetProfileCombo);
        flow.Controls.Add(BuildInfoPill(_budgetProfileLabel));
        flow.Controls.Add(MakeLabel("Budget limit"));
        flow.Controls.Add(_budgetLimitInput);
        flow.Controls.Add(MakeLabel("Stored budget"));
        flow.Controls.Add(_storedBudgetInput);
        flow.Controls.Add(_budgetProgress);
        flow.Controls.Add(_budgetLabel);
        return flow;
    }

    private Control BuildGridSection()
    {
        var flow = BuildVerticalFlow();

        _gridWidthInput.Minimum = 10;
        _gridWidthInput.Maximum = 100;
        _gridWidthInput.Width = 160;

        _gridHeightInput.Minimum = 10;
        _gridHeightInput.Maximum = 100;
        _gridHeightInput.Width = 160;

        flow.Controls.Add(MakeLabel("Grid width"));
        flow.Controls.Add(_gridWidthInput);
        flow.Controls.Add(MakeLabel("Grid height"));
        flow.Controls.Add(_gridHeightInput);
        return flow;
    }

    private Control BuildAnalysisSection()
    {
        var flow = BuildVerticalFlow();
        _showCampRadiusCheck = BuildCheck("Show CAMP radius overlay", true, (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.ShowCampRadiusOverlay = _showCampRadiusCheck?.Checked ?? true;
            _canvas.Invalidate();
            RefreshProjectUi();
        });

        _showTurretCoverageCheck = BuildCheck("Show turret coverage", true, (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.ShowTurretCoverage = _showTurretCoverageCheck?.Checked ?? true;
            _canvas.Invalidate();
            RefreshProjectUi();
        });

        _showVisitorFlowCheck = BuildCheck("Show visitor flow", true, (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.ShowVisitorFlowOverlay = _showVisitorFlowCheck?.Checked ?? true;
            _canvas.Invalidate();
            RefreshProjectUi();
        });

        _showTrapZonesCheck = BuildCheck("Show trap zones", true, (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.ShowTrapZonesOverlay = _showTrapZonesCheck?.Checked ?? true;
            _canvas.Invalidate();
            RefreshProjectUi();
        });

        _analysisLabel.AutoSize = true;
        _analysisLabel.MaximumSize = new Size(300, 0);
        _analysisLabel.ForeColor = TextSecondary;

        flow.Controls.Add(_showCampRadiusCheck);
        flow.Controls.Add(_showTurretCoverageCheck);
        flow.Controls.Add(_showVisitorFlowCheck);
        flow.Controls.Add(_showTrapZonesCheck);
        flow.Controls.Add(BuildInfoPill(_analysisLabel));
        return flow;
    }

    private Control BuildRoutePlanningSection()
    {
        var flow = BuildVerticalFlow();

        _overlayPresetCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _overlayPresetCombo.Width = 300;
        _overlayPresetCombo.DataSource = Enum.GetValues<OverlayReviewPreset>();

        _markerTypeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _markerTypeCombo.Width = 300;
        _markerTypeCombo.DataSource = Enum.GetValues<VisitorMarkerType>();

        _trapZoneSeverityCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _trapZoneSeverityCombo.Width = 300;
        _trapZoneSeverityCombo.DataSource = Enum.GetValues<TrapZoneSeverity>();

        _markerLabelText.Width = 300;
        _markerLabelText.PlaceholderText = "Marker label";

        _trapZoneLabelText.Width = 300;
        _trapZoneLabelText.PlaceholderText = "Trap zone label";

        _trapZoneNotesText.Width = 300;
        _trapZoneNotesText.Height = 62;
        _trapZoneNotesText.Multiline = true;
        _trapZoneNotesText.ScrollBars = ScrollBars.Vertical;

        _defenseReviewNotesText.Width = 300;
        _defenseReviewNotesText.Height = 74;
        _defenseReviewNotesText.Multiline = true;
        _defenseReviewNotesText.ScrollBars = ScrollBars.Vertical;

        _visitorMarkerList.Width = 300;
        _visitorMarkerList.Height = 136;
        _visitorMarkerList.SelectionMode = SelectionMode.One;
        _visitorMarkerList.BorderStyle = BorderStyle.FixedSingle;
        _visitorMarkerList.BackColor = BgCard;
        _visitorMarkerList.ForeColor = TextPrimary;
        _visitorMarkerList.SelectedIndexChanged += (_, _) => PopulateVisitorMarkerEditor();

        _trapZoneList.Width = 300;
        _trapZoneList.Height = 118;
        _trapZoneList.SelectionMode = SelectionMode.One;
        _trapZoneList.BorderStyle = BorderStyle.FixedSingle;
        _trapZoneList.BackColor = BgCard;
        _trapZoneList.ForeColor = TextPrimary;
        _trapZoneList.SelectedIndexChanged += (_, _) => PopulateTrapZoneEditor();

        _routePlanningLabel.AutoSize = true;
        _routePlanningLabel.MaximumSize = new Size(300, 0);
        _routePlanningLabel.ForeColor = TextSecondary;

        var markerRow1 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        markerRow1.Controls.Add(BuildGhostButton("Add ingress", (_, _) => _canvas.AddVisitorMarker(VisitorMarkerType.Ingress)));
        markerRow1.Controls.Add(BuildGhostButton("Add checkpoint", (_, _) => _canvas.AddVisitorMarker(VisitorMarkerType.Checkpoint)));

        var markerRow2 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        markerRow2.Controls.Add(BuildGhostButton("Add egress", (_, _) => _canvas.AddVisitorMarker(VisitorMarkerType.Egress)));
        markerRow2.Controls.Add(BuildGhostButton("Apply marker", (_, _) => ApplySelectedVisitorMarker()));

        var markerRow3 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        markerRow3.Controls.Add(BuildGhostButton("Move up", (_, _) => MoveSelectedVisitorMarker(-1)));
        markerRow3.Controls.Add(BuildGhostButton("Move down", (_, _) => MoveSelectedVisitorMarker(1)));
        markerRow3.Controls.Add(BuildGhostButton("Remove marker", (_, _) => RemoveSelectedVisitorMarker()));

        var trapRow = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent,
            Width = 300
        };
        trapRow.Controls.Add(BuildGhostButton("Zone: Funnel", (_, _) => CreateTrapZoneFromSelection("Funnel", TrapZoneSeverity.Medium)));
        trapRow.Controls.Add(BuildGhostButton("Zone: Kill Box", (_, _) => CreateTrapZoneFromSelection("Kill Box", TrapZoneSeverity.Critical)));
        trapRow.Controls.Add(BuildGhostButton("Zone: Delay", (_, _) => CreateTrapZoneFromSelection("Delay", TrapZoneSeverity.High)));

        var trapRow2 = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        trapRow2.Controls.Add(BuildGhostButton("Apply zone", (_, _) => ApplySelectedTrapZone()));
        trapRow2.Controls.Add(BuildGhostButton("Remove zone", (_, _) => RemoveSelectedTrapZone()));
        trapRow2.Controls.Add(BuildGhostButton("Clear markers", (_, _) => _canvas.ClearVisitorMarkers()));

        flow.Controls.Add(MakeLabel("Overlay review preset"));
        flow.Controls.Add(_overlayPresetCombo);
        flow.Controls.Add(BuildInfoPill(_routePlanningLabel));
        flow.Controls.Add(MakeLabel("Route steps"));
        flow.Controls.Add(_visitorMarkerList);
        flow.Controls.Add(MakeLabel("Marker label"));
        flow.Controls.Add(_markerLabelText);
        flow.Controls.Add(MakeLabel("Marker type"));
        flow.Controls.Add(_markerTypeCombo);
        flow.Controls.Add(markerRow1);
        flow.Controls.Add(markerRow2);
        flow.Controls.Add(markerRow3);
        flow.Controls.Add(MakeLabel("Trap zones"));
        flow.Controls.Add(_trapZoneList);
        flow.Controls.Add(MakeLabel("Zone label"));
        flow.Controls.Add(_trapZoneLabelText);
        flow.Controls.Add(MakeLabel("Severity"));
        flow.Controls.Add(_trapZoneSeverityCombo);
        flow.Controls.Add(MakeLabel("Zone notes"));
        flow.Controls.Add(_trapZoneNotesText);
        flow.Controls.Add(trapRow);
        flow.Controls.Add(trapRow2);
        flow.Controls.Add(MakeLabel("Defense review notes"));
        flow.Controls.Add(_defenseReviewNotesText);
        flow.Controls.Add(BuildGhostButton("Save review notes", (_, _) => ApplyDefenseReviewNotes()));
        return flow;
    }

    private Control BuildLayersSection()
    {
        var flow = BuildVerticalFlow();
        foreach (var layer in Enum.GetValues<LayerType>())
        {
            var row = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                Width = 300,
                Height = 30,
                Margin = new Padding(0, 2, 0, 2),
                BackColor = Color.Transparent
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70f));

            var swatch = new Panel
            {
                Width = 12,
                Height = 12,
                Margin = new Padding(0, 7, 6, 0),
                BackColor = GetLayerSwatch(layer)
            };

            var check = new CheckBox
            {
                Text = layer.ToString(),
                AutoSize = true,
                Checked = true,
                ForeColor = TextPrimary,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 4, 8, 2)
            };
            check.CheckedChanged += (_, _) =>
            {
                if (_suppressUiEvents) return;
                _canvas.SetLayerVisibility(layer, check.Checked);
            };

            var lockCheck = new CheckBox
            {
                Text = "Lock",
                Checked = false,
                AutoSize = true,
                ForeColor = Accent,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 4, 0, 2)
            };
            lockCheck.CheckedChanged += (_, _) =>
            {
                if (_suppressUiEvents) return;
                _canvas.SetLayerLocked(layer, lockCheck.Checked);
            };

            _layerChecks[layer] = check;
            _layerLockChecks[layer] = lockCheck;
            row.Controls.Add(swatch, 0, 0);
            row.Controls.Add(check, 1, 0);
            row.Controls.Add(lockCheck, 2, 0);
            flow.Controls.Add(row);
        }

        var buttons = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 8, 0, 0),
            BackColor = Color.Transparent
        };
        buttons.Controls.Add(BuildGhostButton("Show all", (_, _) => _canvas.ShowAllLayers()));
        buttons.Controls.Add(BuildGhostButton("Unlock all", (_, _) => _canvas.UnlockAllLayers()));
        flow.Controls.Add(buttons);
        return flow;
    }

    private Control BuildSelectionSection()
    {
        var flow = BuildVerticalFlow();
        _selectionLabel.AutoSize = true;
        _selectionLabel.MaximumSize = new Size(300, 0);
        _selectionLabel.Text = "No selection.";
        _selectionLabel.ForeColor = TextSecondary;
        flow.Controls.Add(BuildInfoPill(_selectionLabel));
        return flow;
    }


    private Control BuildInspectorSection()
    {
        var flow = BuildVerticalFlow();

        _inspectorXInput.Minimum = 0;
        _inspectorXInput.Maximum = 1000;
        _inspectorXInput.Width = 90;

        _inspectorYInput.Minimum = 0;
        _inspectorYInput.Maximum = 1000;
        _inspectorYInput.Width = 90;

        _inspectorRotationInput.Minimum = 0;
        _inspectorRotationInput.Maximum = 270;
        _inspectorRotationInput.Increment = 90;
        _inspectorRotationInput.Width = 90;

        _inspectorNoteText.Width = 300;
        _inspectorNoteText.PlaceholderText = "Selection note";

        _inspectorHintLabel.AutoSize = true;
        _inspectorHintLabel.MaximumSize = new Size(300, 0);
        _inspectorHintLabel.ForeColor = TextSecondary;

        var coordsRow = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        coordsRow.Controls.Add(MakeLabel("X"));
        coordsRow.Controls.Add(_inspectorXInput);
        coordsRow.Controls.Add(MakeLabel("Y"));
        coordsRow.Controls.Add(_inspectorYInput);
        coordsRow.Controls.Add(MakeLabel("Rot"));
        coordsRow.Controls.Add(_inspectorRotationInput);

        flow.Controls.Add(MakeLabel("Note"));
        flow.Controls.Add(_inspectorNoteText);
        flow.Controls.Add(coordsRow);
        flow.Controls.Add(BuildActionButton("Apply inspector changes", (_, _) => ApplyInspectorChanges(), true));
        flow.Controls.Add(BuildInfoPill(_inspectorHintLabel));
        return flow;
    }

    private Control BuildInspectorActionsSection()
    {
        var flow = BuildVerticalFlow();
        flow.Controls.Add(BuildActionButton("Rotate selection", (_, _) => _canvas.RotateSelected(), true));
        flow.Controls.Add(BuildActionButton("Duplicate selection", (_, _) => _canvas.DuplicateSelection()));
        flow.Controls.Add(BuildActionButton("Delete selection", (_, _) => _canvas.DeleteSelection()));
        flow.Controls.Add(BuildGhostButton("Tag trap zone", (_, _) => _canvas.TagSelectionAsTrapZone(true)));
        flow.Controls.Add(BuildGhostButton("Clear trap tag", (_, _) => _canvas.TagSelectionAsTrapZone(false)));
        flow.Controls.Add(BuildGhostButton("Set CAMP center from selection", (_, _) => _canvas.SetCampCenterFromSelection()));
        return flow;
    }

    private Control BuildBlueprintSection()
    {
        var flow = BuildVerticalFlow();
        _blueprintLabel.AutoSize = true;
        _blueprintLabel.MaximumSize = new Size(300, 0);
        _blueprintLabel.ForeColor = TextSecondary;

        flow.Controls.Add(BuildInfoPill(_blueprintLabel));
        flow.Controls.Add(BuildActionButton("Save selection as blueprint", (_, _) => SaveSelectionAsBlueprint()));
        flow.Controls.Add(BuildActionButton("Load blueprint", (_, _) => LoadBlueprint()));
        flow.Controls.Add(BuildGhostButton("Paste loaded blueprint", (_, _) => _canvas.PasteLoadedBlueprint()));
        flow.Controls.Add(BuildGhostButton("Clear loaded blueprint", (_, _) => _canvas.ClearLoadedBlueprint()));
        return flow;
    }

    private Control BuildBlueprintLibrarySection()
    {
        var flow = BuildVerticalFlow();

        _campSlotCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _campSlotCombo.Width = 300;
        _campSlotCombo.DataSource = Enum.GetValues<CampSlot>();
        _campSlotCombo.SelectedIndexChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            if (_campSlotCombo.SelectedItem is CampSlot slot)
            {
                _canvas.Project.ActiveCampSlot = slot;
                RefreshBlueprintLibraryUi();
            }
        };

        _blueprintLibraryList.Width = 300;
        _blueprintLibraryList.Height = 180;
        _blueprintLibraryList.SelectionMode = SelectionMode.One;
        _blueprintLibraryList.BorderStyle = BorderStyle.FixedSingle;
        _blueprintLibraryList.BackColor = BgCard;
        _blueprintLibraryList.ForeColor = TextPrimary;

        var buttons = new FlowLayoutPanel
        {
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };
        buttons.Controls.Add(BuildGhostButton("Add loaded to slot", (_, _) => AddLoadedBlueprintToLibrary()));
        buttons.Controls.Add(BuildGhostButton("Load from slot", (_, _) => LoadBlueprintFromLibrary()));
        buttons.Controls.Add(BuildGhostButton("Remove", (_, _) => RemoveBlueprintFromLibrary()));

        flow.Controls.Add(MakeLabel("Active CAMP slot"));
        flow.Controls.Add(_campSlotCombo);
        flow.Controls.Add(MakeLabel("Slot library"));
        flow.Controls.Add(_blueprintLibraryList);
        flow.Controls.Add(buttons);
        return flow;
    }

    private Control BuildItemsSection()
    {
        var flow = BuildVerticalFlow();

        _itemFilterText.Width = 300;
        _itemFilterText.PlaceholderText = "Filter by item name, note, or grid coords";

        _itemLayerFilterCombo.Width = 300;
        _itemLayerFilterCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _itemLayerFilterCombo.Items.Add("All layers");
        foreach (var layer in Enum.GetValues<LayerType>())
        {
            _itemLayerFilterCombo.Items.Add(layer);
        }
        _itemLayerFilterCombo.SelectedIndex = 0;

        _itemList.Width = 300;
        _itemList.Height = 360;
        _itemList.SelectionMode = SelectionMode.MultiExtended;
        _itemList.DrawMode = DrawMode.OwnerDrawFixed;
        _itemList.ItemHeight = 26;
        _itemList.BorderStyle = BorderStyle.FixedSingle;
        _itemList.BackColor = BgCard;
        _itemList.ForeColor = TextPrimary;
        _itemList.DrawItem += DrawItemEntry;

        flow.Controls.Add(MakeLabel("Search"));
        flow.Controls.Add(_itemFilterText);
        flow.Controls.Add(MakeLabel("Layer filter"));
        flow.Controls.Add(_itemLayerFilterCombo);
        flow.Controls.Add(_itemList);
        return flow;
    }

    private Control BuildNotesSection()
    {
        var panel = BuildVerticalFlow();
        var help = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(300, 0),
            ForeColor = TextSecondary,
            Text = "Onboarding checklist:\n" +
                   "• Start in Layout and place foundations first.\n" +
                   "• Switch stages using the top workflow buttons.\n" +
                   "• Use Blueprint tools to save/load/paste repeatable modules.\n" +
                   "• Keep zones clear: entry, crafting, defense, vendor.\n" +
                   "• Validate visitor flow with focus presets before final polish."
        };
        panel.Controls.Add(help);
        return panel;
    }

    private static FlowLayoutPanel BuildVerticalFlow()
        => new()
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            Dock = DockStyle.Top,
            Width = 312,
            Margin = new Padding(0)
        };

    private Button BuildActionButton(string text, EventHandler click, bool primary = false)
    {
        var button = new Button
        {
            Text = text,
            Width = text.Length > 18 ? 216 : 156,
            Height = 36,
            BackColor = primary ? Accent : BgCard,
            ForeColor = primary ? Color.FromArgb(35, 26, 0) : TextPrimary,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0, 6, 0, 0)
        };
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = primary ? Accent : Border;
        button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(255, 206, 102) : BgHover;
        button.FlatAppearance.MouseDownBackColor = primary ? Color.FromArgb(238, 176, 44) : BgPressed;
        button.Click += click;
        return button;
    }

    private Button BuildGhostButton(string text, EventHandler click)
        => BuildActionButton(text, click, false);

    private CheckBox BuildCheck(string text, bool isChecked, EventHandler click)
    {
        var check = new CheckBox
        {
            Text = text,
            Checked = isChecked,
            AutoSize = true,
            ForeColor = TextPrimary,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 4, 0, 2)
        };
        check.CheckedChanged += click;
        return check;
    }

    private Control BuildInfoPill(Label label)
    {
        var panel = BuildCardPanel();
        panel.Width = 312;
        panel.Padding = new Padding(12, 10, 12, 10);
        panel.Margin = new Padding(0, 8, 0, 0);
        label.Location = new Point(12, 10);
        label.AutoSize = true;
        label.BackColor = Color.Transparent;
        panel.Controls.Add(label);
        return panel;
    }

    private Panel BuildCardPanel()
    {
        return new Panel
        {
            BackColor = BgPanel,
            ForeColor = TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 0, 10)
        };
    }

    private Control MakeSection(string title, Control body)
    {
        var shell = BuildCardPanel();
        shell.Dock = DockStyle.Top;
        shell.AutoSize = true;
        shell.Padding = new Padding(0);
        shell.Margin = new Padding(0, 0, 0, 10);

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 36,
            BackColor = BgPanelHeader,
            Padding = new Padding(12, 9, 12, 0)
        };
        var titleLabel = new Label
        {
            Text = title,
            AutoSize = true,
            ForeColor = TextPrimary,
            Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold)
        };
        header.Controls.Add(titleLabel);

        body.Dock = DockStyle.Top;
        body.Padding = new Padding(12, 12, 12, 12);
        shell.Controls.Add(body);
        shell.Controls.Add(header);
        return shell;
    }

    private Label MakeLabel(string text)
        => new() { AutoSize = true, Text = text, Margin = new Padding(0, 5, 0, 3), ForeColor = TextSecondary };

    private void ApplyTheme(Control root)
    {
        foreach (Control control in root.Controls)
        {
            switch (control)
            {
                case Panel panel:
                    panel.ForeColor = TextPrimary;
                    break;
                case Label label:
                    if (label != _overviewHeroLabel)
                    {
                        label.ForeColor = label.ForeColor == Color.Empty ? TextPrimary : label.ForeColor;
                    }
                    break;
                case TextBox textBox:
                    textBox.BackColor = BgCard;
                    textBox.ForeColor = TextPrimary;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case ComboBox comboBox:
                    comboBox.BackColor = BgCard;
                    comboBox.ForeColor = TextPrimary;
                    comboBox.FlatStyle = FlatStyle.Flat;
                    break;
                case NumericUpDown numeric:
                    numeric.BackColor = BgCard;
                    numeric.ForeColor = TextPrimary;
                    numeric.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case ListBox list:
                    list.BackColor = BgCard;
                    list.ForeColor = TextPrimary;
                    break;
                case TabControl tabControl:
                    tabControl.BackColor = BgApp;
                    tabControl.ForeColor = TextPrimary;
                    break;
                case Button button:
                    button.FlatAppearance.MouseOverBackColor = BgHover;
                    button.FlatAppearance.MouseDownBackColor = BgPressed;
                    break;
            }

            ApplyTheme(control);
        }
    }

    private void DrawTab(object? sender, DrawItemEventArgs e)
    {
        if (sender is not TabControl tabs) return;
        var tab = tabs.TabPages[e.Index];
        var rect = e.Bounds;
        var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        using var back = new SolidBrush(isSelected ? BgCard : BgPanel);
        using var accentBrush = new SolidBrush(isSelected ? AccentSoft : Color.FromArgb(0, 0, 0, 0));
        using var textBrush = new SolidBrush(isSelected ? TextPrimary : TextSecondary);
        using var borderPen = new Pen(isSelected ? Accent : Border, isSelected ? 1.8f : 1f);
        e.Graphics.FillRectangle(back, rect);
        if (isSelected)
        {
            e.Graphics.FillRectangle(accentBrush, new Rectangle(rect.X + 1, rect.Bottom - 4, rect.Width - 2, 3));
        }
        e.Graphics.DrawRectangle(borderPen, Rectangle.Inflate(rect, -1, -1));
        TextRenderer.DrawText(e.Graphics, tab.Text, Font, rect, textBrush.Color, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private void DrawItemEntry(object? sender, DrawItemEventArgs e)
    {
        e.DrawBackground();
        if (e.Index < 0 || e.Index >= _itemList.Items.Count)
        {
            return;
        }

        if (_itemList.Items[e.Index] is not ItemListEntry entry || !Catalog.ById.TryGetValue(entry.Item.DefinitionId, out var definition))
        {
            return;
        }

        var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        var bg = selected ? Color.FromArgb(56, 66, 82) : BgCard;
        using var bgBrush = new SolidBrush(bg);
        using var swatchBrush = new SolidBrush(definition.DisplayColor);
        using var textBrush = new SolidBrush(TextPrimary);
        using var mutedBrush = new SolidBrush(TextSecondary);
        using var borderPen = new Pen(selected ? Accent : Border, selected ? 1.4f : 1f);

        var rect = Rectangle.Inflate(e.Bounds, -1, -1);
        e.Graphics.FillRectangle(bgBrush, rect);
        e.Graphics.DrawRectangle(borderPen, rect);

        var swatchRect = new Rectangle(rect.X + 8, rect.Y + 6, 12, 12);
        e.Graphics.FillRectangle(swatchBrush, swatchRect);
        e.Graphics.DrawRectangle(Pens.Black, swatchRect);

        var titleRect = new Rectangle(rect.X + 28, rect.Y + 4, rect.Width - 96, 14);
        var metaRect = new Rectangle(rect.X + 28, rect.Y + 15, rect.Width - 96, 10);
        using var titleFont = new Font("Segoe UI Semibold", 8.6f, FontStyle.Bold);
        using var metaFont = new Font("Segoe UI", 7.7f);
        TextRenderer.DrawText(e.Graphics, definition.Name, titleFont, titleRect, textBrush.Color, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        TextRenderer.DrawText(e.Graphics, $"({entry.Item.X},{entry.Item.Y}) • rot {entry.Item.Rotation}° • {definition.Layer}", metaFont, metaRect, mutedBrush.Color, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

        if (entry.Locked)
        {
            var badge = new Rectangle(rect.Right - 50, rect.Y + 5, 42, 15);
            using var badgeBrush = new SolidBrush(Color.FromArgb(62, 255, 214, 84));
            using var badgePen = new Pen(Accent, 1f);
            e.Graphics.FillRectangle(badgeBrush, badge);
            e.Graphics.DrawRectangle(badgePen, badge);
            using (var badgeFont = new Font("Segoe UI", 7.2f, FontStyle.Bold)) TextRenderer.DrawText(e.Graphics, "LOCK", badgeFont, badge, Color.FromArgb(255, 235, 180), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        e.DrawFocusRectangle();
    }

    private void WireEvents()
    {
        _canvas.ProjectChanged += (_, _) =>
        {
            MarkDirty();
            RefreshProjectUi();
        };
        _canvas.SelectionChanged += (_, _) => RefreshSelectionUi();
        _canvas.HistoryChanged += (_, _) => RefreshHistoryUi();
        _canvas.BlueprintChanged += (_, _) => RefreshBlueprintUi();
        _canvas.StatusMessage += (_, message) => _statusLabel.Text = message;

        _inspectorNoteText.Leave += (_, _) => ApplyInspectorChanges();
        _inspectorRotationInput.ValueChanged += (_, _) =>
        {
            if (!_suppressUiEvents && _canvas.SelectedItems.Count == 1)
            {
                ApplyInspectorChanges();
            }
        };

        _projectNameText.TextChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.Name = string.IsNullOrWhiteSpace(_projectNameText.Text) ? "Untitled CAMP" : _projectNameText.Text.Trim();
            _canvas.Invalidate();
            RefreshProjectUi();
        };

        _modeCombo.SelectedIndexChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            if (_modeCombo.SelectedItem is BuildMode mode)
            {
                _canvas.Project.Mode = mode;
                RefreshProjectUi();
                _canvas.Invalidate();
            }
        };

        _ruleCombo.SelectedIndexChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            if (_ruleCombo.SelectedItem is RuleProfile profile)
            {
                _canvas.Project.RuleProfile = profile;
                RefreshProjectUi();
                _canvas.Invalidate();
            }
        };

        _budgetLimitInput.ValueChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.BudgetLimit = (int)_budgetLimitInput.Value;
            RefreshProjectUi();
        };

        _storedBudgetInput.ValueChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.StoredBudget = (int)_storedBudgetInput.Value;
            RefreshProjectUi();
        };

        _budgetProfileCombo.SelectedIndexChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            if (_budgetProfileCombo.SelectedItem is BudgetPlaystyleProfile profile)
            {
                ApplyBudgetProfile(profile);
            }
        };

        _overlayPresetCombo.SelectedIndexChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            if (_overlayPresetCombo.SelectedItem is OverlayReviewPreset preset)
            {
                ApplyOverlayPreset(preset);
            }
        };

        _gridWidthInput.ValueChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.GridWidth = (int)_gridWidthInput.Value;
            _canvas.Invalidate();
            RefreshProjectUi();
        };

        _gridHeightInput.ValueChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.Project.GridHeight = (int)_gridHeightInput.Value;
            _canvas.Invalidate();
            RefreshProjectUi();
        };

        _zoomInput.ValueChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            _canvas.ZoomPercent = (int)_zoomInput.Value;
        };

        _presetCombo.SelectedIndexChanged += (_, _) => RefreshPresetDescription();
        _itemFilterText.TextChanged += (_, _) => RefreshItemList();
        _itemLayerFilterCombo.SelectedIndexChanged += (_, _) => RefreshItemList();

        _itemList.SelectedIndexChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            var ids = _itemList.SelectedItems.OfType<ItemListEntry>().Select(x => x.Item.Id).ToList();
            _canvas.SelectItems(ids);
            if (ids.Count > 0)
            {
                _statusLabel.Text = $"Selected {ids.Count} item(s) from list.";
            }
        };
    }

    private void SetActiveTool(ToolType tool)
    {
        _canvas.CurrentTool = tool;
        foreach (var (key, button) in _toolButtons)
        {
            button.Checked = key == tool;
            button.BackColor = key == tool ? Color.FromArgb(66, 74, 90) : BgCard;
        }

        _statusLabel.Text = $"Active tool: {tool}";
        RefreshProjectUi();
    }

    private void NewProject()
    {
        _currentPath = null;
        _isDirty = false;
        _canvas.Project = PresetLibrary.GetById("custom").CreateProject();
        _canvas.Project.Name = "Knoksen Foundation Draft";
        SetActiveTool(ToolType.Foundation);
        _zoomInput.Value = 100;
        RefreshProjectUi();
        RefreshPresetDescription();
        RefreshBlueprintUi();
        _statusLabel.Text = "New project created.";
        UpdateWindowTitle();
    }

    private void ApplySelectedPreset()
    {
        if (_presetCombo.SelectedItem is not ProjectPreset preset)
        {
            return;
        }

        _currentPath = null;
        _isDirty = false;
        _canvas.Project = preset.CreateProject();
        SetActiveTool(ToolType.Foundation);
        RefreshProjectUi();
        RefreshBlueprintUi();
        _statusLabel.Text = $"Applied preset: {preset.Name}.";
        UpdateWindowTitle();
    }

    private void RefreshProjectUi()
    {
        _suppressUiEvents = true;
        try
        {
            var project = NormalizeProject(_canvas.Project);

            if (_projectNameText.Text != project.Name)
            {
                _projectNameText.Text = project.Name;
            }

            _modeCombo.SelectedItem = project.Mode;
            _ruleCombo.SelectedItem = project.RuleProfile;
            _snapEnabledCheck.Checked = project.SnapEnabled;
            _campSlotCombo.SelectedItem = project.ActiveCampSlot;
            _budgetProfileCombo.SelectedItem = project.BudgetProfile;
            _overlayPresetCombo.SelectedItem = project.OverlayPreset;
        _budgetLimitInput.Value = ClampDecimal(project.BudgetLimit, _budgetLimitInput.Minimum, _budgetLimitInput.Maximum);
            _storedBudgetInput.Value = ClampDecimal(project.StoredBudget, _storedBudgetInput.Minimum, _storedBudgetInput.Maximum);
            _gridWidthInput.Value = ClampDecimal(project.GridWidth, _gridWidthInput.Minimum, _gridWidthInput.Maximum);
            _gridHeightInput.Value = ClampDecimal(project.GridHeight, _gridHeightInput.Minimum, _gridHeightInput.Maximum);
            _zoomInput.Value = ClampDecimal(_canvas.ZoomPercent, _zoomInput.Minimum, _zoomInput.Maximum);

            var selectedPreset = PresetLibrary.GetById(project.PresetId);
            _presetCombo.SelectedItem = PresetLibrary.All.FirstOrDefault(x => x.Id == selectedPreset.Id);
            _presetDescriptionLabel.Text = selectedPreset.Description;

            foreach (var (layer, check) in _layerChecks)
            {
                check.Checked = project.LayerVisibility.IsVisible(layer);
            }

            foreach (var (layer, lockCheck) in _layerLockChecks)
            {
                lockCheck.Checked = project.LayerLocks.IsLocked(layer);
            }

            if (_showCampRadiusCheck is not null)
            {
                _showCampRadiusCheck.Checked = project.ShowCampRadiusOverlay;
            }

            if (_showTurretCoverageCheck is not null)
            {
                _showTurretCoverageCheck.Checked = project.ShowTurretCoverage;
            }

            if (_showVisitorFlowCheck is not null)
            {
                _showVisitorFlowCheck.Checked = project.ShowVisitorFlowOverlay;
            }

            if (_showTrapZonesCheck is not null)
            {
                _showTrapZonesCheck.Checked = project.ShowTrapZonesOverlay;
            }

            var placedBudget = project.Items.Sum(x => Catalog.ById.TryGetValue(x.DefinitionId, out var def) ? def.BudgetCost : 0);
            var totalBudget = placedBudget + project.StoredBudget;
            _budgetProgress.Maximum = Math.Max(1, project.BudgetLimit);
            _budgetProgress.Value = Math.Max(0, Math.Min(_budgetProgress.Maximum, totalBudget));
            _budgetLabel.Text = $"Placed {placedBudget} • Stored {project.StoredBudget} • Total {totalBudget} / {project.BudgetLimit}";
            _modeLabel.Text = $"Mode: {project.Mode}   •   Rules: {project.RuleProfile}   •   Tool: {_canvas.CurrentTool}\nTip: Use Workflow + Focus to keep each build pass clear.";

            if (BudgetProfileLibrary.Profiles.TryGetValue(project.BudgetProfile, out var profilePreset))
            {
                _budgetProfileLabel.Text = $"{project.BudgetProfile}: {profilePreset.Description}";
            }

            _overviewHeroLabel.Text = project.Name;
            _overviewMetaLabel.Text = $"{selectedPreset.Name} • {project.Mode} • Grid {project.GridWidth}x{project.GridHeight} • Zoom {_canvas.ZoomPercent}%";
            _statusCardLabel.Text = $"Budget {Math.Min(100, (int)Math.Round((double)totalBudget / Math.Max(1, project.BudgetLimit) * 100))}% • {_canvas.CurrentTool} active • {_canvas.SelectedItems.Count} selected";

            RefreshItemList();
            RefreshSelectionUi();
            RefreshHistoryUi();
            RefreshBlueprintUi();
            RefreshBlueprintLibraryUi();
            RefreshRoutePlanningUi();
            RefreshWorkflowUi();
            RefreshFocusUi();
            RefreshQuickStartUi();
            RefreshAnalysisUi();
            RefreshMinimapUi();
            UpdateWindowTitle();
        }
        finally
        {
            _suppressUiEvents = false;
        }
    }

    private void RefreshItemList()
    {
        var project = _canvas.Project;
        var selectedIds = _canvas.SelectedItems.Select(x => x.Id).ToHashSet();
        var query = (_itemFilterText.Text ?? string.Empty).Trim();
        var selectedLayer = _itemLayerFilterCombo.SelectedItem;

        IEnumerable<PlacedItem> filtered = project.Items;
        if (!string.IsNullOrWhiteSpace(query))
        {
            filtered = filtered.Where(item =>
            {
                Catalog.ById.TryGetValue(item.DefinitionId, out var def);
                var hay = $"{def?.Name} {item.Note} {item.X} {item.Y}";
                return hay.Contains(query, StringComparison.OrdinalIgnoreCase);
            });
        }

        if (selectedLayer is LayerType layerFilter)
        {
            filtered = filtered.Where(item => Catalog.ById.TryGetValue(item.DefinitionId, out var def) && def.Layer == layerFilter);
        }

        _itemList.BeginUpdate();
        _itemList.Items.Clear();
        foreach (var item in filtered)
        {
            if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition))
            {
                continue;
            }

            _itemList.Items.Add(new ItemListEntry(item, definition.Name, project.LayerLocks.IsLocked(definition.Layer)));
        }

        for (var i = 0; i < _itemList.Items.Count; i++)
        {
            if (_itemList.Items[i] is ItemListEntry entry && selectedIds.Contains(entry.Item.Id))
            {
                _itemList.SetSelected(i, true);
            }
        }
        _itemList.EndUpdate();
    }


    private void ActivateWorkflowStage(string stage)
    {
        switch (stage)
        {
            case "Layout":
                ApplyFocusPreset("Structure");
                SetActiveTool(ToolType.Foundation);
                break;
            case "Envelope":
                ApplyFocusPreset("Structure");
                SetActiveTool(ToolType.Wall);
                break;
            case "Systems":
                ApplyFocusPreset("Systems");
                SetActiveTool(ToolType.Power);
                break;
            case "Defense":
                ApplyFocusPreset("Defense");
                SetActiveTool(ToolType.Turret);
                break;
            case "Polish":
                ApplyFocusPreset("Presentation");
                SetActiveTool(ToolType.Decor);
                break;
            default:
                ApplyFocusPreset("All");
                break;
        }

        _statusLabel.Text = $"Workflow stage: {stage}";
        RefreshProjectUi();
    }

    private void ApplyFocusPreset(string preset)
    {
        var project = _canvas.Project;
        switch (preset)
        {
            case "Structure":
                SetLayerVisibilityPreset(structure: true, utility: false, defense: false, power: false, aesthetic: false, commerce: false);
                project.ShowCampRadiusOverlay = true;
                project.ShowTurretCoverage = false;
                break;
            case "Systems":
                SetLayerVisibilityPreset(structure: true, utility: true, defense: false, power: true, aesthetic: false, commerce: false);
                project.ShowCampRadiusOverlay = true;
                project.ShowTurretCoverage = false;
                break;
            case "Defense":
                SetLayerVisibilityPreset(structure: true, utility: false, defense: true, power: true, aesthetic: false, commerce: false);
                project.ShowCampRadiusOverlay = true;
                project.ShowTurretCoverage = true;
                break;
            case "Presentation":
                SetLayerVisibilityPreset(structure: true, utility: false, defense: false, power: false, aesthetic: true, commerce: true);
                project.ShowCampRadiusOverlay = false;
                project.ShowTurretCoverage = false;
                break;
            default:
                SetLayerVisibilityPreset(structure: true, utility: true, defense: true, power: true, aesthetic: true, commerce: true);
                project.ShowCampRadiusOverlay = true;
                project.ShowTurretCoverage = true;
                break;
        }

        _canvas.Invalidate();
        RefreshProjectUi();
        _statusLabel.Text = $"Focus preset: {preset}";
    }

    private void SetLayerVisibilityPreset(bool structure, bool utility, bool defense, bool power, bool aesthetic, bool commerce)
    {
        _canvas.Project.LayerVisibility.Structure = structure;
        _canvas.Project.LayerVisibility.Utility = utility;
        _canvas.Project.LayerVisibility.Defense = defense;
        _canvas.Project.LayerVisibility.Power = power;
        _canvas.Project.LayerVisibility.Aesthetic = aesthetic;
        _canvas.Project.LayerVisibility.Commerce = commerce;
    }

    private string GetCurrentWorkflowStage()
        => _canvas.CurrentTool switch
        {
            ToolType.Foundation => "Layout",
            ToolType.Wall or ToolType.Door or ToolType.Stairs or ToolType.Roof => "Envelope",
            ToolType.Workbench or ToolType.Power or ToolType.Light => "Systems",
            ToolType.Turret => "Defense",
            ToolType.Decor or ToolType.Vendor or ToolType.Resource or ToolType.Display or ToolType.Ally => "Polish",
            _ => "Custom"
        };

    private void RefreshWorkflowUi()
    {
        var stage = GetCurrentWorkflowStage();
        _workflowLabel.Text = stage switch
        {
            "Layout" => "Stage 1 – Layout\n1) Place 6–12 foundations.\n2) Set CAMP center from your footprint.\n3) Keep walk lanes open for visitor flow.",
            "Envelope" => "Stage 2 – Envelope\n1) Add walls/doors on foundation edges.\n2) Place stairs for vertical routes.\n3) Add roof support before systems.",
            "Systems" => "Stage 3 – Systems\n1) Add workbench and core utilities.\n2) Place power and lights.\n3) Verify interaction paths stay clear.",
            "Defense" => "Stage 4 – Defense\n1) Add perimeter turrets.\n2) Check arc overlap and blind spots.\n3) Protect main approach lanes.",
            "Polish" => "Stage 5 – Polish / Presentation\n1) Add decor/vendor/readability cues.\n2) Remove clutter from key routes.\n3) Final pass with focus presets.",
            _ => "Custom stage\nFreeform editing is active. Use workflow buttons for guided step-by-step building."
        };

        foreach (var (key, button) in _workflowButtons)
        {
            var active = key == stage;
            button.BackColor = active ? Accent : BgCard;
            button.ForeColor = active ? Color.FromArgb(35, 26, 0) : TextPrimary;
            button.FlatAppearance.BorderColor = active ? Accent : Border;
        }
    }
    private void RefreshFocusUi()
    {
        var visible = Enum.GetValues<LayerType>()
            .Where(layer => _canvas.Project.LayerVisibility.IsVisible(layer))
            .Select(layer => layer.ToString())
            .ToList();
        _focusLabel.Text = $"Visible focus: {string.Join(", ", visible)}\nOverlays: radius {(_canvas.Project.ShowCampRadiusOverlay ? "On" : "Off")}, turret {(_canvas.Project.ShowTurretCoverage ? "On" : "Off")}";
    }


    private void ApplyInspectorChanges()
    {
        if (_suppressUiEvents || _canvas.SelectedItems.Count != 1)
        {
            return;
        }

        var note = string.IsNullOrWhiteSpace(_inspectorNoteText.Text) ? null : _inspectorNoteText.Text.Trim();
        _canvas.UpdateSingleSelectedItem(
            note,
            (int)_inspectorXInput.Value,
            (int)_inspectorYInput.Value,
            (int)_inspectorRotationInput.Value);
    }

    private void RefreshQuickStartUi()
    {
        var project = _canvas.Project;
        var definitions = project.Items
            .Select(x => Catalog.ById.TryGetValue(x.DefinitionId, out var def) ? def : null)
            .Where(x => x is not null)
            .Cast<ItemDefinition>()
            .ToList();

        var structureCount = definitions.Count(x => x.Layer == LayerType.Structure);
        var foundationCount = definitions.Count(x => x.Id == "foundation");
        var systemCount = definitions.Count(x => x.Layer is LayerType.Utility or LayerType.Power);
        var defenseCount = definitions.Count(x => x.Layer == LayerType.Defense);
        var polishCount = definitions.Count(x => x.Layer is LayerType.Aesthetic or LayerType.Commerce);

        string recommendation;
        if (foundationCount == 0)
        {
            recommendation = "Next best move: place your first 6–12 foundations. Then set CAMP center so radius and visitor flow are easy to read.";
        }
        else if (structureCount <= foundationCount)
        {
            recommendation = "Next best move: switch to Envelope and lock in the shell with walls, doors, stairs and roof anchors.";
        }
        else if (systemCount == 0)
        {
            recommendation = "Next best move: switch to Systems and add workbench, power and lights after the shell is stable.";
        }
        else if (defenseCount == 0 && project.Mode == BuildMode.SurfaceCamp)
        {
            recommendation = "Next best move: validate perimeter logic. Add turrets, check arc overlap, place ingress/egress markers, and protect approach lanes.";
        }
        else
        {
            recommendation = "Next best move: polish readability. Use focus presets, run a visitor-flow pass with checkpoints, and clean up clutter.";
        }

        _quickStartLabel.Text =
            $"Workflow pulse\n" +
            $"Foundations: {foundationCount} • Structure: {structureCount} • Systems: {systemCount} • Defense: {defenseCount} • Polish: {polishCount}\n" +
            recommendation + "\n\n" +
            "First-run steps\n" +
            "1) Foundations first (Layout)\n" +
            "2) Build shell (Envelope)\n" +
            "3) Utilities and power (Systems)\n" +
            "4) Turret coverage (Defense)\n" +
                "5) Visitor flow + trap zones + blueprint modules + polish\n\n" +
            "Fast actions: Duplicate for module iteration, Set CAMP center for cleaner radius planning, use focus presets to reduce noise, and save/load blueprints for repeatable modules.";
    }

    private void RefreshMinimapUi()
    {
        _minimap.Project = _canvas.Project;
        _minimap.SelectedItemIds = _canvas.SelectedItems.Select(x => x.Id).ToHashSet();
        _minimap.HoverGrid = _canvas.HoverGridPoint;
        _minimap.Invalidate();
    }

    private void RefreshSelectionUi()
    {
        var selected = _canvas.SelectedItems;
        _suppressUiEvents = true;
        try
        {
            if (selected.Count == 0)
            {
                _selectionLabel.Text = "No selection.\nUse Select and click an item, or drag a marquee box.\nNew users: start with foundations, then switch stages from Workflow.";
                _inspectorNoteText.Text = string.Empty;
                _inspectorXInput.Value = 0;
                _inspectorYInput.Value = 0;
                _inspectorRotationInput.Value = 0;
                _inspectorNoteText.Enabled = false;
                _inspectorXInput.Enabled = false;
                _inspectorYInput.Enabled = false;
                _inspectorRotationInput.Enabled = false;
                _inspectorHintLabel.Text = "Inspector idle: select exactly one item to edit note, grid position, and rotation.";
                RefreshMinimapUi();
                return;
            }

            if (selected.Count == 1)
            {
                var item = selected[0];
                if (!Catalog.ById.TryGetValue(item.DefinitionId, out var definition))
                {
                    _selectionLabel.Text = "Unknown selection.";
                    _inspectorHintLabel.Text = "Unknown selection type.";
                    RefreshMinimapUi();
                    return;
                }

                _selectionLabel.Text = $"{definition.Name}\nGrid: ({item.X}, {item.Y})\nRotation: {item.Rotation}°\nLayer: {definition.Layer}\nBudget: {definition.BudgetCost}\nShelter allowed: {(definition.AllowedInShelter ? "Yes" : "No")}";
                _inspectorNoteText.Enabled = true;
                _inspectorXInput.Enabled = true;
                _inspectorYInput.Enabled = true;
                _inspectorRotationInput.Enabled = true;
                _inspectorNoteText.Text = item.Note ?? string.Empty;
                _inspectorXInput.Maximum = Math.Max(1000, _canvas.Project.GridWidth);
                _inspectorYInput.Maximum = Math.Max(1000, _canvas.Project.GridHeight);
                _inspectorXInput.Value = Math.Max(_inspectorXInput.Minimum, Math.Min(_inspectorXInput.Maximum, item.X));
                _inspectorYInput.Value = Math.Max(_inspectorYInput.Minimum, Math.Min(_inspectorYInput.Maximum, item.Y));
                _inspectorRotationInput.Value = item.Rotation;
                _inspectorHintLabel.Text = "Inspector active for this item. Update fields, then click Apply inspector changes.";
                RefreshMinimapUi();
                return;
            }

            var totalBudget = selected.Sum(x => Catalog.ById.TryGetValue(x.DefinitionId, out var def) ? def.BudgetCost : 0);
            var layers = selected
                .Select(x => Catalog.ById.TryGetValue(x.DefinitionId, out var def) ? def.Layer.ToString() : "Unknown")
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            _selectionLabel.Text = $"Multi-selection\nItems: {selected.Count}\nCombined budget: {totalBudget}\nLayers: {string.Join(", ", layers)}\nDrag to move as one module\nR rotates whole group";
            _inspectorNoteText.Text = string.Empty;
            _inspectorXInput.Value = 0;
            _inspectorYInput.Value = 0;
            _inspectorRotationInput.Value = 0;
            _inspectorNoteText.Enabled = false;
            _inspectorXInput.Enabled = false;
            _inspectorYInput.Enabled = false;
            _inspectorRotationInput.Enabled = false;
            _inspectorHintLabel.Text = "Inspector locked for multi-select. Use Quick Actions (rotate/duplicate/delete) for grouped edits.";
            RefreshMinimapUi();
        }
        finally
        {
            _suppressUiEvents = false;
        }
    }

    private void RefreshHistoryUi()
    {
        if (_undoButton is not null)
        {
            _undoButton.Enabled = _canvas.CanUndo;
        }

        if (_redoButton is not null)
        {
            _redoButton.Enabled = _canvas.CanRedo;
        }

        _historyLabel.Text = $"History: {(_canvas.CanUndo ? "Undo ready" : "Undo empty")} • {(_canvas.CanRedo ? "Redo ready" : "Redo empty")}";
    }

    private void RefreshAnalysisUi()
    {
        var centerX = _canvas.Project.CampCenterX >= 0 ? _canvas.Project.CampCenterX : _canvas.Project.GridWidth / 2;
        var centerY = _canvas.Project.CampCenterY >= 0 ? _canvas.Project.CampCenterY : _canvas.Project.GridHeight / 2;
        var lockedCount = Enum.GetValues<LayerType>().Count(layer => _canvas.Project.LayerLocks.IsLocked(layer));
        var visibleCount = Enum.GetValues<LayerType>().Count(layer => _canvas.Project.LayerVisibility.IsVisible(layer));
        _analysisLabel.Text =
            $"Approx center: ({centerX}, {centerY})\n" +
            $"Surface radius: {(_canvas.Project.ShowCampRadiusOverlay ? "On" : "Off")}\n" +
            $"Turret arcs: {(_canvas.Project.ShowTurretCoverage ? "On" : "Off")}\n" +
            $"Visitor flow: {(_canvas.Project.ShowVisitorFlowOverlay ? "On" : "Off")}\n" +
            $"Trap zones: {(_canvas.Project.ShowTrapZonesOverlay ? "On" : "Off")}\n" +
            $"Overlay preset: {_canvas.Project.OverlayPreset}\n" +
            $"Visitor markers: {_canvas.Project.VisitorMarkers.Count}\n" +
                $"Structured trap zones: {_canvas.Project.TrapZones.Count}\n" +
            $"Visible layers: {visibleCount}/6\n" +
            $"Locked layers: {lockedCount}";
    }

    private void RefreshBlueprintUi()
    {
        var blueprint = _canvas.LoadedBlueprint;
        _blueprintLabel.Text = blueprint is null
            ? "No blueprint loaded.\nQuick start: place a small foundation module, select it, save as blueprint, then load/paste to iterate faster."
            : $"Loaded: {blueprint.Name}\nItems: {blueprint.Items.Count}\nRecommended mode: {(blueprint.RecommendedMode?.ToString() ?? "Any")}\n{blueprint.Description}\nTip: Paste near your current selection to expand in modular steps.";
    }

    private void RefreshBlueprintLibraryUi()
    {
        var slot = _canvas.Project.ActiveCampSlot;
        var entries = _canvas.Project.BlueprintLibrary
            .Where(e => e.Slot == slot)
            .ToList();

        _blueprintLibraryList.BeginUpdate();
        _blueprintLibraryList.Items.Clear();
        foreach (var entry in entries)
        {
            _blueprintLibraryList.Items.Add(entry);
        }
        _blueprintLibraryList.EndUpdate();
    }

    private void RefreshRoutePlanningUi()
    {
        var markerId = (_visitorMarkerList.SelectedItem as VisitorMarker)?.Id;
        var trapZoneId = (_trapZoneList.SelectedItem as TrapZonePlan)?.Id;

        var orderedMarkers = _canvas.Project.VisitorMarkers
            .OrderBy(marker => marker.Order)
            .ThenBy(marker => marker.Id)
            .ToList();
        var trapZones = _canvas.Project.TrapZones
            .OrderByDescending(zone => zone.Severity)
            .ThenBy(zone => zone.Label)
            .ToList();

        _visitorMarkerList.BeginUpdate();
        _visitorMarkerList.Items.Clear();
        foreach (var marker in orderedMarkers)
        {
            _visitorMarkerList.Items.Add(marker);
        }
        _visitorMarkerList.EndUpdate();

        _trapZoneList.BeginUpdate();
        _trapZoneList.Items.Clear();
        foreach (var trapZone in trapZones)
        {
            _trapZoneList.Items.Add(trapZone);
        }
        _trapZoneList.EndUpdate();

        RestoreListSelection(_visitorMarkerList, markerId);
        RestoreListSelection(_trapZoneList, trapZoneId);

        if (_visitorMarkerList.SelectedIndex < 0 && _visitorMarkerList.Items.Count > 0)
        {
            _visitorMarkerList.SelectedIndex = 0;
        }

        if (_trapZoneList.SelectedIndex < 0 && _trapZoneList.Items.Count > 0)
        {
            _trapZoneList.SelectedIndex = 0;
        }

        if (_visitorMarkerList.SelectedIndex < 0)
        {
            _markerLabelText.Text = string.Empty;
            _markerTypeCombo.SelectedItem = VisitorMarkerType.Checkpoint;
        }

        if (_trapZoneList.SelectedIndex < 0)
        {
            _trapZoneLabelText.Text = string.Empty;
            _trapZoneSeverityCombo.SelectedItem = TrapZoneSeverity.Medium;
            _trapZoneNotesText.Text = string.Empty;
        }

        if (_defenseReviewNotesText.Text != _canvas.Project.DefenseReviewNotes)
        {
            _defenseReviewNotesText.Text = _canvas.Project.DefenseReviewNotes;
        }

        _routePlanningLabel.Text =
            $"Overlay preset: {_canvas.Project.OverlayPreset}\n" +
            $"Ordered route steps: {orderedMarkers.Count}\n" +
            $"Trap zones: {trapZones.Count}\n" +
            $"Review notes: {(string.IsNullOrWhiteSpace(_canvas.Project.DefenseReviewNotes) ? "Not saved" : "Saved")}\n" +
            "Add markers from hover/selection, reorder them here, and keep zone severity plus review notes in the project file.";
    }

    private void PopulateVisitorMarkerEditor()
    {
        if (_visitorMarkerList.SelectedItem is not VisitorMarker marker)
        {
            return;
        }

        if (_markerLabelText.Text != marker.Label)
        {
            _markerLabelText.Text = marker.Label;
        }

        _markerTypeCombo.SelectedItem = marker.Type;
    }

    private void PopulateTrapZoneEditor()
    {
        if (_trapZoneList.SelectedItem is not TrapZonePlan trapZone)
        {
            return;
        }

        if (_trapZoneLabelText.Text != trapZone.Label)
        {
            _trapZoneLabelText.Text = trapZone.Label;
        }

        if (_trapZoneNotesText.Text != trapZone.Notes)
        {
            _trapZoneNotesText.Text = trapZone.Notes;
        }

        _trapZoneSeverityCombo.SelectedItem = trapZone.Severity;
    }

    private void ApplySelectedVisitorMarker()
    {
        if (_visitorMarkerList.SelectedItem is not VisitorMarker marker)
        {
            MessageBox.Show(this, "Select a route marker first.", "Route editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_markerTypeCombo.SelectedItem is not VisitorMarkerType markerType)
        {
            markerType = marker.Type;
        }

        _canvas.UpdateVisitorMarker(marker.Id, markerType, _markerLabelText.Text);
    }

    private void MoveSelectedVisitorMarker(int direction)
    {
        if (_visitorMarkerList.SelectedItem is not VisitorMarker marker)
        {
            MessageBox.Show(this, "Select a route marker first.", "Route editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _canvas.ReorderVisitorMarker(marker.Id, direction);
    }

    private void RemoveSelectedVisitorMarker()
    {
        if (_visitorMarkerList.SelectedItem is not VisitorMarker marker)
        {
            MessageBox.Show(this, "Select a route marker first.", "Route editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _canvas.RemoveVisitorMarker(marker.Id);
    }

    private void CreateTrapZoneFromSelection(string label, TrapZoneSeverity severity)
    {
        _trapZoneLabelText.Text = label;
        _trapZoneSeverityCombo.SelectedItem = severity;
        _canvas.AddTrapZoneFromSelection(label, severity, _trapZoneNotesText.Text);
    }

    private void ApplySelectedTrapZone()
    {
        if (_trapZoneList.SelectedItem is not TrapZonePlan trapZone)
        {
            _canvas.AddTrapZoneFromSelection(
                _trapZoneLabelText.Text,
                _trapZoneSeverityCombo.SelectedItem is TrapZoneSeverity severity ? severity : TrapZoneSeverity.Medium,
                _trapZoneNotesText.Text);
            return;
        }

        _canvas.UpdateTrapZone(
            trapZone.Id,
            _trapZoneLabelText.Text,
            _trapZoneSeverityCombo.SelectedItem is TrapZoneSeverity selectedSeverity ? selectedSeverity : trapZone.Severity,
            _trapZoneNotesText.Text);
    }

    private void RemoveSelectedTrapZone()
    {
        if (_trapZoneList.SelectedItem is not TrapZonePlan trapZone)
        {
            MessageBox.Show(this, "Select a trap zone first.", "Trap editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _canvas.RemoveTrapZone(trapZone.Id);
    }

    private void ApplyDefenseReviewNotes()
    {
        _canvas.Project.DefenseReviewNotes = (_defenseReviewNotesText.Text ?? string.Empty).Trim();
        MarkDirty();
        RefreshProjectUi();
        _statusLabel.Text = "Defense review notes saved into the project.";
    }

    private static void RestoreListSelection(ListBox listBox, Guid? id)
    {
        if (id is null)
        {
            return;
        }

        for (var i = 0; i < listBox.Items.Count; i++)
        {
            switch (listBox.Items[i])
            {
                case VisitorMarker marker when marker.Id == id.Value:
                    listBox.SelectedIndex = i;
                    return;
                case TrapZonePlan trapZone when trapZone.Id == id.Value:
                    listBox.SelectedIndex = i;
                    return;
            }
        }
    }

    private void AddLoadedBlueprintToLibrary()
    {
        var loaded = _canvas.LoadedBlueprint;
        if (loaded is null)
        {
            MessageBox.Show(this, "No blueprint is currently loaded. Load a blueprint first.", "Add to library", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var entry = new BlueprintLibraryEntry
        {
            Slot = _canvas.Project.ActiveCampSlot,
            Name = loaded.Name,
            Description = loaded.Description,
            FilePath = "(in-memory)",
            Module = CloneBlueprintModule(loaded)
        };

        _canvas.Project.BlueprintLibrary.Add(entry);
        RefreshBlueprintLibraryUi();
        _statusLabel.Text = $"Added '{loaded.Name}' to {_canvas.Project.ActiveCampSlot} library.";
    }

    private void LoadBlueprintFromLibrary()
    {
        if (_blueprintLibraryList.SelectedItem is not BlueprintLibraryEntry entry)
        {
            MessageBox.Show(this, "Select a blueprint from the slot library first.", "Load from library", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (entry.Module is not null)
        {
            _canvas.LoadBlueprintModule(entry.Module, entry.Name);
            _statusLabel.Text = $"Loaded '{entry.Name}' from {_canvas.Project.ActiveCampSlot}.";
            return;
        }

        if (!string.IsNullOrWhiteSpace(entry.FilePath) && entry.FilePath != "(in-memory)" && File.Exists(entry.FilePath))
        {
            try
            {
                _canvas.LoadBlueprint(entry.FilePath);
                _statusLabel.Text = $"Loaded '{entry.Name}' from library.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Load failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            MessageBox.Show(this, "Blueprint source file was not found and no in-memory module exists for this entry.", "Load from library", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void RemoveBlueprintFromLibrary()
    {
        if (_blueprintLibraryList.SelectedItem is not BlueprintLibraryEntry entry)
        {
            MessageBox.Show(this, "Select a blueprint from the slot library first.", "Remove from library", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _canvas.Project.BlueprintLibrary.Remove(entry);
        RefreshBlueprintLibraryUi();
        _statusLabel.Text = $"Removed '{entry.Name}' from library.";
    }

    private void RefreshPresetDescription()
    {
        if (_presetCombo.SelectedItem is ProjectPreset preset)
        {
            _presetDescriptionLabel.Text = preset.Description;
        }
    }

    private void SaveSelectionAsBlueprint()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "FO76 Blueprint JSON (*.blueprint.json)|*.blueprint.json|JSON (*.json)|*.json",
            FileName = SafeFileName(_canvas.Project.Name) + "-module.blueprint.json",
            Title = "Save selection as blueprint"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _canvas.ExportSelectionAsBlueprint(dialog.FileName, Path.GetFileNameWithoutExtension(dialog.FileName));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Blueprint save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadBlueprint()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "FO76 Blueprint JSON (*.blueprint.json;*.json)|*.blueprint.json;*.json|All Files (*.*)|*.*",
            Title = "Load blueprint"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _canvas.LoadBlueprint(dialog.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Blueprint load failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenProject()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "FO76 Planner JSON (*.json)|*.json|All Files (*.*)|*.*",
            Title = "Open planner project"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var json = File.ReadAllText(dialog.FileName);
            var project = JsonSerializer.Deserialize<PlannerProject>(json, AppJson.Default);
            if (project is null)
            {
                throw new InvalidOperationException("Could not deserialize project file.");
            }

            NormalizeProject(project);
            _currentPath = dialog.FileName;
            _isDirty = false;
            _canvas.Project = project;
            RefreshProjectUi();
            _statusLabel.Text = $"Opened {Path.GetFileName(dialog.FileName)}.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Open failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveProject(bool forceChoosePath)
    {
        var path = _currentPath;
        if (forceChoosePath || string.IsNullOrWhiteSpace(path))
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "FO76 Planner JSON (*.json)|*.json",
                FileName = SafeFileName(_canvas.Project.Name) + ".json",
                Title = "Save planner project"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            path = dialog.FileName;
            _currentPath = path;
        }

        try
        {
            var json = JsonSerializer.Serialize(_canvas.Project, AppJson.Default);
            File.WriteAllText(path!, json);
            _isDirty = false;
            _statusLabel.Text = $"Saved {Path.GetFileName(path)}.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExportPng()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "PNG Image (*.png)|*.png",
            FileName = SafeFileName(_canvas.Project.Name) + ".png",
            Title = "Export canvas as PNG"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            using var bitmap = _canvas.RenderToBitmap();
            bitmap.Save(dialog.FileName);
            _statusLabel.Text = $"Exported {Path.GetFileName(dialog.FileName)}.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Export failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static PlannerProject NormalizeProject(PlannerProject project)
    {
        project.PresetId = string.IsNullOrWhiteSpace(project.PresetId) ? "custom" : project.PresetId;
        project.LayerVisibility ??= new LayerVisibilitySettings();
        project.LayerLocks ??= new LayerLockSettings();
        project.VisitorMarkers ??= new List<VisitorMarker>();
        project.TrapZones ??= new List<TrapZonePlan>();
        project.BlueprintLibrary ??= new List<BlueprintLibraryEntry>();
        project.DefenseReviewNotes ??= string.Empty;
        project.Name = string.IsNullOrWhiteSpace(project.Name) ? "Untitled CAMP" : project.Name;
        project.GridWidth = Math.Max(10, project.GridWidth);
        project.GridHeight = Math.Max(10, project.GridHeight);
        project.BudgetLimit = Math.Max(100, project.BudgetLimit);
        project.CellSize = Math.Max(24, project.CellSize);

        var order = 1;
        foreach (var marker in project.VisitorMarkers.OrderBy(x => x.Order).ThenBy(x => x.Id))
        {
            marker.Order = order++;
            marker.Label = string.IsNullOrWhiteSpace(marker.Label)
                ? marker.Type switch
                {
                    VisitorMarkerType.Ingress => "Ingress",
                    VisitorMarkerType.Checkpoint => "Checkpoint",
                    VisitorMarkerType.Egress => "Egress",
                    _ => marker.Type.ToString()
                }
                : marker.Label.Trim();
        }

        foreach (var trapZone in project.TrapZones)
        {
            trapZone.Label = string.IsNullOrWhiteSpace(trapZone.Label) ? "Zone" : trapZone.Label.Trim();
            trapZone.Notes ??= string.Empty;
            trapZone.Width = Math.Max(1, trapZone.Width);
            trapZone.Height = Math.Max(1, trapZone.Height);
        }

        return project;
    }

    private void ToggleSnapMode()
    {
        _canvas.Project.SnapEnabled = !_canvas.Project.SnapEnabled;
        _canvas.Invalidate();
        RefreshProjectUi();
        _statusLabel.Text = _canvas.Project.SnapEnabled ? "Smart snap enabled." : "Smart snap disabled.";
    }

    private void ApplyBudgetProfile(BudgetPlaystyleProfile profile)
    {
        _canvas.Project.BudgetProfile = profile;
        if (BudgetProfileLibrary.Profiles.TryGetValue(profile, out var preset))
        {
            _canvas.Project.BudgetLimit = preset.BudgetLimit;
            _canvas.Project.StoredBudget = preset.StoredBudget;
            _statusLabel.Text = $"Budget profile applied: {profile}.";
        }
        RefreshProjectUi();
    }

    private void ApplyOverlayPreset(OverlayReviewPreset preset)
    {
        var project = _canvas.Project;
        project.OverlayPreset = preset;

        switch (preset)
        {
            case OverlayReviewPreset.VisitorFlow:
                project.ShowCampRadiusOverlay = true;
                project.ShowTurretCoverage = false;
                project.ShowVisitorFlowOverlay = true;
                project.ShowTrapZonesOverlay = false;
                break;
            case OverlayReviewPreset.TrapReview:
                project.ShowCampRadiusOverlay = false;
                project.ShowTurretCoverage = true;
                project.ShowVisitorFlowOverlay = true;
                project.ShowTrapZonesOverlay = true;
                break;
            case OverlayReviewPreset.DefenseReview:
                project.ShowCampRadiusOverlay = true;
                project.ShowTurretCoverage = true;
                project.ShowVisitorFlowOverlay = false;
                project.ShowTrapZonesOverlay = true;
                break;
            case OverlayReviewPreset.Presentation:
                project.ShowCampRadiusOverlay = false;
                project.ShowTurretCoverage = false;
                project.ShowVisitorFlowOverlay = false;
                project.ShowTrapZonesOverlay = false;
                break;
            default:
                project.ShowCampRadiusOverlay = true;
                project.ShowTurretCoverage = true;
                project.ShowVisitorFlowOverlay = true;
                project.ShowTrapZonesOverlay = true;
                break;
        }

        _canvas.Invalidate();
        RefreshProjectUi();
        _statusLabel.Text = $"Overlay preset applied: {preset}.";
    }

    private void UpdateWindowTitle()
    {
        var dirtyPrefix = _isDirty ? "* " : string.Empty;
        var projectName = string.IsNullOrWhiteSpace(_canvas.Project.Name) ? "Untitled CAMP" : _canvas.Project.Name;
        Text = $"{dirtyPrefix}FO76 CAMP Planner - {projectName}";
    }

    private static BlueprintModule CloneBlueprintModule(BlueprintModule module)
    {
        var json = JsonSerializer.Serialize(module, AppJson.Default);
        return JsonSerializer.Deserialize<BlueprintModule>(json, AppJson.Default) ?? new BlueprintModule();
    }


    private static string SafeFileName(string value)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(c, '_');
        }
        return string.IsNullOrWhiteSpace(value) ? "fo76-camp-planner" : value;
    }

    private static decimal ClampDecimal(int value, decimal min, decimal max)
        => Math.Min(max, Math.Max(min, value));

    private static Color GetLayerSwatch(LayerType layer)
        => layer switch
        {
            LayerType.Structure => Color.FromArgb(143, 124, 107),
            LayerType.Utility => Color.FromArgb(114, 170, 123),
            LayerType.Defense => Color.FromArgb(191, 90, 90),
            LayerType.Power => Color.FromArgb(255, 196, 87),
            LayerType.Aesthetic => Color.FromArgb(122, 160, 214),
            LayerType.Commerce => Color.FromArgb(176, 122, 194),
            _ => Accent
        };

    private void MarkDirty()
    {
        _isDirty = true;
        UpdateWindowTitle();
    }

    private sealed class ItemListEntry
    {
        public ItemListEntry(PlacedItem item, string name, bool locked)
        {
            Item = item;
            Name = name;
            Locked = locked;
        }

        public PlacedItem Item { get; }
        public string Name { get; }
        public bool Locked { get; }
        public override string ToString() => Name;
    }
}