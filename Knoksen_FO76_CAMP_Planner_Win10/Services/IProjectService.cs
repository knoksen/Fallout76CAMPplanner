using System.Threading;
using System.Threading.Tasks;

namespace FO76CampPlanner;

public interface IProjectService
{
    Task<PlannerProject> LoadAsync(string path, CancellationToken ct = default);
    Task SaveAsync(PlannerProject project, string path, CancellationToken ct = default);
    Task<string> GetAutosavePathAsync(string projectPath);
    Task AutosaveAsync(PlannerProject project, string autosavePath, CancellationToken ct = default);
}
