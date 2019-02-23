using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Archer
{
    public class PoisonArrow : DamageOverTimeAction
    {
        public PoisonArrow(int duration, int damagePerTurn) : base(
            icon: SkillIcon.PoisonTip,
            name: "Poison Arrow",
            duration: duration,
            damagePerTurn: damagePerTurn,
            range: new[] {2},
            toastMessage: "Poisoned!"
        )
        {
        }
    }
}