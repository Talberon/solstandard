using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
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