using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FO76CampPlanner.Tests;

public class ProjectServiceTests
{
    [Fact]
    public async Task SaveAndLoad_RoundtripsProject()
    {
        var service = new ProjectService();
        var tmp = Path.Combine(Path.GetTempPath(), "FO76Test", "roundtrip.json");
        Directory.CreateDirectory(Path.GetDirectoryName(tmp)!);

        var project = new PlannerProject
        {
            Name = "Test Camp",
            GridWidth = 10,
            GridHeight = 8,
            BudgetLimit = 500
        };

        await service.SaveAsync(project, tmp);
        var loaded = await service.LoadAsync(tmp);

        Assert.NotNull(loaded);
        Assert.Equal(project.Name, loaded.Name);
        Assert.Equal(project.GridWidth, loaded.GridWidth);
        Assert.Equal(project.GridHeight, loaded.GridHeight);
        Assert.Equal(project.BudgetLimit, loaded.BudgetLimit);
    }

    [Fact]
    public async Task Autosave_CreatesFile()
    {
        var service = new ProjectService();
        var dir = Path.Combine(Path.GetTempPath(), "FO76Test", "autosave");
        Directory.CreateDirectory(dir);
        var autosave = Path.Combine(dir, "autosave.json");

        var project = new PlannerProject { Name = "Autosave Camp" };
        await service.AutosaveAsync(project, autosave);

        Assert.True(File.Exists(autosave));
        var loaded = await service.LoadAsync(autosave);
        Assert.Equal(project.Name, loaded.Name);
    }
}
