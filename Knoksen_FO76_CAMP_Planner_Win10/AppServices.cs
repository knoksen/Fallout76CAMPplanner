using System.Drawing;
using System.Text.Json;

namespace FO76CampPlanner;

internal static class AppJson
{
    public static readonly JsonSerializerOptions Default = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // Guard against deeply nested or excessively large JSON payloads
        MaxDepth = 64
    };
}

internal static class AppDiagnostics
{
    private const long MaxLogSizeBytes = 1 * 1024 * 1024; // 1 MB — rotate before growing unbounded

    private static readonly object LogLock = new();

    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "FO76CampPlanner_Error.log");

    public static void TryAppend(string message)
    {
        try
        {
            lock (LogLock)
            {
                var fileInfo = new FileInfo(LogPath);
                if (fileInfo.Exists && fileInfo.Length > MaxLogSizeBytes)
                {
                    // Rotate: overwrite the backup and start a fresh log
                    File.Move(LogPath, LogPath + ".old", overwrite: true);
                }

                File.AppendAllText(LogPath, message + Environment.NewLine);
            }
        }
        catch
        {
            // Non-fatal diagnostics path, never crash on logging.
        }
    }

    public static string GetLogPath() => LogPath;
}

internal interface IThreeDPreviewAdapter
{
    string AdapterName { get; }
    bool IsAvailable();
    ThreeDPreviewPayload BuildPreviewPayload(PlannerProject project);
}

internal sealed record ThreeDPreviewItem(
    Guid Id,
    string DefinitionId,
    LayerType Layer,
    int X,
    int Y,
    int Rotation,
    int Width,
    int Height);

internal sealed record ThreeDPreviewPayload(
    string ProjectName,
    int GridWidth,
    int GridHeight,
    IReadOnlyList<ThreeDPreviewItem> Items,
    IReadOnlyList<TrapZonePlan> TrapZones,
    IReadOnlyList<VisitorMarker> VisitorMarkers);

internal interface IPrintExportAdapter
{
    string AdapterName { get; }
    bool IsAvailable();
    void Export(PrintExportDocument document, string outputPath);
}

internal sealed record PrintExportDocument(
    string Title,
    DateTime GeneratedAtUtc,
    PlannerProject Project,
    Bitmap CanvasSnapshot,
    IReadOnlyList<string> SummaryLines);

internal static class FutureFeatureFlags
{
    public static bool EnableThreeDPreview => false;
    public static bool EnablePdfExport => false;
    public static bool EnableThemeProfiles => false;
}
