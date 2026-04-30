using System.Text.Json;
using System.Text.RegularExpressions;

namespace FO76CampPlanner;

/// <summary>
/// Manages offline license activation and lifetime stats.
/// Persistence is via plain JSON in the user's AppData folder.
/// Note: offline key validation is UX friction only; no server-side DRM.
/// </summary>
internal static partial class PremiumStore
{
    private static readonly string DataFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "FO76CampPlanner");

    private static readonly string StorePath = Path.Combine(DataFolder, "premium.json");
    private static readonly string StatsPath = Path.Combine(DataFolder, "stats.json");

    // Compiled regex: ^FO76-[PE][A-Z0-9]{3}-[A-Z0-9]{4}-[A-Z0-9]{4}$
    // Bounded, no nested quantifiers — no ReDoS risk.
    [GeneratedRegex(@"^FO76-[PE][A-Z0-9]{3}-[A-Z0-9]{4}-[A-Z0-9]{4}$", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    private static partial Regex KeyPattern();

    private static PremiumState? _state;
    private static AppLifetimeStats? _stats;

    public static PremiumState State
    {
        get
        {
            if (_state is not null) return _state;
            _state = Load<PremiumState>(StorePath) ?? new PremiumState();
            return _state;
        }
    }

    public static AppLifetimeStats Stats
    {
        get
        {
            if (_stats is not null) return _stats;
            _stats = Load<AppLifetimeStats>(StatsPath) ?? new AppLifetimeStats();
            return _stats;
        }
    }

    /// <summary>
    /// Validates and activates a license key. Returns true on success.
    /// Key format: FO76-P/E + 3 chars – 4 chars – 4 chars.
    /// 'P' activates Pro tier; 'E' activates VaultTecElite tier.
    /// </summary>
    public static bool TryActivate(string licenseKey, out string error)
    {
        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            error = "License key cannot be empty.";
            return false;
        }

        var key = licenseKey.Trim().ToUpperInvariant();
        if (!KeyPattern().IsMatch(key))
        {
            error = "Invalid key format. Keys look like: FO76-PXXX-XXXX-XXXX (Pro) or FO76-EXXX-XXXX-XXXX (Elite).";
            return false;
        }

        var tier = key[5] == 'E' ? PremiumTier.VaultTecElite : PremiumTier.Pro;
        State.Tier = tier;
        State.LicenseKey = key;
        State.ActivatedAtUtc = DateTime.UtcNow;
        Save(StorePath, State);
        error = string.Empty;
        return true;
    }

    public static void IncrementSaves()
    {
        Stats.TotalSaves++;
        Save(StatsPath, Stats);
    }

    public static void IncrementItemsPlaced(int count = 1)
    {
        Stats.TotalItemsPlaced += count;
        Save(StatsPath, Stats);
    }

    public static void UnlockAchievement(AchievementId id)
    {
        if (Stats.UnlockedAchievements.Add(id.ToString()))
        {
            Save(StatsPath, Stats);
        }
    }

    public static bool IsUnlocked(AchievementId id)
        => Stats.UnlockedAchievements.Contains(id.ToString());

    private static T? Load<T>(string path) where T : class
    {
        try
        {
            if (!File.Exists(path)) return null;
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, AppJson.Default);
        }
        catch
        {
            return null;
        }
    }

    private static void Save<T>(string path, T obj)
    {
        try
        {
            Directory.CreateDirectory(DataFolder);
            File.WriteAllText(path, JsonSerializer.Serialize(obj, AppJson.Default));
        }
        catch
        {
            // Non-fatal — don't crash on persistence failures.
        }
    }
}
