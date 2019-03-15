using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    public class ResetCursorToNextUnitEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.GameMapContext.ResetCursorToNextUnitOnTeam();
            GameContext.MapCamera.CenterCameraToCursor();
            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}