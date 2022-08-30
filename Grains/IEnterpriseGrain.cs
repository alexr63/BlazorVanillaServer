using System.Threading.Tasks;
using BlazorVanillaServer.Models;
using Orleans;

namespace BlazorVanillaServer.Grains;

public interface IEnterpriseGrain : IGrainWithGuidKey
{
    Task SetAsync(Enterprise item);

    Task ClearAsync();

    Task<Enterprise?> GetAsync();
}
