using System;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ResetCursorToPreviousUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.ResetCursorToPreviousUnitOnTeam();
            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}