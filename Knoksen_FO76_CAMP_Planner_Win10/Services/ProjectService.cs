using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FO76CampPlanner;

public sealed class ProjectService : IProjectService
{
    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<PlannerProject> LoadAsync(string path, CancellationToken ct = default)
    {
        using var fs = File.OpenRead(path);
        var project = await JsonSerializer.DeserializeAsync<PlannerProject>(fs, _options, ct).ConfigureAwait(false);
        return project ?? new PlannerProject();
    }

    public async Task SaveAsync(PlannerProject project, string path, CancellationToken ct = default)
    {
        var dir = Path.GetDirectoryName(path) ?? ".";
        Directory.CreateDirectory(dir);
        using var fs = File.Create(path);
        await JsonSerializer.SerializeAsync(fs, project, _options, ct).ConfigureAwait(false);
        await fs.FlushAsync(ct).ConfigureAwait(false);
    }

    public Task<string> GetAutosavePathAsync(string projectPath)
    {
        var dir = Path.GetDirectoryName(projectPath) ?? ".";
        var autosaveDir = Path.Combine(dir, ".autosave");
        var autosave = Path.Combine(autosaveDir, Path.GetFileNameWithoutExtension(projectPath) + ".autosave.json");
        return Task.FromResult(autosave);
    }

    public async Task AutosaveAsync(PlannerProject project, string autosavePath, CancellationToken ct = default)
    {
        var dir = Path.GetDirectoryName(autosavePath) ?? ".";
        Directory.CreateDirectory(dir);
        var tempPath = autosavePath + ".tmp";
        using (var fs = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(fs, project, _options, ct).ConfigureAwait(false);
            await fs.FlushAsync(ct).ConfigureAwait(false);
        }

        File.Move(tempPath, autosavePath, overwrite: true);
    }
}
