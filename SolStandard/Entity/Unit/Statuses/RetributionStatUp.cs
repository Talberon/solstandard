using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class RetributionStatUp : StatusEffect
    {
        private readonly int retModifier;

        public RetributionStatUp(int turnDuration, int retModifier) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Retribution, new Vector2(GameDriver.CellSize)),
            name: UnitStatistics.Abbreviation[Stats.Retribution] + " Up!",
            description: "Increased retribution.",
            turnDuration: turnDuration
        )
        {
            this.retModifier = retModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.RetModifier += retModifier;

            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                target.Id + " has status " + Name,
                50
            );
            AssetManager.MapUnitCancelSFX.Play();
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.RetModifier -= retModifier;
        }
    }
}