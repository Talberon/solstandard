using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class AtkRangeStatUp : StatusEffect
    {
        private readonly int atkRangeModifier;

        public AtkRangeStatUp(int turnDuration, int atkRangeModifier) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.AtkRangeUp, GameDriver.CellSizeVector),
            name: UnitStatistics.Abbreviation[Stats.AtkRange] + " Up!",
            description: "Increased attack range.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.atkRangeModifier = atkRangeModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            int[] atkRange = target.Stats.CurrentAtkRange;

            //Add +1 to end of ranges
            for (int range = 1; range <= atkRangeModifier; range++)
            {
                int extraRange = target.Stats.CurrentAtkRange.Max() + range;
                target.Stats.CurrentAtkRange = atkRange.Concat(new[] {extraRange}).ToArray();
            }

            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override void RemoveEffect(GameUnit target)
        {
            int[] atkRange = target.Stats.CurrentAtkRange;

            //Remove the last range
            target.Stats.CurrentAtkRange = atkRange.Take(atkRange.Length - atkRangeModifier).ToArray();
        }
    }
}