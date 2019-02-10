using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Statuses;

namespace SolStandard.Utility.Events
{
    public class UpdateStatusEffectEvent : IEvent
    {
        private readonly StatusEffect effect;
        private readonly GameUnit effectedUnit;
        public bool Complete { get; private set; }

        public UpdateStatusEffectEvent(StatusEffect effect, GameUnit effectedUnit)
        {
            this.effect = effect;
            this.effectedUnit = effectedUnit;
        }

        public void Continue()
        {
            effect.UpdateEffect(effectedUnit);
            Complete = true;
        }
    }
}