using System;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ResolveTurnEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.ResolveTurn();
            AssetManager.MapUnitSelectSFX.Play();
            Complete = true;
        }
    }
}