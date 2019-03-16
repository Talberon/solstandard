using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class ExecuteActionEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.ExecuteAction();
            Complete = true;
        }
    }
}