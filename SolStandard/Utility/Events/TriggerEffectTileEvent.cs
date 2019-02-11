using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class TriggerEffectTileEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly IEffectTile effectTile;
        private readonly EffectTriggerTime effectTriggerTime;

        public TriggerEffectTileEvent(IEffectTile effectTile, EffectTriggerTime effectTriggerTime)
        {
            this.effectTile = effectTile;
            this.effectTriggerTime = effectTriggerTime;
        }

        public void Continue()
        {
            effectTile.Trigger(effectTriggerTime);

            Complete = true;
        }
    }
}