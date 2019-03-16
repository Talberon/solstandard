using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    public class ResetCursorToNextUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.ResetCursorToNextUnitOnTeam();
            GameContext.MapCamera.CenterCameraToCursor();
            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}