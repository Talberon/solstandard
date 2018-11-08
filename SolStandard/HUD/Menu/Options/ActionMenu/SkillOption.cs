using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.ActionMenu
{
    public class SkillOption : MenuOption
    {
        public UnitAction Action { get; private set; }

        public SkillOption(Color windowColor, UnitAction action) : base(
            new WindowContentGrid(
                new[,]
                {
                    {
                        action.Icon,
                        new RenderText(AssetManager.WindowFont, action.Name),
                    }
                },
                1
            ), windowColor)
        {
            Action = action;
        }

        public override void Execute()
        {
            Action.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            GameContext.ActiveUnit.ArmUnitSkill(Action);
        }

        public override IRenderable Clone()
        {
            return new SkillOption(Color, Action);
        }
    }
}