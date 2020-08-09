using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ResetCursorToPreviousUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.GameMapContext.ResetCursorToPreviousUnitOnTeam();
            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}