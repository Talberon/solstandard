using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.View;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DialMenu
{
    public class MainMenuOption : MenuOption
    {
        private NetworkMenuView NetworkMenuView { get; set; }
        private const string MainMenu = "Menu";

        public MainMenuOption(Color color, NetworkMenuView networkMenuView) : base(
            new RenderText(AssetManager.WindowFont, MainMenu), color
        )
        {
            NetworkMenuView = networkMenuView;
        }

        public override void Execute()
        {
            GameContext.CurrentGameState = GameContext.GameState.MainMenu;
        }

        public override IRenderable Clone()
        {
            return new MainMenuOption(DefaultColor, NetworkMenuView);
        }
    }
}