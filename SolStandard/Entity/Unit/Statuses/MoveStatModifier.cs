using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class MoveStatModifier : StatusEffect
    {
        private readonly int pointsToIncrease;
        private const int FastFrameDelay = 8;

        public MoveStatModifier(int turnDuration, int pointsToIncrease, string name = null) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Mv, GameDriver.CellSizeVector),
            name: name ?? UnitStatistics.Abbreviation[Stats.Mv] + " Up! <+" + pointsToIncrease + ">",
            description: "Increased movement distance.",
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
            target.Stats.MvModifier += pointsToIncrease;
            target.UnitEntity.UnitSpriteSheet.SetFrameDelay(FastFrameDelay);
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.MvModifier -= pointsToIncrease;
            target.UnitEntity?.UnitSpriteSheet.ResetFrameDelay();
        }
    }
}