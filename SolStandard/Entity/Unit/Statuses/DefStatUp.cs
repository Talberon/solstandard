using Microsoft.Xna.Framework;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class DefStatUp : StatusEffect
    {
        private readonly int defModifier;

        public DefStatUp(int turnDuration, int defModifier) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.DefUp, new Vector2(32)),
            name: "DEF Up!",
            description: "Increased defensive power.",
            turnDuration: turnDuration
        )
        {
            this.defModifier = defModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            target.Stats.Armor += defModifier;
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void RemoveEffect(GameUnit target)
        {
            target.Stats.Armor -= defModifier;
        }
    }
}