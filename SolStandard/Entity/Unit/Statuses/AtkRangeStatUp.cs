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
            for (int i = 0; i < GameContext.ActiveUnit.Stats.AtkRange.Length; i++)
            {
                GameContext.ActiveUnit.Stats.AtkRange[i] += atkRangeModifier;
            }
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void RemoveEffect(GameUnit target)
        {
            for (int i = 0; i < GameContext.ActiveUnit.Stats.AtkRange.Length; i++)
            {
                GameContext.ActiveUnit.Stats.AtkRange[i] -= atkRangeModifier;
            }
        }
    }
}