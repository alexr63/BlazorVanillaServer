using System;
using Orleans.Concurrency;

namespace BlazorVanillaServer.Models;

[Immutable, Serializable]
public record class EnterpriseNotification(
    Guid ItemKey,
    Enterprise? Item = null);