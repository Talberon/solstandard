using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DraftSelectNextUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.DeploymentContext.SelectNextUnit();
            Complete = true;
        }
    }
}