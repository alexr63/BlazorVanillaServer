using System.Collections.ObjectModel;
using System;

namespace BlazorVanillaServer.Models
{
    public class EnterpriseKeyedCollection : KeyedCollection<Guid, Enterprise>
    {
        protected override Guid GetKeyForItem(Enterprise item) => item.Key;
    }
}
