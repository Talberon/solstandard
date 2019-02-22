using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Statuses
{
    public class IgnoreArmorCombatStatus : StatusEffect, ICombatProc
    {
        private bool opponentHasArmor;

        public IgnoreArmorCombatStatus(IRenderable icon, int turnDuration) : base(
            statusIcon: icon,
            name: "Ignore Armor!",
            description: "Ignores opponent's " + UnitStatistics.Abbreviation[Stats.Armor] +
                         " when dealing combat damage.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
        }

        public override void ApplyEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            //Do nothing
        }

        //ICombatProc

        public void OnCombatStart(GameUnit attacker, GameUnit defender)
        {
            opponentHasArmor = defender.Stats.CurrentArmor > 0;
        }

        public void OnBlock(GameUnit damageDealer, GameUnit target)
        {
            //Do nothing
        }

        public void OnDamage(GameUnit damageDealer, GameUnit target)
        {
            if (!opponentHasArmor) return;
            target.RecoverArmor(1);
            target.DamageUnit(ignoreArmor: true);
        }

        public void OnCombatEnd(GameUnit attacker, GameUnit defender)
        {
            //Remove status
            GameContext.ActiveUnit.StatusEffects.RemoveAll(effect => effect == this);
        }
    }
}