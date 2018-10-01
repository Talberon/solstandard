using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class CastBuffEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly StatusEffect statusEffect;

        public CastBuffEvent(ref GameUnit targetUnit, StatusEffect statusEffect)
        {
            this.targetUnit = targetUnit;
            this.statusEffect = statusEffect;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            AssetManager.SkillBuffSFX.Play();
            targetUnit.AddStatusEffect(statusEffect);
            Complete = true;
        }
    }
}