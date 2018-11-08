using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class TriggerEntityEvent : IEvent
    {
        private readonly ITriggerable triggerable;

        public TriggerEntityEvent(ITriggerable triggerable)
        {
            this.triggerable = triggerable;
        }
        
        public bool Complete { get; private set; }
        public void Continue()
        {
            triggerable.Trigger();
            Complete = true;
        }
    }
}