using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    public class ResolveTurnEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.GameMapContext.ResolveTurn();
            AssetManager.MapUnitSelectSFX.Play();
            Complete = true;
        }
    }
}