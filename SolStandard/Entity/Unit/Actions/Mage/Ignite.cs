using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Mage
{
    public class Ignite : DamageOverTimeAction
    {
        public Ignite(int duration, int damagePerTurn) : base(
            icon: SkillIcon.Ignite,
            name: "Ignite",
            duration: duration,
            damagePerTurn: damagePerTurn,
            range: new[] {1, 2},
            toastMessage: "Burning!"
        )
        {
        }
    }
}