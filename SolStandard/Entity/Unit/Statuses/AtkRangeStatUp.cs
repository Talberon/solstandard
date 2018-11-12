using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class AtkRangeStatUp : StatusEffect
    {
        private readonly int atkRangeModifier;

        public AtkRangeStatUp(int turnDuration, int atkRangeModifier) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.AtkRangeUp, new Vector2(32)),
            name: UnitStatistics.Abbreviation[Stats.AtkRange] + " Up!",
            description: "Increased attack range.",
            turnDuration: turnDuration
        )
        {
            this.atkRangeModifier = atkRangeModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            int[] atkRange = GameContext.ActiveUnit.Stats.AtkRange;

            //Add +1 to end of ranges
            for (int range = 1; range <= atkRangeModifier; range++)
            {
                int extraRange = GameContext.ActiveUnit.Stats.AtkRange.Max() + range;
                GameContext.ActiveUnit.Stats.AtkRange = atkRange.Concat(new[] {extraRange}).ToArray();
            }

            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
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
            int[] atkRange = GameContext.ActiveUnit.Stats.AtkRange;

            //Remove the last range
            GameContext.ActiveUnit.Stats.AtkRange = atkRange.Take(atkRange.Length - atkRangeModifier).ToArray();
        }
    }
}