using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.Network;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu.Options.DialMenu
{
    public class MainMenuOption : MenuOption
    {
        private readonly ISpriteFont font;
        private NetworkHUD NetworkHUD { get; }
        private const string MainMenu = "Menu";

        public MainMenuOption(ISpriteFont font, Color color, NetworkHUD networkHUD) : base(
            new RenderText(font, MainMenu), color, HorizontalAlignment.Centered
        )
        {
            this.font = font;
            NetworkHUD = networkHUD;
        }

        public override void Execute()
        {
            GlobalContext.CurrentGameState = GlobalContext.GameState.MainMenu;
            NetworkHUD.ResetIPAddress();
        }

        public override IRenderable Clone()
        {
            return new MainMenuOption(font, DefaultColor, NetworkHUD);
        }
    }
}