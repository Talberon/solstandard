using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DraftSelectPreviousUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.DeploymentContext.SelectPreviousUnit();
            Complete = true;
        }
    }
}