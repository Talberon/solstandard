using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class PoisonTip : DamageOverTimeAction
    {
        public PoisonTip(int duration, int damagePerTurn) : base(
            icon: SkillIcon.PoisonTip,
            name: "Poison Tip",
            duration: duration,
            damagePerTurn: damagePerTurn,
            range: new[] {1},
            toastMessage: "Poisoned!"
        )
        {
        }
    }
}