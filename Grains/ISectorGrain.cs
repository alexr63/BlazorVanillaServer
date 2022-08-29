using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BlazorVanillaServer.Core;
using Orleans;

namespace BlazorVanillaServer.Grains;

public interface ISectorGrain : IGrainWithGuidKey
{
    Task RegisterAsync(Guid itemKey);
    Task UnregisterAsync(Guid itemKey);

    Task<ImmutableArray<Guid>> GetAllAsync();
    Task<SectorMap> GetMapAsync();
    Task<Location> GetStepForward(Location source, Location destination);
}
