using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class IncrementCurrentAdjustableActionEvent : IEvent
    {
        private readonly int value;
        public bool Complete { get; private set; }

        public IncrementCurrentAdjustableActionEvent(int value)
        {
            this.value = value;
        }
        
        public void Continue()
        {
            GameContext.GameMapContext.IncrementCurrentAdjustableAction(value);
            Complete = true;
        }
    }
}