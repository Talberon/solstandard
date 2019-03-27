using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.ActionMenu
{
    public class ActionOption : MenuOption
    {
        public UnitAction Action { get; private set; }

        public ActionOption(Color windowColor, UnitAction action) : base(
            new WindowContentGrid(
                new[,]
                {
                    {
                        action.Icon,
                        new RenderText(AssetManager.WindowFont, action.Name, action.FreeAction ? GameContext.PositiveColor : Color.White),
                    }
                },
                1
            ), windowColor)
        {
            Action = action;
        }

        public override void Refresh()
        {
            LabelContent = new WindowContentGrid(
                new[,]
                {
                    {
                        Action.Icon,
                        new RenderText(AssetManager.WindowFont, Action.Name),
                    }
                },
                1
            );

            base.Refresh();
        }

        public override void Execute()
        {
            Action.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            GameContext.ActiveUnit.ArmUnitSkill(Action);
        }

        public override IRenderable Clone()
        {
            return new ActionOption(DefaultColor, Action);
        }
    }
}