using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ResetCursorToActiveUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.GameMapContext.ResetCursorToActiveUnit();
            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}