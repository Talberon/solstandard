using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class NewGameOption : MenuOption
    {
        private const string NewGameOptionText = "New Game";

        public NewGameOption(Color windowColor) : base(new RenderText(AssetManager.MainMenuFont, NewGameOptionText), windowColor)
        {
        }

        public override void Execute()
        {
            GameContext.LoadMapSelect();
        }
    }
}