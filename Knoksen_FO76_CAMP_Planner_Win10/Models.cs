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

public enum SnapMode
{
    Strict,
    Relaxed,
    Off
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

public enum ThemeProfile
{
    Classic,
    WastelandDark,
    PipBoyContrast
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

public sealed class LaunchTargetProfile
{
    public string Label { get; set; } = "Target";
    public string Target { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public sealed class DeviceHubProfile
{
    public string GitHubRepositoryUrl { get; set; } = "https://github.com/knoksen/Fallout76CAMPplanner";
    public string SourceForgeUrl { get; set; } = string.Empty;
    public string FalloutDocsTarget { get; set; } = "README.md";
    public string MobileCompanionUrl { get; set; } = string.Empty;
    public string MobileExportFolder { get; set; } = "exports/mobile";
    public string ConsoleInstructions { get; set; } = "Configure Xbox/PlayStation/Generic targets with URLs or local docs.";
    public LaunchTargetProfile Xbox { get; set; } = new() { Label = "Xbox" };
    public LaunchTargetProfile PlayStation { get; set; } = new() { Label = "PlayStation" };
    public LaunchTargetProfile GenericConsole { get; set; } = new() { Label = "Generic Console" };

    public void EnsureDefaults()
    {
        GitHubRepositoryUrl = string.IsNullOrWhiteSpace(GitHubRepositoryUrl)
            ? "https://github.com/knoksen/Fallout76CAMPplanner"
            : GitHubRepositoryUrl.Trim();
        SourceForgeUrl = SourceForgeUrl?.Trim() ?? string.Empty;
        FalloutDocsTarget = string.IsNullOrWhiteSpace(FalloutDocsTarget) ? "README.md" : FalloutDocsTarget.Trim();
        MobileCompanionUrl = MobileCompanionUrl?.Trim() ?? string.Empty;
        MobileExportFolder = string.IsNullOrWhiteSpace(MobileExportFolder) ? "exports/mobile" : MobileExportFolder.Trim();
        ConsoleInstructions = ConsoleInstructions?.Trim() ?? string.Empty;

        Xbox ??= new LaunchTargetProfile();
        PlayStation ??= new LaunchTargetProfile();
        GenericConsole ??= new LaunchTargetProfile();

        Xbox.Label = string.IsNullOrWhiteSpace(Xbox.Label) ? "Xbox" : Xbox.Label.Trim();
        PlayStation.Label = string.IsNullOrWhiteSpace(PlayStation.Label) ? "PlayStation" : PlayStation.Label.Trim();
        GenericConsole.Label = string.IsNullOrWhiteSpace(GenericConsole.Label) ? "Generic Console" : GenericConsole.Label.Trim();

        Xbox.Target = Xbox.Target?.Trim() ?? string.Empty;
        PlayStation.Target = PlayStation.Target?.Trim() ?? string.Empty;
        GenericConsole.Target = GenericConsole.Target?.Trim() ?? string.Empty;

        Xbox.Notes = Xbox.Notes?.Trim() ?? string.Empty;
        PlayStation.Notes = PlayStation.Notes?.Trim() ?? string.Empty;
        GenericConsole.Notes = GenericConsole.Notes?.Trim() ?? string.Empty;
    }
}

public sealed class PlannerProject
{
    public string Name { get; set; } = "Untitled CAMP";
    public string PresetId { get; set; } = "custom";
    public BuildMode Mode { get; set; } = BuildMode.SurfaceCamp;
    public RuleProfile RuleProfile { get; set; } = RuleProfile.Strict;
    public SnapMode SnapMode { get; set; } = SnapMode.Strict;

    [JsonIgnore]
    public bool SnapEnabled => SnapMode != SnapMode.Off;

    [JsonPropertyName("SnapEnabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? LegacySnapEnabled
    {
        get => null;
        set
        {
            if (value.HasValue)
            {
                SnapMode = value.Value ? SnapMode.Strict : SnapMode.Off;
            }
        }
    }
    public CampSlot ActiveCampSlot { get; set; } = CampSlot.Slot1;
    public BudgetPlaystyleProfile BudgetProfile { get; set; } = BudgetPlaystyleProfile.Builder;
    public OverlayReviewPreset OverlayPreset { get; set; } = OverlayReviewPreset.Balanced;
    public ThemeProfile ThemeProfile { get; set; } = ThemeProfile.Classic;
    public int BudgetLimit { get; set; } = 1000;
    public int StoredBudget { get; set; }
    public int GridWidth { get; set; } = 40;
    public int GridHeight { get; set; } = 30;
    public int CellSize { get; set; } = 48;
    public bool AutosaveEnabled { get; set; } = true;
    public int AutosaveIntervalMinutes { get; set; } = 2;
    public string? AutosavePath { get; set; }
    public LayerVisibilitySettings LayerVisibility { get; set; } = new();
    public LayerLockSettings LayerLocks { get; set; } = new();
    public bool ShowCampRadiusOverlay { get; set; } = true;
    public bool ShowTurretCoverage { get; set; } = true;
    public bool ShowVisitorFlowOverlay { get; set; } = true;
    public bool ShowTrapZonesOverlay { get; set; } = true;
    public int CampCenterX { get; set; } = -1;
    public int CampCenterY { get; set; } = -1;
    public string DefenseReviewNotes { get; set; } = string.Empty;
    public DeviceHubProfile DeviceHub { get; set; } = new();
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
    public sealed record BudgetProfileSpec(
        int BudgetLimit,
        int StoredBudget,
        string Description,
        int RecommendedTurrets,
        int RecommendedDefenseItems,
        int RecommendedIngressMarkers,
        int RecommendedTrapZones);

    public static readonly IReadOnlyDictionary<BudgetPlaystyleProfile, BudgetProfileSpec> Profiles =
        new Dictionary<BudgetPlaystyleProfile, BudgetProfileSpec>
        {
            [BudgetPlaystyleProfile.Builder] = new BudgetProfileSpec(1200, 120, "Balanced build profile for general construction and iteration.", 3, 6, 1, 1),
            [BudgetPlaystyleProfile.TrapCamp] = new BudgetProfileSpec(1400, 200, "Higher reserve for trap routing and layered control zones.", 4, 8, 1, 3),
            [BudgetPlaystyleProfile.VendorCamp] = new BudgetProfileSpec(1100, 140, "Commerce-focused profile with budget room for vendor frontage.", 2, 4, 1, 1),
            [BudgetPlaystyleProfile.UtilityCamp] = new BudgetProfileSpec(1150, 160, "Utility-heavy profile for workbench and infrastructure density.", 3, 5, 1, 1),
            [BudgetPlaystyleProfile.NukeCamp] = new BudgetProfileSpec(1500, 220, "Aggressive defense and launch-route profile.", 6, 10, 2, 2),
            [BudgetPlaystyleProfile.ShowcaseCamp] = new BudgetProfileSpec(1300, 100, "Presentation-forward profile for decor and layout clarity.", 2, 4, 1, 1)
        };
}

public static class ShelterRuleLibrary
{
    public sealed record ShelterRuleSpec(
        int MaxTurrets,
        int MaxVisitorMarkers,
        int MaxTrapZones,
        TrapZoneSeverity MaxTrapSeverity,
        string Guidance);

    public static readonly IReadOnlyDictionary<string, ShelterRuleSpec> ByPresetId =
        new Dictionary<string, ShelterRuleSpec>(StringComparer.OrdinalIgnoreCase)
        {
            ["vault-lobby"] = new ShelterRuleSpec(5, 10, 4, TrapZoneSeverity.High, "Balanced shelter flow with moderate trap layering."),
            ["vault-utility"] = new ShelterRuleSpec(3, 6, 3, TrapZoneSeverity.High, "Compact utility shell; prioritize clear movement lanes."),
            ["missile-silo"] = new ShelterRuleSpec(8, 14, 6, TrapZoneSeverity.Critical, "High-intensity route control profile for long corridors."),
            ["nuclear-bunker"] = new ShelterRuleSpec(6, 12, 5, TrapZoneSeverity.Critical, "Segmented bunker profile with layered defensive fallback."),
            ["flatlands"] = new ShelterRuleSpec(7, 12, 5, TrapZoneSeverity.High, "Wide layout profile focused on coverage spacing."),
            ["triumph-terrace"] = new ShelterRuleSpec(4, 10, 3, TrapZoneSeverity.Medium, "Presentation-first profile with controlled trap intensity."),
            ["wrangler-casino"] = new ShelterRuleSpec(3, 9, 3, TrapZoneSeverity.Medium, "Social/event profile where readability and flow are prioritized.")
        };

    public static bool TryGetForPreset(string? presetId, out ShelterRuleSpec spec)
    {
        if (!string.IsNullOrWhiteSpace(presetId) && ByPresetId.TryGetValue(presetId, out spec!))
        {
            return true;
        }

        spec = null!;
        return false;
    }
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

// ---------------------------------------------------------------------------
// Portable cross-platform plan format (shared with the mobile companion app)
// ---------------------------------------------------------------------------

/// <summary>
/// A lightweight plan representation shared between the desktop and mobile apps.
/// The mobile app exports this via Share; the desktop imports it via File Open.
/// </summary>
public sealed class MobilePortablePlan
{
    public string SchemaVersion { get; set; } = "1";
    public string SchemaType { get; set; } = "fo76camp-portable-plan";
    public string Name { get; set; } = "Imported Plan";
    public int Budget { get; set; } = 1000;
    public string ExportedAt { get; set; } = string.Empty;
    public string Source { get; set; } = "mobile";
    public List<MobilePortableItem> Items { get; set; } = new();

    public bool IsValid()
        => SchemaType == "fo76camp-portable-plan"
           && SchemaVersion == "1"
           && Items is not null;
}

public sealed class MobilePortableItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Cost { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Notes { get; set; }
}
