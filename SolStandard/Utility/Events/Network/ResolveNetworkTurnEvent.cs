using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ResolveNetworkTurnEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.WorldContext.ResolveTurn();
            AssetManager.MapUnitSelectSFX.Play();
            Complete = true;
        }
    }
}