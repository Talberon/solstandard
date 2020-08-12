using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class RetributionStatUp : StatusEffect
    {
        private readonly int retModifier;

        public RetributionStatUp(int turnDuration, int retModifier) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Retribution, GameDriver.CellSizeVector),
            name: UnitStatistics.Abbreviation[Stats.Retribution] + " Up! <+" + retModifier + ">",
            description: "Increased retribution.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.retModifier = retModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.RetModifier += retModifier;

            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.RetModifier -= retModifier;
        }
    }
}