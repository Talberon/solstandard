using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Statuses;

namespace SolStandard.Utility.Events
{
    public class CastStatusEffectEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly StatusEffect statusEffect;

        public CastStatusEffectEvent(GameUnit targetUnit, StatusEffect statusEffect)
        {
            this.targetUnit = targetUnit;
            this.statusEffect = statusEffect;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            targetUnit.AddStatusEffect(statusEffect);
            Complete = true;
        }
    }
}