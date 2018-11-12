using SolStandard.Containers.Contexts;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class DamageOverTimeStatus : StatusEffect
    {
        private readonly int damage;
        private readonly string applyMessage;

        public DamageOverTimeStatus(IRenderable statusIcon, int turnDuration, int damage, string applyMessage) : base(
            statusIcon: statusIcon,
            name: "Damage Over Time [" + damage + "] DMG",
            description: "Deals damage to the afflicted at the beginning of each turn.",
            turnDuration: turnDuration
        )
        {
            this.damage = damage;
            this.applyMessage = applyMessage;
        }

        public override void ApplyEffect(GameUnit target)
        {
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
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

            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                target.Id + " takes [" + damage + "] Damage!",
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