using System;
using System.Threading.Tasks;
using BlazorVanillaServer.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace BlazorVanillaServer.Grains;

public class KlingonGrain : Grain, IKlingonGrain
{
    private readonly ILogger<KlingonGrain> _logger;
    private readonly IPersistentState<State> _state;

    private string GrainType => nameof(KlingonGrain);
    private Guid GrainKey => this.GetPrimaryKey();

    public KlingonGrain(
        ILogger<KlingonGrain> logger,
        [PersistentState("State")] IPersistentState<State> state)
    {
        _logger = logger;
        _state = state;
    }

    public override Task OnActivateAsync()
    {
        RegisterTimer(
            _ => Move(),
            null,
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(10));

        return base.OnActivateAsync();
    }

    private async Task Move()
    {
        var ownerKey = _state.State.Item.OwnerKey;
        var sectorGrain = GrainFactory.GetGrain<ISectorGrain>(ownerKey);
        var stepForward = await sectorGrain.GetStepForward(_state.State.Item.Position, _state.State.Item.Destination);
        if (stepForward != _state.State.Item.Destination)
        {
            var updated = _state.State.Item with { Position = stepForward, Timestamp = DateTime.UtcNow };
            await SetAsync(updated);
        }
    }

    public Task<Klingon?> GetAsync() => Task.FromResult(_state.State.Item);

    public async Task SetAsync(Klingon item)
    {
        // Ensure the key is consistent
        if (item.Key != GrainKey)
        {
            throw new InvalidOperationException();
        }

        // Save the item
        _state.State.Item = item;
        await _state.WriteStateAsync();

        // Register the item with its owner list
        await GrainFactory.GetGrain<ISectorGrain>(item.OwnerKey)
            .RegisterAsync(item.Key);

        // For sample debugging
        _logger.LogInformation(
            "{@GrainType} {@GrainKey} now contains {@Item}",
            GrainType, GrainKey, item);

        // Notify listeners - best effort only
        GetStreamProvider("SMS").GetStream<KlingonNotification>(item.OwnerKey, nameof(IKlingonGrain))
            .OnNextAsync(new KlingonNotification(item.Key, item))
            .Ignore();
    }

    public async Task ClearAsync()
    {
        // Fast path for already cleared state
        if (_state.State.Item is null) return;

        // Hold on to the keys
        var itemKey = _state.State.Item.Key;
        var ownerKey = _state.State.Item.OwnerKey;

        // Unregister from the registry
        await GrainFactory.GetGrain<ISectorGrain>(ownerKey)
            .UnregisterAsync(itemKey);

        // Clear the state
        await _state.ClearStateAsync();

        // For sample debugging
        _logger.LogInformation(
            "{@GrainType} {@GrainKey} is now cleared",
            GrainType, GrainKey);

        // Notify listeners - best effort only
        GetStreamProvider("SMS").GetStream<KlingonNotification>(ownerKey, nameof(IKlingonGrain))
            .OnNextAsync(new KlingonNotification(itemKey, null))
            .Ignore();

        // No need to stay alive anymore
        DeactivateOnIdle();
    }

    public class State
    {
        public Klingon? Item { get; set; }
    }
}
