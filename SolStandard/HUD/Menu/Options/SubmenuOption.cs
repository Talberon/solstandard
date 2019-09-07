using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.Utility;

namespace SolStandard.HUD.Menu.Options
{
    public class SubmenuOption : MenuOption
    {
        private readonly IMenu submenu;
        private readonly IRenderable icon;
        private readonly string label;

        public SubmenuOption(IMenu submenu, IRenderable icon, string label, Color color) : base(
            ActionOption.GenerateActionContent(icon, label, false),
            color
        )
        {
            this.submenu = submenu;
            this.icon = icon;
            this.label = label;
        }

        public override void Execute()
        {
            GameMapContext.GameMapView.ActionMenuContext.OpenSubMenu(submenu);
        }

        public override IRenderable Clone()
        {
            return new SubmenuOption(submenu, icon, label, DefaultColor);
        }
    }
}