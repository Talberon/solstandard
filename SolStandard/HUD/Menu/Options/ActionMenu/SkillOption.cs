using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Skills;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.ActionMenu
{
    public class SkillOption : MenuOption
    {
        private readonly UnitAction action;

        public SkillOption(Color windowColor, UnitAction action) : base(
            windowColor,
            new WindowContentGrid(
                new [,]
                {
                    {
                        action.Icon,
                        new RenderText(AssetManager.WindowFont, action.Name),
                    }
                },
                1
            )
        )
        {
            this.action = action;
        }

        public override void Execute()
        {
            action.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            GameContext.ActiveUnit.ArmUnitSkill(action);
        }
    }
}