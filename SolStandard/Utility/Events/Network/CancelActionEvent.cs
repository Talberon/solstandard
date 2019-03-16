using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class CancelActionEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.CancelTargetAction();
            Complete = true;
        }
    }
}