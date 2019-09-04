using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Rogue
{
    public class Rend : DamageOverTimeAction
    {
        public Rend(int duration, int damagePerTurn) : base(
            icon: SkillIcon.Rend,
            name: "Rend",
            duration: duration,
            damagePerTurn: damagePerTurn,
            range: new[] {1},
            toastMessage: "Bleeding!"
        )
        {
        }
    }
}