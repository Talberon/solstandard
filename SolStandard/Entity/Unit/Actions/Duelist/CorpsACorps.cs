using SolStandard.Entity.Unit.Actions.Lancer;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Duelist
{
    public class CorpsACorps : Charge
    {
        public CorpsACorps(int chargeDistance) : base(
            //TODO New Icon
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, GameDriver.CellSizeVector),
            skillName: "Corps-a-corps",
            chargeDistance: chargeDistance
        )
        {
        }
    }
}