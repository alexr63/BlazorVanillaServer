using BlazorVanillaServer.Core;
using BlazorVanillaServer.Systems;

namespace BlazorVanillaServer.Interfaces
{
    public interface IBehavior
    {
        bool Act(Actor actor, CommandSystem commandSystem);
    }
}
