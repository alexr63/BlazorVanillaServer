using System.Collections.ObjectModel;
using System;

namespace BlazorVanillaServer.Models
{
    public class KlingonKeyedCollection : KeyedCollection<Guid, Klingon>
    {
        protected override Guid GetKeyForItem(Klingon item) => item.Key;
    }
}
