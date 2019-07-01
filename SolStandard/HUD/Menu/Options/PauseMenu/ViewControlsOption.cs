using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ControlsOption : MenuOption
    {
        public ControlsOption(Color color) :
            base(new RenderText(AssetManager.MainMenuFont, "View Controls"), color)
        {
        }

        public override void Execute()
        {
            //Show Control Scheme
            PauseScreenView.OpenScreen(PauseScreenView.PauseMenus.Controller);
        }

        public override IRenderable Clone()
        {
            return new ControlsOption(DefaultColor);
        }
    }
}