using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DeployUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.DeploymentContext.TryDeployUnit();
            Complete = true;
        }
    }
}