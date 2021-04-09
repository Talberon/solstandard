using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public class OpenControlsMenuOption : MenuOption
    {
        public OpenControlsMenuOption(Color color) :
            base(new RenderText(AssetManager.WindowFont, "Control Config"), color)
        {
        }

        public override void Execute()
        {
            GlobalContext.ControlConfigContext.OpenMenu();
        }

        public override IRenderable Clone()
        {
            return new OpenCodexOption(DefaultColor);
        }
    }
}