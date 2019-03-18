using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.DialMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Network;

namespace SolStandard.Containers.View
{
    public class NetworkMenuView : IUserInterface
    {
        private readonly SpriteAtlas title;
        private readonly AnimatedSpriteSheet logo;
        private readonly SpriteAtlas background;
        private bool visible;
        private Window networkStatusWindow;
        public TwoDimensionalMenu DialMenu { get; private set; }
        private string inputIPAddress;

        public NetworkMenuView(SpriteAtlas title, AnimatedSpriteSheet logo, SpriteAtlas background)
        {
            this.title = title;
            this.logo = logo;
            this.background = background;
            visible = true;
            networkStatusWindow = GenerateStatusWindow();
            inputIPAddress = string.Empty;

            GenerateDialMenu();
        }

        public void GenerateDialMenu()
        {
            Color menuColor = MainMenuView.MenuColor;

            DialMenu = new TwoDimensionalMenu(
                new MenuOption[,]
                {
                    {
                        new CharacterOption('7', menuColor, this),
                        new CharacterOption('8', menuColor, this),
                        new CharacterOption('9', menuColor, this)
                    },
                    {
                        new CharacterOption('4', menuColor, this),
                        new CharacterOption('5', menuColor, this),
                        new CharacterOption('6', menuColor, this)
                    },
                    {
                        new CharacterOption('1', menuColor, this),
                        new CharacterOption('2', menuColor, this),
                        new CharacterOption('3', menuColor, this)
                    },
                    {
                        new BackspaceOption(menuColor, this),
                        new CharacterOption('0', menuColor, this),
                        new CharacterOption('.', menuColor, this)
                    },
                    {
                        new MainMenuOption(menuColor, this),
                        new ConnectOption(menuColor, this),
                        new MainMenuOption(menuColor, this)
                    }
                },
                new SpriteAtlas(AssetManager.MenuCursorTexture, new Vector2(GameDriver.CellSize)),
                menuColor,
                TwoDimensionalMenu.CursorType.Pointer
            );
        }

        public void RemoveDialMenu()
        {
            DialMenu = null;
        }

        public void UpdateStatus(string ipAddress, bool hosting, bool serverIpFound = true)
        {
            networkStatusWindow = GenerateStatusWindow(ipAddress, hosting, serverIpFound);
        }

        private static Window GenerateStatusWindow(string ipAddress = null, bool hosting = true,
            bool serverIpFound = true)
        {
            string displayIpAddress = ipAddress ?? "___.___.___.___";
            string statusMessage = (hosting) ? "Waiting for connection..." : "Attempting to connect to host...";

            string tipText1 = (serverIpFound)
                ? ""
                : "You can get your public IP address by searching online for \"What is my IP?\"";
            string tipText2 = "If your peer cannot connect, check to make sure you have port forwarding enabled on your router for port " +
                  ConnectionManager.NetworkPort + ".";

            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {new RenderText(AssetManager.MainMenuFont, displayIpAddress)},
                        {new RenderText(AssetManager.WindowFont, tipText1)},
                        {new RenderText(AssetManager.WindowFont, tipText2)},
                        {new RenderText(AssetManager.MainMenuFont, statusMessage)}
                    },
                    2,
                    HorizontalAlignment.Centered
                ),
                MainMenuView.MenuColor, HorizontalAlignment.Centered
            );
        }


        public void EnterCharacter(char character)
        {
            Regex matcher = new Regex("[0-9]|.");

            if (matcher.IsMatch(character.ToString()) && inputIPAddress.Length < 15)
            {
                inputIPAddress += character;
                UpdateStatus(inputIPAddress, false);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        public void BackspaceCharacter()
        {
            if (inputIPAddress.Length > 0)
            {
                inputIPAddress = inputIPAddress.Substring(0, inputIPAddress.Length - 1);
                UpdateStatus(inputIPAddress, false);
                AssetManager.MapUnitCancelSFX.Play();
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        public void ResetIPAddress()
        {
            inputIPAddress = string.Empty;
        }

        public void AttemptConnection()
        {
            Regex ipAddressRegex =
                new Regex(
                    "^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");

            if (ipAddressRegex.IsMatch(inputIPAddress))
            {
                GameDriver.JoinGame(inputIPAddress);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        public void Exit()
        {
            GameContext.CurrentGameState = GameContext.GameState.MainMenu;
            AssetManager.MapUnitCancelSFX.Play();
            ResetIPAddress();
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

                const int titlePadding = 100;
                Vector2 statusWindowCenter = new Vector2(networkStatusWindow.Width, networkStatusWindow.Height) / 2;
                Vector2 statusWindowPosition =
                    new Vector2(centerScreen.X - statusWindowCenter.X, titlePosition.Y + title.Height + titlePadding);
                networkStatusWindow.Draw(spriteBatch, statusWindowPosition);


                if (DialMenu != null)
                {
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
}