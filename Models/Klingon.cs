using System;
using BlazorVanillaServer.Core;
using Orleans.Concurrency;

namespace BlazorVanillaServer.Models;

[Immutable, Serializable]
public record class Klingon(
    Guid Key,
    string Title,
    Location Position,
    Location Destination,
    Guid OwnerKey,
    DateTime? Timestamp = null);
