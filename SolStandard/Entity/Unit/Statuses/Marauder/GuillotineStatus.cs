using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Lancer;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Statuses.Marauder
{
    public class GuillotineStatus : StatusEffect, ICombatProc
    {
        private readonly int healPercentage;

        public GuillotineStatus(IRenderable icon, int turnDuration, int healPercentage) : base(
            statusIcon: icon,
            name: "Guillotine!",
            description: "Recovers " + UnitStatistics.Abbreviation[Stats.Hp] + " if opponent is defeated.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.healPercentage = healPercentage;
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
            //Do nothing
        }

        public void OnBlock(GameUnit damageDealer, GameUnit target)
        {
            //Do nothing
        }

        public void OnDamage(GameUnit damageDealer, GameUnit target)
        {
        }

        public void OnCombatEnd(GameUnit attacker, GameUnit defender)
        {
            //Remove status
            attacker.StatusEffects.RemoveAll(effect => effect == this);

            if (defender.IsAlive || !attacker.IsAlive) return;

            int missingAttackerHP = attacker.Stats.MaxHP - attacker.Stats.CurrentHP;
            int hpToHeal = Execute.ApplyPercentageRoundedUp(missingAttackerHP, healPercentage);
            GlobalEventQueue.QueueSingleEvent(new RegenerateHealthEvent(attacker, hpToHeal));
        }
    }
}