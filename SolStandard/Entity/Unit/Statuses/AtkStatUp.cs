using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class AtkStatUp : StatusEffect
    {
        private readonly int pointsToIncrease;

        public AtkStatUp(int turnDuration, int pointsToIncrease, string name = null) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Atk, GameDriver.CellSizeVector),
            name: name ?? UnitStatistics.Abbreviation[Stats.Atk] + " Up! <+" + pointsToIncrease + ">",
            description: "Increased attack power.",
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
            target.Stats.AtkModifier += pointsToIncrease;
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.AtkModifier -= pointsToIncrease;
        }
    }
}