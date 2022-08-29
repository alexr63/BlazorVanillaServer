using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BlazorVanillaServer.Core;
using Orleans;
using Orleans.Runtime;
using RogueSharp;

namespace BlazorVanillaServer.Grains;

public class SectorGrain : Grain, ISectorGrain
{
    private readonly IPersistentState<State> _state;

    private readonly SectorMap _map;

    public Task<SectorMap> GetMapAsync() => Task.FromResult(_map);

    public SectorGrain(
        [PersistentState("State")] IPersistentState<State> state)
    {
        _state = state;
        _map = GetSectorMap();
    }

    public async Task RegisterAsync(Guid itemKey)
    {
        _state.State.Items.Add(itemKey);
        await _state.WriteStateAsync();
    }

    public async Task UnregisterAsync(Guid itemKey)
    {
        _state.State.Items.Remove(itemKey);
        await _state.WriteStateAsync();
    }

    public Task<ImmutableArray<Guid>> GetAllAsync() =>
        Task.FromResult(ImmutableArray.CreateRange(_state.State.Items));

    public class State
    {
        public HashSet<Guid> Items { get; set; } = new();
    }

    private SectorMap GetSectorMap()
    {
        var sectorMap = new SectorMap(7, 8);
        sectorMap.Clear();
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                sectorMap.SetCellProperties(i, j, true, true);
            }
        }

        sectorMap.SetCellProperties(0, 2, false, false);
        sectorMap.SetCellProperties(1, 6, false, false);
        sectorMap.SetCellProperties(2, 1, false, false);
        sectorMap.SetCellProperties(3, 5, false, false);
        sectorMap.SetCellProperties(3, 7, false, false);
        sectorMap.SetCellProperties(4, 5, false, false);
        sectorMap.SetCellProperties(5, 0, false, false);
        sectorMap.SetCellProperties(6, 4, false, false);
        sectorMap.SetCellProperties(5, 6, false, false);

        return sectorMap;
    }

    Task<Location> ISectorGrain.GetStepForward(Location source, Location destination)
    {
        var pathFinder = new PathFinder(_map, 1.41);
        var path = pathFinder.ShortestPath(new Cell(source.X, source.Y, false, false, true), new Cell(destination.X, destination.Y, false, false, true));
        var stepForward = path.StepForward();
        return Task.FromResult(new Location(stepForward.X, stepForward.Y));
    }
}
