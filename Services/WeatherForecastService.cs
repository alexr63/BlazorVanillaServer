using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BlazorVanillaServer.Grains;
using BlazorVanillaServer.Models;
using Orleans;

namespace BlazorVanillaServer.Services;

public class WeatherForecastService
{
    private readonly IClusterClient _client;

    public WeatherForecastService(IClusterClient client)
    {
        _client = client;
    }

    public Task<ImmutableArray<WeatherInfo>> GetForecastAsync() =>
        _client.GetGrain<IWeatherGrain>(Guid.Empty).GetForecastAsync();
}
