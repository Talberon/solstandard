using Microsoft.Xna.Framework;

namespace SolStandard.Entity.Unit.Statuses
{
    public class LuckStatUp : StatusEffect
    {
        private readonly int luckModifier;

        public LuckStatUp(int turnDuration, int luckModifier) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Luck, new Vector2(GameDriver.CellSize)),
            name: UnitStatistics.Abbreviation[Stats.Luck] + " Up!",
            description: "Increased luck.",
            turnDuration: turnDuration
        )
        {
            this.luckModifier = luckModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            target.Stats.Luck += luckModifier;
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void RemoveEffect(GameUnit target)
        {
            target.Stats.Luck -= luckModifier;
        }
    }
}