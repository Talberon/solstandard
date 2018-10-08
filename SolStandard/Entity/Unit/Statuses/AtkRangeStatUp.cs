using System;
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
            name: "Range Up!",
            description: "Increased attack range.",
            turnDuration: turnDuration
        )
        {
            this.atkRangeModifier = atkRangeModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            int extraRange = GameContext.ActiveUnit.Stats.AtkRange.Max() + 1;
            int[] atkRange = GameContext.ActiveUnit.Stats.AtkRange;

            //Add +1 to end of ranges
            GameContext.ActiveUnit.Stats.AtkRange = atkRange.Concat(new[] {extraRange}).ToArray();
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void RemoveEffect(GameUnit target)
        {
            int[] atkRange = GameContext.ActiveUnit.Stats.AtkRange;

            //Remove the last range
            GameContext.ActiveUnit.Stats.AtkRange = atkRange.Take(atkRange.Length - 1).ToArray();
        }
    }
}