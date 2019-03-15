using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DeployUnitEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.DeploymentContext.TryDeployUnit();
            Complete = true;
        }
    }
}