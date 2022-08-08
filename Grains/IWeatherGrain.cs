using System.Collections.Immutable;
using System.Threading.Tasks;
using BlazorVanillaServer.Models;
using Orleans;

namespace BlazorVanillaServer.Grains;

public interface IWeatherGrain : IGrainWithGuidKey
{
    Task<ImmutableArray<WeatherInfo>> GetForecastAsync();
}
