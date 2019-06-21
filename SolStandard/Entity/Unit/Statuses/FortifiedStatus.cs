using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class FortifiedStatus : StatusEffect
    {
        private readonly int pointsToTrade;

        public FortifiedStatus(int turnDuration, int pointsToTrade) : base(
            statusIcon: SkillIconProvider.GetSkillIcon(SkillIcon.Fortify, new Vector2(GameDriver.CellSize)),
            name:
            $"Fortified! <+{pointsToTrade} {UnitStatistics.Abbreviation[Stats.Block]}/-{pointsToTrade} {UnitStatistics.Abbreviation[Stats.Luck]}>",
            description: "Exchange luck for guaranteed damage mitigation.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.pointsToTrade = pointsToTrade;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.BlkModifier += pointsToTrade;
            target.Stats.LuckModifier -= pointsToTrade;
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.BlkModifier -= pointsToTrade;
            target.Stats.LuckModifier += pointsToTrade;
        }
    }
}