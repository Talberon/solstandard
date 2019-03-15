using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DraftSelectNextUnitEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.DeploymentContext.SelectNextUnit();
            Complete = true;
        }
    }
}