using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class BlkStatUp : StatusEffect
    {
        private readonly int pointsToIncrease;

        public BlkStatUp(int turnDuration, int pointsToIncrease, string name = null) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Block, GameDriver.CellSizeVector),
            name: name ?? UnitStatistics.Abbreviation[Stats.Block] + " Up! <+" + pointsToIncrease + ">",
            description: "Increased damage mitigation.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.pointsToIncrease = pointsToIncrease;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.BlkModifier += pointsToIncrease;
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.BlkModifier -= pointsToIncrease;
        }
    }
}