using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Skills;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.ActionMenu
{
    public class SkillOption : MenuOption
    {
        private readonly UnitSkill skill;

        public SkillOption(Color windowColor, UnitSkill skill) : base(
            windowColor,
            new WindowContentGrid(
                new [,]
                {
                    {
                        skill.Icon,
                        new RenderText(AssetManager.WindowFont, skill.Name),
                    }
                },
                1
            )
        )
        {
            this.skill = skill;
        }

        public override void Execute()
        {
            skill.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            GameContext.ActiveUnit.ArmUnitSkill(skill);
        }
    }
}