using SolStandard.Containers.Components.World;
using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class EffectTilesStartOfRoundEvent : IEvent
    {
        private readonly IEvent callback;
        public bool Complete { get; private set; }

        public EffectTilesStartOfRoundEvent(IEvent callback = null)
        {
            this.callback = callback;
        }

        public void Continue()
        {
            //IMPORTANT Do not allow tiles that have been triggered to trigger again or the risk of soft-locking via infinite triggers can occur
            WorldContext.TriggerEffectTiles(EffectTriggerTime.StartOfRound, false);
            Complete = true;
            if (callback is object) GlobalEventQueue.QueueSingleEvent(callback);
        }
    }
}