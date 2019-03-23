using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    public class ResetCursorToActiveUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.ResetCursorToActiveUnit();
            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}