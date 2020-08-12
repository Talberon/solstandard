using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
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

        public ActionOption(Color windowColor, UnitAction action) : this(action.Name, windowColor, action)
        {
        }

        protected ActionOption(string actionName, Color windowColor, UnitAction action) : base(
            GenerateActionContent(action.Icon, actionName, action.FreeAction),
            windowColor
        )
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
                            freeAction ? GlobalContext.PositiveColor : Color.White)
                    }
                }
            );
        }

        public override void Refresh()
        {
            LabelContent = GenerateActionContent(Action.Icon, Action.Name, Action.FreeAction);
            base.Refresh();
        }

        public override void Execute()
        {
            Action.GenerateActionGrid(GlobalContext.ActiveUnit.UnitEntity.MapCoordinates);
            GlobalContext.ActiveUnit.ArmUnitSkill(Action);
        }

        public override IRenderable Clone()
        {
            return new ActionOption(DefaultColor, Action);
        }
    }
}