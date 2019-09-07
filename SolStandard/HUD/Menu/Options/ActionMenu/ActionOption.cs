using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.ActionMenu
{
    public class ActionOption : MenuOption, IOptionDescription
    {
        public UnitAction Action { get; }
        public IRenderable Description => Action.Description;

        public ActionOption(Color windowColor, UnitAction action) : base(
            GenerateActionContent(action.Icon, action.Name, action.FreeAction), windowColor)
        {
            Action = action;
        }

        public static WindowContentGrid GenerateActionContent(IRenderable icon, string name, bool freeAction)
        {
            return new WindowContentGrid(
                new[,]
                {
                    {
                        icon,
                        new RenderText(AssetManager.WindowFont, name,
                            freeAction ? GameContext.PositiveColor : Color.White)
                    }
                },
                1
            );
        }

        public override void Refresh()
        {
            LabelContent = GenerateActionContent(Action.Icon, Action.Name, Action.FreeAction);
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