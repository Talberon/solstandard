using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class CancelActionEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.GameMapContext.CancelTargetAction();
            Complete = true;
        }
    }
}