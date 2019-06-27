using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class RetributionStatDown : StatusEffect
    {
        private readonly int retToReduce;

        public RetributionStatDown(int turnDuration, int retToReduce) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Retribution, GameDriver.CellSizeVector),
            name: UnitStatistics.Abbreviation[Stats.Retribution] + " Down! <-" + retToReduce + ">",
            description: "Decreased retribution.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: true
        )
        {
            this.retToReduce = retToReduce;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.RetModifier -= retToReduce;

            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.RetModifier += retToReduce;
        }
    }
}