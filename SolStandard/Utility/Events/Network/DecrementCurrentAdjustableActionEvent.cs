using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class DecrementCurrentAdjustableActionEvent : NetworkEvent
    {
        private readonly int value;
        public DecrementCurrentAdjustableActionEvent(int value)
        {
            this.value = value;
        }
        
        public override void Continue()
        {
            GameContext.GameMapContext.DecrementCurrentAdjustableAction(value);
            Complete = true;
        }
    }
}