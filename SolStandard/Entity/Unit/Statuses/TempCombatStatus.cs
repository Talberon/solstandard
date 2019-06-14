using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions;

namespace SolStandard.Entity.Unit.Statuses
{
    public class TempCombatStatus : StatusEffect, ICombatProc
    {
        private readonly StatusEffect temporaryEffect;

        public TempCombatStatus(StatusEffect temporaryEffect) : base(
            statusIcon: temporaryEffect.StatusIcon,
            name: temporaryEffect.Name + " (Temp)",
            description: temporaryEffect.Description + " (Temp)",
            turnDuration: 0,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.temporaryEffect = temporaryEffect;
        }

        public override void ApplyEffect(GameUnit target)
        {
            temporaryEffect.ApplyEffect(target);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            temporaryEffect.RemoveEffect(target);
        }

        public void OnCombatStart(GameUnit attacker, GameUnit defender)
        {
            //Do nothing
        }

        public void OnBlock(GameUnit damageDealer, GameUnit target)
        {
            //Do nothing
        }

        public void OnDamage(GameUnit damageDealer, GameUnit target)
        {
            //Do nothing
        }

        public void OnCombatEnd(GameUnit attacker, GameUnit defender)
        {
            GameContext.ActiveUnit.StatusEffects.RemoveAll(effect => effect == this);
        }
    }
}