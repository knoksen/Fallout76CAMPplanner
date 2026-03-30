using System.Drawing;
using System.Text.Json.Serialization;

namespace FO76CampPlanner;

public enum BuildMode
{
    SurfaceCamp,
    Shelter
}

public enum RuleProfile
{
    Strict,
    Relaxed,
    Shelter
}

public enum ToolType
{
    Select,
    Foundation,
    Wall,
    Door,
    Stairs,
    Roof,
    Workbench,
    Turret,
    Power,
    Light,
    Decor,
    Vendor,
    Resource,
    Display,
    Ally,
    Erase
}

public enum LayerType
{
    Structure,
    Utility,
    Defense,
    Power,
    Aesthetic,
    Commerce
}

public enum CampSlot
{
    Slot1,
    Slot2,
    Slot3,
    Slot4
}

public enum BudgetPlaystyleProfile
{
    Builder,
    TrapCamp,
    VendorCamp,
    UtilityCamp,
    NukeCamp,
    ShowcaseCamp
}

public enum VisitorMarkerType
{
    Ingress,
    Checkpoint,
    Egress
}

public enum OverlayReviewPreset
{
    Balanced,
    VisitorFlow,
    TrapReview,
    DefenseReview,
    Presentation
}

public enum TrapZoneSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public sealed class LayerVisibilitySettings
{
    public bool Structure { get; set; } = true;
    public bool Utility { get; set; } = true;
    public bool Defense { get; set; } = true;
    public bool Power { get; set; } = true;
    public bool Aesthetic { get; set; } = true;
    public bool Commerce { get; set; } = true;

    public bool IsVisible(LayerType layer)
        => layer switch
        {
            LayerType.Structure => Structure,
            LayerType.Utility => Utility,
            LayerType.Defense => Defense,
            LayerType.Power => Power,
            LayerType.Aesthetic => Aesthetic,
            LayerType.Commerce => Commerce,
            _ => true
        };

    public void SetVisible(LayerType layer, bool value)
    {
        switch (layer)
        {
            case LayerType.Structure:
                Structure = value;
                break;
            case LayerType.Utility:
                Utility = value;
                break;
            case LayerType.Defense:
                Defense = value;
                break;
            case LayerType.Power:
                Power = value;
                break;
            case LayerType.Aesthetic:
                Aesthetic = value;
                break;
            case LayerType.Commerce:
                Commerce = value;
                break;
        }
    }

    public void ShowAll()
    {
        Structure = true;
        Utility = true;
        Defense = true;
        Power = true;
        Aesthetic = true;
        Commerce = true;
    }
}



public sealed class LayerLockSettings
{
    public bool Structure { get; set; }
    public bool Utility { get; set; }
    public bool Defense { get; set; }
    public bool Power { get; set; }
    public bool Aesthetic { get; set; }
    public bool Commerce { get; set; }

    public bool IsLocked(LayerType layer)
        => layer switch
        {
            LayerType.Structure => Structure,
            LayerType.Utility => Utility,
            LayerType.Defense => Defense,
            LayerType.Power => Power,
            LayerType.Aesthetic => Aesthetic,
            LayerType.Commerce => Commerce,
            _ => false
        };

    public void SetLocked(LayerType layer, bool value)
    {
        switch (layer)
        {
            case LayerType.Structure:
                Structure = value;
                break;
            case LayerType.Utility:
                Utility = value;
                break;
            case LayerType.Defense:
                Defense = value;
                break;
            case LayerType.Power:
                Power = value;
                break;
            case LayerType.Aesthetic:
                Aesthetic = value;
                break;
            case LayerType.Commerce:
                Commerce = value;
                break;
        }
    }

    public void UnlockAll()
    {
        Structure = false;
        Utility = false;
        Defense = false;
        Power = false;
        Aesthetic = false;
        Commerce = false;
    }
}

public sealed class ItemDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Width { get; init; } = 1;
    public int Height { get; init; } = 1;
    public int BudgetCost { get; init; } = 10;
    public LayerType Layer { get; init; } = LayerType.Structure;
    public bool AllowedInSurface { get; init; } = true;
    public bool AllowedInShelter { get; init; } = true;
    public string ColorHex { get; init; } = "#4A90E2";

    [JsonIgnore]
    public Color DisplayColor => ColorTranslator.FromHtml(ColorHex);
}

public sealed class PlacedItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DefinitionId { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int Rotation { get; set; }
    public string? Note { get; set; }
}

public sealed class VisitorMarker
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public VisitorMarkerType Type { get; set; } = VisitorMarkerType.Checkpoint;
    public int Order { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Label { get; set; } = string.Empty;

    public override string ToString()
        => $"{Math.Max(1, Order):00}. {Label} [{Type}] ({X}, {Y})";
}

public sealed class TrapZonePlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Label { get; set; } = "Zone";
    public TrapZoneSeverity Severity { get; set; } = TrapZoneSeverity.Medium;
    public string Notes { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 1;
    public int Height { get; set; } = 1;

    public override string ToString()
        => $"{Label} [{Severity}] {Width}x{Height} @ ({X}, {Y})";
}

public sealed class PlannerProject
{
    public string Name { get; set; } = "Untitled CAMP";
    public string PresetId { get; set; } = "custom";
    public BuildMode Mode { get; set; } = BuildMode.SurfaceCamp;
    public RuleProfile RuleProfile { get; set; } = RuleProfile.Strict;
    public bool SnapEnabled { get; set; } = true;
    public CampSlot ActiveCampSlot { get; set; } = CampSlot.Slot1;
    public BudgetPlaystyleProfile BudgetProfile { get; set; } = BudgetPlaystyleProfile.Builder;
    public OverlayReviewPreset OverlayPreset { get; set; } = OverlayReviewPreset.Balanced;
    public int BudgetLimit { get; set; } = 1000;
    public int StoredBudget { get; set; }
    public int GridWidth { get; set; } = 40;
    public int GridHeight { get; set; } = 30;
    public int CellSize { get; set; } = 48;
    public LayerVisibilitySettings LayerVisibility { get; set; } = new();
    public LayerLockSettings LayerLocks { get; set; } = new();
    public bool ShowCampRadiusOverlay { get; set; } = true;
    public bool ShowTurretCoverage { get; set; } = true;
    public bool ShowVisitorFlowOverlay { get; set; } = true;
    public bool ShowTrapZonesOverlay { get; set; } = true;
    public int CampCenterX { get; set; } = -1;
    public int CampCenterY { get; set; } = -1;
    public string DefenseReviewNotes { get; set; } = string.Empty;
    public List<VisitorMarker> VisitorMarkers { get; set; } = new();
    public List<TrapZonePlan> TrapZones { get; set; } = new();
    public List<BlueprintLibraryEntry> BlueprintLibrary { get; set; } = new();
    public List<PlacedItem> Items { get; set; } = new();
}

public sealed class ProjectPreset
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public BuildMode Mode { get; init; } = BuildMode.SurfaceCamp;
    public RuleProfile RuleProfile { get; init; } = RuleProfile.Strict;
    public int GridWidth { get; init; }
    public int GridHeight { get; init; }
    public int BudgetLimit { get; init; } = 1000;
    public int StoredBudget { get; init; }

    public PlannerProject CreateProject()
        => new()
        {
            Name = Name,
            PresetId = Id,
            Mode = Mode,
            RuleProfile = RuleProfile,
            GridWidth = GridWidth,
            GridHeight = GridHeight,
            BudgetLimit = BudgetLimit,
            StoredBudget = StoredBudget,
            CellSize = 48,
            LayerVisibility = new LayerVisibilitySettings(),
            LayerLocks = new LayerLockSettings(),
            ShowCampRadiusOverlay = true,
            ShowTurretCoverage = true,
            CampCenterX = GridWidth / 2,
            CampCenterY = GridHeight / 2
        };
}

public sealed class BlueprintItem
{
    public string DefinitionId { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int Rotation { get; set; }
    public string? Note { get; set; }
}

public sealed class BlueprintModule
{
    public string Name { get; set; } = "Untitled Blueprint";
    public string Description { get; set; } = string.Empty;
    public BuildMode? RecommendedMode { get; set; }
    public List<BlueprintItem> Items { get; set; } = new();
}

public sealed class BlueprintLibraryEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public CampSlot Slot { get; set; } = CampSlot.Slot1;
    public string Name { get; set; } = "Unnamed Blueprint";
    public string FilePath { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BlueprintModule? Module { get; set; }
    public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;

    public override string ToString()
        => string.IsNullOrWhiteSpace(Name)
            ? $"{Slot} • {AddedAtUtc:yyyy-MM-dd}"
            : $"{Name} • {AddedAtUtc:yyyy-MM-dd}";
}

public static class Catalog
{
    public static readonly IReadOnlyDictionary<ToolType, ItemDefinition> ByTool =
        new Dictionary<ToolType, ItemDefinition>
        {
            [ToolType.Foundation] = new ItemDefinition
            {
                Id = "foundation",
                Name = "Foundation",
                Width = 1,
                Height = 1,
                BudgetCost = 18,
                Layer = LayerType.Structure,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#6B5B53"
            },
            [ToolType.Wall] = new ItemDefinition
            {
                Id = "wall",
                Name = "Wall",
                Width = 1,
                Height = 1,
                BudgetCost = 9,
                Layer = LayerType.Structure,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#8F7C6B"
            },
            [ToolType.Door] = new ItemDefinition
            {
                Id = "door",
                Name = "Door",
                Width = 1,
                Height = 1,
                BudgetCost = 10,
                Layer = LayerType.Structure,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#B06C49"
            },
            [ToolType.Stairs] = new ItemDefinition
            {
                Id = "stairs",
                Name = "Stairs",
                Width = 1,
                Height = 2,
                BudgetCost = 16,
                Layer = LayerType.Structure,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#9C8E72"
            },
            [ToolType.Roof] = new ItemDefinition
            {
                Id = "roof",
                Name = "Roof",
                Width = 1,
                Height = 1,
                BudgetCost = 12,
                Layer = LayerType.Structure,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#5F6D7A"
            },
            [ToolType.Workbench] = new ItemDefinition
            {
                Id = "workbench",
                Name = "Workbench",
                Width = 2,
                Height = 1,
                BudgetCost = 14,
                Layer = LayerType.Utility,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#4A7A5A"
            },
            [ToolType.Turret] = new ItemDefinition
            {
                Id = "turret",
                Name = "Turret",
                Width = 1,
                Height = 1,
                BudgetCost = 20,
                Layer = LayerType.Defense,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#B43F3F"
            },
            [ToolType.Power] = new ItemDefinition
            {
                Id = "power",
                Name = "Power",
                Width = 1,
                Height = 1,
                BudgetCost = 12,
                Layer = LayerType.Power,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#D4A017"
            },
            [ToolType.Light] = new ItemDefinition
            {
                Id = "light",
                Name = "Light",
                Width = 1,
                Height = 1,
                BudgetCost = 6,
                Layer = LayerType.Power,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#FFE07A"
            },
            [ToolType.Decor] = new ItemDefinition
            {
                Id = "decor",
                Name = "Decor",
                Width = 1,
                Height = 1,
                BudgetCost = 8,
                Layer = LayerType.Aesthetic,
                AllowedInSurface = true,
                AllowedInShelter = true,
                ColorHex = "#7C56A0"
            },
            [ToolType.Vendor] = new ItemDefinition
            {
                Id = "vendor",
                Name = "Vendor",
                Width = 2,
                Height = 1,
                BudgetCost = 18,
                Layer = LayerType.Commerce,
                AllowedInSurface = true,
                AllowedInShelter = false,
                ColorHex = "#2E86AB"
            },
            [ToolType.Resource] = new ItemDefinition
            {
                Id = "resource",
                Name = "Resource",
                Width = 2,
                Height = 1,
                BudgetCost = 16,
                Layer = LayerType.Utility,
                AllowedInSurface = true,
                AllowedInShelter = false,
                ColorHex = "#5F9E5B"
            },
            [ToolType.Display] = new ItemDefinition
            {
                Id = "display",
                Name = "Display",
                Width = 2,
                Height = 1,
                BudgetCost = 10,
                Layer = LayerType.Aesthetic,
                AllowedInSurface = true,
                AllowedInShelter = false,
                ColorHex = "#8D6AC8"
            },
            [ToolType.Ally] = new ItemDefinition
            {
                Id = "ally",
                Name = "Ally",
                Width = 1,
                Height = 1,
                BudgetCost = 16,
                Layer = LayerType.Utility,
                AllowedInSurface = true,
                AllowedInShelter = false,
                ColorHex = "#C86B6B"
            }
        };

    public static readonly IReadOnlyDictionary<string, ItemDefinition> ById =
        ByTool.Values.ToDictionary(v => v.Id, v => v, StringComparer.OrdinalIgnoreCase);

    public static ItemDefinition? GetForTool(ToolType tool)
        => ByTool.TryGetValue(tool, out var item) ? item : null;
}

public static class BudgetProfileLibrary
{
    public static readonly IReadOnlyDictionary<BudgetPlaystyleProfile, (int BudgetLimit, int StoredBudget, string Description)> Profiles =
        new Dictionary<BudgetPlaystyleProfile, (int BudgetLimit, int StoredBudget, string Description)>
        {
            [BudgetPlaystyleProfile.Builder] = (1200, 120, "Balanced build profile for general construction and iteration."),
            [BudgetPlaystyleProfile.TrapCamp] = (1400, 200, "Higher reserve for trap routing and layered control zones."),
            [BudgetPlaystyleProfile.VendorCamp] = (1100, 140, "Commerce-focused profile with budget room for vendor frontage."),
            [BudgetPlaystyleProfile.UtilityCamp] = (1150, 160, "Utility-heavy profile for workbench and infrastructure density."),
            [BudgetPlaystyleProfile.NukeCamp] = (1500, 220, "Aggressive defense and launch-route profile."),
            [BudgetPlaystyleProfile.ShowcaseCamp] = (1300, 100, "Presentation-forward profile for decor and layout clarity.")
        };
}

public static class PresetLibrary
{
    public static readonly IReadOnlyList<ProjectPreset> All =
        new List<ProjectPreset>
        {
            new()
            {
                Id = "custom",
                Name = "Custom Surface CAMP",
                Description = "Blank surface CAMP canvas for free planning.",
                Mode = BuildMode.SurfaceCamp,
                RuleProfile = RuleProfile.Strict,
                GridWidth = 40,
                GridHeight = 30,
                BudgetLimit = 1000
            },
            new()
            {
                Id = "vault-lobby",
                Name = "Vault Lobby Shelter",
                Description = "Balanced indoor shelter footprint for circulation planning.",
                Mode = BuildMode.Shelter,
                RuleProfile = RuleProfile.Shelter,
                GridWidth = 42,
                GridHeight = 28,
                BudgetLimit = 1200
            },
            new()
            {
                Id = "vault-utility",
                Name = "Vault Utility Room",
                Description = "Compact utility shell for benches and compact logic.",
                Mode = BuildMode.Shelter,
                RuleProfile = RuleProfile.Shelter,
                GridWidth = 24,
                GridHeight = 18,
                BudgetLimit = 900
            },
            new()
            {
                Id = "missile-silo",
                Name = "Missile Silo Shelter",
                Description = "Large linear layout for tactical, trap or control-room builds.",
                Mode = BuildMode.Shelter,
                RuleProfile = RuleProfile.Shelter,
                GridWidth = 56,
                GridHeight = 32,
                BudgetLimit = 1500
            },
            new()
            {
                Id = "nuclear-bunker",
                Name = "Nuclear Test Bunker",
                Description = "Medium bunker footprint with room for segmented zones.",
                Mode = BuildMode.Shelter,
                RuleProfile = RuleProfile.Shelter,
                GridWidth = 38,
                GridHeight = 24,
                BudgetLimit = 1300
            },
            new()
            {
                Id = "flatlands",
                Name = "The Flatlands Shelter",
                Description = "Wide flat planning area for showcase builds.",
                Mode = BuildMode.Shelter,
                RuleProfile = RuleProfile.Shelter,
                GridWidth = 60,
                GridHeight = 34,
                BudgetLimit = 1600
            },
            new()
            {
                Id = "triumph-terrace",
                Name = "Triumph Terrace",
                Description = "Residential showcase footprint for high-end shelter layouts.",
                Mode = BuildMode.Shelter,
                RuleProfile = RuleProfile.Shelter,
                GridWidth = 44,
                GridHeight = 28,
                BudgetLimit = 1450
            },
            new()
            {
                Id = "wrangler-casino",
                Name = "Wrangler Casino",
                Description = "Entertainment and social layout shell.",
                Mode = BuildMode.Shelter,
                RuleProfile = RuleProfile.Shelter,
                GridWidth = 48,
                GridHeight = 30,
                BudgetLimit = 1500
            },
            new()
            {
                Id = "nuke-camp",
                Name = "Nuke Surface CAMP",
                Description = "Fast-launch surface blueprint area with defense headroom.",
                Mode = BuildMode.SurfaceCamp,
                RuleProfile = RuleProfile.Relaxed,
                GridWidth = 36,
                GridHeight = 26,
                BudgetLimit = 1100
            }
        };

    public static ProjectPreset GetById(string? presetId)
        => All.FirstOrDefault(x => string.Equals(x.Id, presetId, StringComparison.OrdinalIgnoreCase)) ?? All[0];
}
