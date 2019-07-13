using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class TriggerSingleEffectTileEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly IEffectTile effectTile;
        private readonly EffectTriggerTime effectTriggerTime;
        private readonly int delayTime;
        private int delayTicker;

        public TriggerSingleEffectTileEvent(IEffectTile effectTile, EffectTriggerTime effectTriggerTime,
            int delayTime = 100)
        {
            this.effectTile = effectTile;
            this.effectTriggerTime = effectTriggerTime;
            this.delayTime = delayTime;
            delayTicker = delayTime;
        }

        public void Continue()
        {
            if (delayTicker == delayTime)
            {
                if (!effectTile.Trigger(effectTriggerTime))
                {
                    Complete = true;
                    return;
                }

                effectTile.HasTriggered = true;
            }

            delayTicker--;

            if (delayTicker > 0) return;

            Complete = true;
        }
    }
}