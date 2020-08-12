using SolStandard.Containers.Components.Global;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class DamageOverTimeStatus : StatusEffect
    {
        private readonly int damage;
        private readonly string applyMessage;
        private readonly string abilityName;

        public DamageOverTimeStatus(IRenderable statusIcon, int turnDuration, int damage, string applyMessage,
            string abilityName) : base(
            statusIcon: statusIcon,
            name: abilityName + " <" + damage + " DMG/turn>",
            description: "Deals damage to the afflicted at the beginning of each turn.",
            turnDuration: turnDuration,
            hasNotification: true,
            canCleanse: true
        )
        {
            this.damage = damage;
            this.applyMessage = applyMessage;
            this.abilityName = abilityName;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                applyMessage,
                50
            );
        }


        protected override void ExecuteEffect(GameUnit target)
        {
            for (int i = 0; i < damage; i++)
            {
                target.DamageUnit();
            }

            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                target.Id + " takes [" + damage + "] Damage from " + abilityName + "!",
                50
            );
            AssetManager.CombatDamageSFX.Play();
        }

        public override void RemoveEffect(GameUnit target)
        {
            //Do nothing
        }
    }
}