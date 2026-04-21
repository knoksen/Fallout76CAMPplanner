using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FO76CampPlanner.Tests;

public class BlueprintServiceTests
{
    [Fact]
    public async Task LoadAsync_ParsesBlueprintModule()
    {
        var service = new BlueprintService();
        var tmpDir = Path.Combine(Path.GetTempPath(), "FO76Test", "blueprint");
        Directory.CreateDirectory(tmpDir);
        var path = Path.Combine(tmpDir, "test.blueprint.json");

        var module = new BlueprintModule
        {
            Name = "Test Blueprint",
            Items = new System.Collections.Generic.List<BlueprintItem>
            {
                new BlueprintItem { DefinitionId = "foundation", X = 0, Y = 0 }
            }
        };

        var json = JsonSerializer.Serialize(module, AppJson.Default);
        await File.WriteAllTextAsync(path, json);

        var loaded = await service.LoadAsync(path);

        Assert.NotNull(loaded);
        Assert.Equal(module.Name, loaded.Name);
        Assert.NotEmpty(loaded.Items);
        Assert.Equal("foundation", loaded.Items[0].DefinitionId);
    }
}
