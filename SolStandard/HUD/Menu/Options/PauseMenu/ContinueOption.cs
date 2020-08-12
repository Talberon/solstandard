using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
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
            GlobalContext.CurrentGameState = GlobalContext.GameState.InGame;
        }

        public override IRenderable Clone()
        {
            return new ConcedeOption(DefaultColor);
        }
    }
}