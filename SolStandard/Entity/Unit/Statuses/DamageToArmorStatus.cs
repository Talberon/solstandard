using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class DamageToArmorStatus : StatusEffect, ICombatProc
    {
        private readonly int damageThreshold;
        private int damageCounter;

        public DamageToArmorStatus(IRenderable icon, int damageThreshold) : base(
            statusIcon: icon,
            name: UnitStatistics.Abbreviation[Stats.Armor] + " Siphon!",
            description:
            $"Recovers {UnitStatistics.Abbreviation[Stats.Armor]} for each {damageThreshold} damage dealt.",
            turnDuration: 0,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.damageThreshold = damageThreshold;
            damageCounter = 0;
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
            damageCounter++;

            if (damageCounter % damageThreshold == 0)
            {
                damageDealer.RecoverArmor(1);
                AssetManager.SkillBuffSFX.Play();
            }
        }

        public void OnCombatEnd(GameUnit attacker, GameUnit defender)
        {
            //Remove status
            GlobalContext.ActiveUnit.StatusEffects.RemoveAll(effect => effect == this);
        }
    }
}