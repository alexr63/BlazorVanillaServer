using System;
using System.Buffers;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BlazorVanillaServer.Grains;
using BlazorVanillaServer.Models;
using BlazorVanillaServer.Pages;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;

namespace BlazorVanillaServer.Services;

public class SectorService
{
    private readonly ILogger<SectorService> _logger;
    private readonly IClusterClient _client;

    public SectorService(ILogger<SectorService> logger, IClusterClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<string> GetMapAsync(Guid ownerKey, KlingonKeyedCollection klingonKeyedCollection)
    {
        var map = await _client.GetGrain<ISectorGrain>(ownerKey)
            .GetMapAsync();
        return map.FormatString(klingonKeyedCollection);
    }

    

    public async Task<ImmutableArray<Klingon>> GetAllAsync(Guid ownerKey)
    {
        // get all the todo item keys for this owner
        var itemKeys = await _client.GetGrain<ISectorGrain>(ownerKey)
            .GetAllAsync();

        // fan out to get the individual items from the cluster in parallel
        var tasks = ArrayPool<Task<Klingon?>>.Shared.Rent(itemKeys.Length);
        try
        {
            // issue all individual requests at the same time
            for (var i = 0; i < itemKeys.Length; ++i)
            {
                tasks[i] = _client.GetGrain<IKlingonGrain>(itemKeys[i]).GetAsync();
            }

            // build the result as requests complete
            var result = ImmutableArray.CreateBuilder<Klingon>(itemKeys.Length);
            for (var i = 0; i < itemKeys.Length; ++i)
            {
                var item = await tasks[i];

                // we can get a null result if the individual grain failed to unregister
                // in this case we can finish the job here
                if (item is null)
                {
                    await _client.GetGrain<ISectorGrain>(ownerKey)
                        .UnregisterAsync(itemKeys[i]);
                }

                if (item is not null)
                {
                    result.Add(item);
                }
            }
            return result.ToImmutable();
        }
        finally
        {
            ArrayPool<Task<Klingon?>>.Shared.Return(tasks);
        }
    }

    public Task SetAsync(Klingon item) =>
        _client.GetGrain<IKlingonGrain>(item.Key).SetAsync(item);

    public Task DeleteAsync(Guid itemKey) =>
        _client.GetGrain<IKlingonGrain>(itemKey).ClearAsync();

    public Task<StreamSubscriptionHandle<KlingonNotification>> SubscribeAsync(
        Guid ownerKey, Func<KlingonNotification, Task> action) =>
        _client.GetStreamProvider("SMS")
            .GetStream<KlingonNotification>(ownerKey, nameof(IKlingonGrain))
            .SubscribeAsync(new KlingonObserver(_logger, action));

    private class KlingonObserver : IAsyncObserver<KlingonNotification>
    {
        private readonly ILogger<SectorService> _logger;
        private readonly Func<KlingonNotification, Task> _onNext;

        public KlingonObserver(
            ILogger<SectorService> logger,
            Func<KlingonNotification, Task> action)
        {
            _logger = logger;
            _onNext = action;
        }

        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Task.CompletedTask;
        }

        public Task OnNextAsync(
            KlingonNotification item,
            StreamSequenceToken? token = null) =>
            _onNext(item);
    }
}
