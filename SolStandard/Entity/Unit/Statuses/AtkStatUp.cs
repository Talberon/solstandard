using Microsoft.Xna.Framework;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class AtkStatUp : StatusEffect
    {
        private readonly int atkModifier;

        public AtkStatUp(int turnDuration, int atkModifier) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.AtkUp, new Vector2(32)),
            name: "ATK Up!",
            description: "Increased attack power.",
            turnDuration: turnDuration
        )
        {
            this.atkModifier = atkModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            target.Stats.Atk += atkModifier;
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void RemoveEffect(GameUnit target)
        {
            target.Stats.Atk -= atkModifier;
        }
    }
}