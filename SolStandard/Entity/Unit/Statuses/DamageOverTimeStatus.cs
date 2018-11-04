using SolStandard.Containers.Contexts;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class DamageOverTimeStatus : StatusEffect
    {
        private readonly int damage;

        public DamageOverTimeStatus(IRenderable statusIcon, int turnDuration, int damage) : base(
            statusIcon: statusIcon,
            name: "Damage Over Time",
            description: "Deals damage to the afflicted at the beginning of each turn.",
            turnDuration: turnDuration
        )
        {
            this.damage = damage;
        }

        public override void ApplyEffect(GameUnit target)
        {
            //Do nothing
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

        protected override void RemoveEffect(GameUnit target)
        {
            //Do nothing
        }
    }
}