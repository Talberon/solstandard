using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class DecrementCurrentAdjustableActionEvent : NetworkEvent
    {
        private readonly int value;
        public DecrementCurrentAdjustableActionEvent(int value)
        {
            this.value = value;
        }
        
        public override void Continue()
        {
            GlobalContext.WorldContext.DecrementCurrentAdjustableAction(value);
            Complete = true;
        }
    }
}