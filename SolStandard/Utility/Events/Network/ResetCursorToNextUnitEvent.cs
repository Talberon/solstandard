using System;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ResetCursorToNextUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.ResetCursorToNextUnitOnTeam();
            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}