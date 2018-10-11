using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class NewGameOption : MenuOption
    {
        private const string NewGameOptionText = "New Game";

        public NewGameOption(Color windowColor) : base(
            windowColor,
            new RenderText(AssetManager.MainMenuFont, NewGameOptionText)
        )
        {
        }

        public override void Execute()
        {
            GameContext.LoadMapSelect();
        }
    }
}