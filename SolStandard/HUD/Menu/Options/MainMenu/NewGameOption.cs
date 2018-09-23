using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class NewGameOption : MenuOption
    {
        private const string NewGameOptionText = "New Game ";

        public NewGameOption(Color windowColor) : base(windowColor, NewGameOptionText, AssetManager.MainMenuFont)
        {
        }

        public override void Execute()
        {
            GameContext.LoadMapSelect();
        }
    }
}