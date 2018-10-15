using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.UI;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ConfigOption : MenuOption
    {
        private readonly PauseMenuUI pauseMenuUI;

        public ConfigOption(Color color, PauseMenuUI pauseMenuUI) : base(
            new RenderText(AssetManager.MainMenuFont, "Config"), color)
        {
            this.pauseMenuUI = pauseMenuUI;
        }

        public override void Execute()
        {
            //Show Options Menu for Sound + Music
            pauseMenuUI.ChangeMenu(PauseMenuUI.PauseMenus.Config);
        }
    }
}