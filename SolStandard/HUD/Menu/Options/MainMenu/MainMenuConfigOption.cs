using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.World.SubContext.Pause;
using SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class MainMenuConfigOption : MenuOption
    {
        public MainMenuConfigOption(Color color) :
            base(new RenderText(AssetManager.MainMenuFont, "Config"), color)
        {
        }

        public override void Execute()
        {
            ReturnToPauseMenuOption.FromMainMenu = true;
            PauseScreenUtils.OpenScreen(PauseScreenUtils.PauseMenus.PauseConfig);
        }

        public override IRenderable Clone()
        {
            return new MainMenuConfigOption(DefaultColor);
        }
    }
}