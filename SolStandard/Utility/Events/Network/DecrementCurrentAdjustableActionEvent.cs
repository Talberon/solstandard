using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DecrementCurrentAdjustableActionEvent : IEvent
    {
        private readonly int value;
        public bool Complete { get; private set; }

        public DecrementCurrentAdjustableActionEvent(int value)
        {
            this.value = value;
        }
        
        public void Continue()
        {
            GameContext.GameMapContext.DecrementCurrentAdjustableAction(value);
            Complete = true;
        }
    }
}