using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class ExecuteActionEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.GameMapContext.ExecuteAction();
            Complete = true;
        }
    }
}