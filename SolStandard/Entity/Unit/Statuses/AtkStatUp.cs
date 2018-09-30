using SolStandard.HUD.Window.Content;

namespace SolStandard.Entity.Unit.Statuses
{
    public class AtkStatUp : StatusEffect
    {
        private readonly int atkModifier;

        public AtkStatUp(int turnDuration, int atkModifier) : base(
            statusIcon: new RenderBlank(),
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