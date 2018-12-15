using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class Rend : DamageOverTimeAction
    {
        public Rend(int duration, int damagePerTurn) : base(
            icon: SkillIcon.BasicAttack, //TODO Add Rend Icon
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