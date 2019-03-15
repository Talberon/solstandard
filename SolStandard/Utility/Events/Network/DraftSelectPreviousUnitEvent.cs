using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DraftSelectPreviousUnitEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.DeploymentContext.SelectPreviousUnit();
            Complete = true;
        }
    }
}