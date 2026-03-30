using System.Text.Json;

namespace FO76CampPlanner;

internal static class AppJson
{
    public static readonly JsonSerializerOptions Default = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}

internal static class AppDiagnostics
{
    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "FO76CampPlanner_Error.log");

    public static void TryAppend(string message)
    {
        try
        {
            File.AppendAllText(LogPath, message + Environment.NewLine);
        }
        catch
        {
            // Non-fatal diagnostics path, never crash on logging.
        }
    }

    public static string GetLogPath() => LogPath;
}
