using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.World;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options
{
    public class SubmenuOption : MenuOption, IOptionDescription
    {
        private readonly IMenu submenu;
        private readonly IRenderable icon;
        private readonly string label;
        public IRenderable Description { get; }

        public SubmenuOption(IMenu submenu, IRenderable icon, string label, Color color) : this(
            submenu, icon, label, string.Empty, color
        )
        {
        }

        public SubmenuOption(IMenu submenu, IRenderable icon, string label, string description, Color color) : base(
            ActionOption.GenerateActionContent(icon, label, false),
            color
        )
        {
            this.submenu = submenu;
            this.icon = icon;
            this.label = label;
            Description = new RenderText(AssetManager.WindowFont, description);
        }


        public override void Execute()
        {
            WorldContext.WorldHUD.ActionMenuContext.OpenSubMenu(submenu);
        }

        public override IRenderable Clone()
        {
            return new SubmenuOption(submenu, icon, label, DefaultColor);
        }

    }
}