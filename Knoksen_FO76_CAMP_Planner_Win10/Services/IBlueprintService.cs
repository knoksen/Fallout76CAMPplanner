using System.Threading;
using System.Threading.Tasks;

namespace FO76CampPlanner;

public interface IBlueprintService
{
    Task<BlueprintModule> LoadAsync(string path, CancellationToken ct = default);
}
