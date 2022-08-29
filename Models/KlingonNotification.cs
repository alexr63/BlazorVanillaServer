using System;
using Orleans.Concurrency;

namespace BlazorVanillaServer.Models;

[Immutable, Serializable]
public record class KlingonNotification(
    Guid ItemKey,
    Klingon? Item = null);