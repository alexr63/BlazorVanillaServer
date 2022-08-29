using System.Threading.Tasks;
using BlazorVanillaServer.Models;
using Orleans;

namespace BlazorVanillaServer.Grains;

public interface IKlingonGrain : IGrainWithGuidKey
{
    Task SetAsync(Klingon item);

    Task ClearAsync();

    Task<Klingon?> GetAsync();
}
