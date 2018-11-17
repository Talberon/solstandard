using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ControlsOption : MenuOption
    {
        private readonly PauseScreenView pauseScreenView;

        public ControlsOption(Color color, PauseScreenView pauseScreenView) :
            base(new RenderText(AssetManager.MainMenuFont, "View Controls"), color)
        {
            this.pauseScreenView = pauseScreenView;
        }

        public override void Execute()
        {
            //Show Control Scheme
            pauseScreenView.ChangeMenu(PauseScreenView.PauseMenus.Controller);
        }
        
        public override IRenderable Clone()
        {
            return new ControlsOption(DefaultColor, pauseScreenView);
        }
    }
}