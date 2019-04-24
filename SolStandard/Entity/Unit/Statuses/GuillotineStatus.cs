using System;
using System.Collections.Generic;
using System.Linq;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Statuses
{
    public class GuillotineStatus : StatusEffect, ICombatProc
    {
        private readonly bool attackerIsEnraged;

        public GuillotineStatus(IRenderable icon, int turnDuration, bool attackerIsEnraged) : base(
            statusIcon: icon,
            name: "Guillotine!",
            description: "Recovers " + UnitStatistics.Abbreviation[Stats.Hp] + " if opponent is defeated.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.attackerIsEnraged = attackerIsEnraged;
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

            float missingAttackerHP = attacker.Stats.MaxHP - attacker.Stats.CurrentHP;

            int hpToHeal;
            if (attackerIsEnraged)
            {
                const int enragedDenominator = 3;
                hpToHeal = (int) Math.Floor(missingAttackerHP / enragedDenominator);
                List<EnragedStatus> enragedStatuses = attacker.StatusEffects.Where(status => status is EnragedStatus)
                    .Cast<EnragedStatus>().ToList();
                enragedStatuses.ForEach(enrage => enrage.RemoveEffect(attacker));
                attacker.StatusEffects.RemoveAll(status => status is EnragedStatus);
            }
            else
            {
                const int normalDenominator = 4;
                hpToHeal = (int) Math.Floor(missingAttackerHP / normalDenominator);
            }

            GlobalEventQueue.QueueSingleEvent(new RegenerateHealthEvent(attacker, hpToHeal));
        }
    }
}