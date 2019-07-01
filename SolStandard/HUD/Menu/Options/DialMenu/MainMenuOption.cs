using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.View;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu.Options.DialMenu
{
    public class MainMenuOption : MenuOption
    {
        private readonly ISpriteFont font;
        private NetworkMenuView NetworkMenuView { get; }
        private const string MainMenu = "Menu";

        public MainMenuOption(ISpriteFont font, Color color, NetworkMenuView networkMenuView) : base(
            new RenderText(font, MainMenu), color, HorizontalAlignment.Centered
        )
        {
            this.font = font;
            NetworkMenuView = networkMenuView;
        }

        public override void Execute()
        {
            GameContext.CurrentGameState = GameContext.GameState.MainMenu;
            NetworkMenuView.ResetIPAddress();
        }

        public override IRenderable Clone()
        {
            return new MainMenuOption(font, DefaultColor, NetworkMenuView);
        }
    }
}