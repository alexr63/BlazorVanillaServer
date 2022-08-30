using System;
using BlazorVanillaServer.Core;
using Orleans.Concurrency;

namespace BlazorVanillaServer.Models;

[Immutable, Serializable]
public record class Enterprise(
    Guid Key,
    string Title,
    Location Position,
    Location Destination,
    Guid OwnerKey,
    DateTime? Timestamp = null);
