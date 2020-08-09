using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ResetCursorToNextUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.WorldContext.ResetCursorToNextUnitOnTeam();
            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}