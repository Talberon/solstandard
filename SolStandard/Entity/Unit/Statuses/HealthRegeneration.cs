using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class HealthRegeneration : StatusEffect
    {
        private readonly int healthModifier;

        public HealthRegeneration(int turnDuration, int healthModifier) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.HpUp,
                GameDriver.CellSizeVector),
            name: UnitStatistics.Abbreviation[Stats.Hp] + " Regen! <+" + healthModifier + "/turn>",
            description: "Increased defensive power.",
            turnDuration: turnDuration,
            hasNotification: true,
            canCleanse: false
        )
        {
            this.healthModifier = healthModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            target.RecoverHP(healthModifier);

            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                target.Id + " regenerates [" + healthModifier + "] " + UnitStatistics.Abbreviation[Stats.Hp] + "!",
                50
            );

            AssetManager.SkillBuffSFX.Play();
        }

        public override void RemoveEffect(GameUnit target)
        {
            //Do nothing
        }
    }
}