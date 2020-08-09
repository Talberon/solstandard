using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class LuckStatUp : StatusEffect
    {
        private readonly int luckModifier;

        public LuckStatUp(int turnDuration, int luckModifier) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Luck, GameDriver.CellSizeVector),
            name: UnitStatistics.Abbreviation[Stats.Luck] + " Up! <+" + luckModifier + ">",
            description: "Increased luck.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.luckModifier = luckModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.LuckModifier += luckModifier;

            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.LuckModifier -= luckModifier;
        }
    }
}