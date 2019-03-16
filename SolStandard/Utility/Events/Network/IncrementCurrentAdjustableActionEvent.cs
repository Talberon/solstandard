using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class IncrementCurrentAdjustableActionEvent : NetworkEvent
    {
        private readonly int value;

        public IncrementCurrentAdjustableActionEvent(int value)
        {
            this.value = value;
        }
        
        public override void Continue()
        {
            GameContext.GameMapContext.IncrementCurrentAdjustableAction(value);
            Complete = true;
        }
    }
}