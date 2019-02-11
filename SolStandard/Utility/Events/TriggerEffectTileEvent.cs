using SolStandard.Containers.Contexts;
using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class TriggerEffectTileEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly IEffectTile effectTile;
        private readonly EffectTriggerTime effectTriggerTime;
        private readonly int delayTime;
        private int delayTicker;

        public TriggerEffectTileEvent(IEffectTile effectTile, EffectTriggerTime effectTriggerTime, int delayTime = 50)
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
            }

            delayTicker--;

            if (delayTicker > 0) return;

            Complete = true;
        }
    }
}