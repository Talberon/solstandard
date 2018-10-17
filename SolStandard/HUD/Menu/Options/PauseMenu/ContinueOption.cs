using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ContinueOption : MenuOption
    {
        public ContinueOption(Color color) : base(new RenderText(AssetManager.MainMenuFont, "Continue"), color)
        {
        }

        public override void Execute()
        {
            GameContext.CurrentGameState = GameContext.GameState.InGame;
        }
    }
}