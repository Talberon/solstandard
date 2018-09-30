using SolStandard.HUD.Window.Content;

namespace SolStandard.Entity.Unit.Statuses
{
    public class DefStatUp : StatusEffect
    {
        private readonly int defModifier;

        public DefStatUp(int turnDuration, int defModifier) : base(
            statusIcon: new RenderBlank(),
            name: "DEF Up!",
            description: "Increased defensive power.",
            turnDuration: turnDuration
        )
        {
            this.defModifier = defModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            target.Stats.Def += defModifier;
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void RemoveEffect(GameUnit target)
        {
            target.Stats.Def -= defModifier;
        }
    }
}