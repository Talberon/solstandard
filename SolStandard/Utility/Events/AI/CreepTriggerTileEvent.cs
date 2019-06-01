using SolStandard.Entity;

namespace SolStandard.Utility.Events.AI
{
    public class CreepTriggerTileEvent : IEvent
    {
        private readonly ITriggerable targetToTrigger;

        public CreepTriggerTileEvent(ITriggerable targetToTrigger)
        {
            this.targetToTrigger = targetToTrigger;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            if (targetToTrigger.CanTrigger) targetToTrigger.Trigger();
            Complete = true;
        }
    }
}