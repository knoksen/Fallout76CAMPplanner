using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FO76CampPlanner;

public sealed class BlueprintService : IBlueprintService
{
    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<BlueprintModule> LoadAsync(string path, CancellationToken ct = default)
    {
        using var fs = File.OpenRead(path);
        var module = await JsonSerializer.DeserializeAsync<BlueprintModule>(fs, _options, ct).ConfigureAwait(false);
        return module ?? new BlueprintModule();
    }
}
