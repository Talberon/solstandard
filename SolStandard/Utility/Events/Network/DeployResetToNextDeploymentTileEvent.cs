using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DeployResetToNextDeploymentTileEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.DeploymentContext.MoveToNextDeploymentTile();
            Complete = true;
        }
    }
}