using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Statuses
{
    public class ExecutionerStatus : StatusEffect, ICombatProc
    {
        public ExecutionerStatus(IRenderable icon, int turnDuration) : base(
            statusIcon: icon,
            name: "Ending an opponent's suffering.",
            description: "Regenerates all " + UnitStatistics.Abbreviation[Stats.Armor] +
                         " at the end of combat if the target is killed and gains an " +
                         UnitStatistics.Abbreviation[Stats.Atk] + " Up buff.",
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
            if (defender.Stats.CurrentHP <= 0)
            {
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitFramesEvent(5));
                eventQueue.Enqueue(new RegenerateArmorEvent(attacker, attacker.Stats.MaxArmor));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new CastStatusEffectEvent(attacker, new AtkStatModifier(1, 2)));
                GlobalEventQueue.QueueEvents(eventQueue);
            }

            //Remove status
            GameContext.ActiveUnit.StatusEffects.RemoveAll(effect => effect == this);
        }
    }
}