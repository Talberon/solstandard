using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DeployResetToNextDeploymentTileEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.DeploymentContext.MoveToNextDeploymentTile();
            Complete = true;
        }
    }
}