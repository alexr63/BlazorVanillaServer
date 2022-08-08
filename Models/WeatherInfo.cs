using System;
using Orleans.Concurrency;

namespace BlazorVanillaServer.Models;

[Immutable, Serializable]
public record class WeatherInfo(
    DateTime Date,
    int TemperatureC,
    string Summary,
    int TemperatureF);
