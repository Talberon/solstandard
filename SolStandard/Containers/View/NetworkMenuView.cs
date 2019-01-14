using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.HUD.Menu.Options.PauseMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class NetworkMenuView : IUserInterface
    {
        private readonly SpriteAtlas title;
        private readonly AnimatedSpriteSheet logo;
        private readonly SpriteAtlas background;
        private bool visible;
        private Window networkStatusWindow;
        private TwoDimensionalMenu DialMenu;

        public NetworkMenuView(SpriteAtlas title, AnimatedSpriteSheet logo, SpriteAtlas background)
        {
            this.title = title;
            this.logo = logo;
            this.background = background;
            visible = true;
            networkStatusWindow = GenerateStatusWindow();

            Color menuColor = MainMenuView.MenuColor;

            DialMenu = new TwoDimensionalMenu(
                new MenuOption[,]
                {
                    //TODO Replace these options with real ones
                    {new NewGameOption(menuColor), new EndTurnOption(menuColor), new HostGameOption(menuColor),},
                    {new NewGameOption(menuColor), new EndTurnOption(menuColor), new HostGameOption(menuColor),},
                    {new NewGameOption(menuColor), new EndTurnOption(menuColor), new HostGameOption(menuColor),},
                    {new JoinGameOption(menuColor), new ContinueOption(menuColor), new QuitGameOption(menuColor),}
                },
                new SpriteAtlas(AssetManager.MenuCursorTexture, new Vector2(GameDriver.CellSize)),
                menuColor
            );
        }

        public void UpdateStatus(string ipAddress, bool hosting)
        {
            networkStatusWindow = GenerateStatusWindow(ipAddress, hosting);
        }

        private static Window GenerateStatusWindow(string ipAddress = null, bool hosting = true)
        {
            string displayIpAddress = ipAddress ?? "___.___.___.___";
            string statusMessage = (hosting) ? "Waiting for connection..." : "Attempting to connect to host...";

            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {new RenderText(AssetManager.MainMenuFont, displayIpAddress)},
                        {new RenderText(AssetManager.MainMenuFont, statusMessage)}
                    },
                    2,
                    HorizontalAlignment.Centered
                ),
                MainMenuView.MenuColor, HorizontalAlignment.Centered
            );
        }


        public void EnterNumber()
        {
            //TODO Read keyboard input and update IP Address based on input
            //TODO Read any digit or period "." input
            //TODO Read backspace to undo character input
        }

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                Vector2 centerScreen = GameDriver.ScreenSize / 2;

                Vector2 backgroundCenter = new Vector2(background.Width, background.Height) / 2;
                background.Draw(spriteBatch, centerScreen - backgroundCenter);

                const int titleVertCoordinate = 30;
                Vector2 titleCenter = new Vector2(title.Width, title.Height) / 2;
                Vector2 titlePosition = new Vector2(centerScreen.X - titleCenter.X, titleVertCoordinate);
                logo.Draw(spriteBatch, titlePosition);
                title.Draw(spriteBatch, titlePosition + new Vector2(100));

                const int titlePadding = 150;
                Vector2 statusWindowCenter = new Vector2(networkStatusWindow.Width, networkStatusWindow.Height) / 2;
                Vector2 statusWindowPosition =
                    new Vector2(centerScreen.X - statusWindowCenter.X, titlePosition.Y + title.Height + titlePadding);
                networkStatusWindow.Draw(spriteBatch, statusWindowPosition);

                const int statusPadding = 10;
                Vector2 dialMenuCenter = new Vector2(DialMenu.Width, DialMenu.Height) / 2;
                Vector2 dialMenuPosition = new Vector2(
                    centerScreen.X - dialMenuCenter.X,
                    statusWindowPosition.Y + networkStatusWindow.Height + statusPadding
                );
                DialMenu.Draw(spriteBatch, dialMenuPosition);
            }
        }
    }
}