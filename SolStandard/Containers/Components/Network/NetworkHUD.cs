using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.MainMenu;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.DialMenu;
using SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Network;
using TextCopy;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;


namespace SolStandard.Containers.Components.Network
{
    public class NetworkHUD : IUserInterface
    {
        private readonly IRenderable title;
        private readonly IRenderable versionNumber;
        private Window networkStatusWindow;
        private TwoDimensionalMenu DialMenu { get; set; }
        private TwoDimensionalMenu HostMenu { get; set; }
        private string inputIPAddress;
        private string hostIPAddress;

        public NetworkHUD(IRenderable title)
        {
            this.title = title;
            versionNumber = new RenderText(AssetManager.WindowFont, $"v{GameDriver.VersionNumber}");
            networkStatusWindow = GenerateStatusWindow();
            inputIPAddress = string.Empty;
            hostIPAddress = string.Empty;

            GenerateDialMenu();
        }

        public IMenu Menu => HostMenu ?? DialMenu;

        public void GenerateDialMenu()
        {
            Color menuColor = MainMenuHUD.MenuColor;

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
                        new MainMenuOption(AssetManager.WindowFont, menuColor, this),
                        new ConnectOption(menuColor, this),
                        new PasteIPAddressOption(menuColor, this),
                    },
                    {
                        new UnselectableOption(RenderBlank.Blank, menuColor),
                        new CreepDisableOption(menuColor),
                        new UnselectableOption(RenderBlank.Blank, menuColor),
                    }
                },
                new SpriteAtlas(AssetManager.MenuCursorTexture, new Vector2(AssetManager.MenuCursorTexture.Width)),
                menuColor,
                TwoDimensionalMenu.CursorType.Pointer
            );
        }

        public void RemoveDialMenu()
        {
            DialMenu = null;
        }

        public void GenerateHostMenu()
        {
            Color menuColor = MainMenuHUD.MenuColor;

            HostMenu = new TwoDimensionalMenu(
                new MenuOption[,]
                {
                    {
                        new CopyIPAddressOption(menuColor, this),
                    },
                    {
                        new CreepDisableOption(menuColor)
                    },
                    {
                        new MainMenuOption(AssetManager.WindowFont, menuColor, this),
                    }
                },
                new SpriteAtlas(AssetManager.MenuCursorTexture, new Vector2(AssetManager.MenuCursorTexture.Width)),
                menuColor,
                TwoDimensionalMenu.CursorType.Pointer
            );
        }

        public void RemoveHostMenu()
        {
            HostMenu = null;
        }

        public void CopyHostIPAddress()
        {
            Clipboard.SetText(hostIPAddress);
        }

        public void PasteIPAddressFromClipboard()
        {
            string clipboardContents = Clipboard.GetText();
            const string ipAddressPattern = "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$";
            bool clipboardHasIP = Regex.IsMatch(clipboardContents, ipAddressPattern);

            if (clipboardHasIP)
            {
                inputIPAddress = clipboardContents;
                UpdateStatus(inputIPAddress, false);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        public void UpdateStatus(string ipAddress, bool hosting, bool serverIpFound = true)
        {
            hostIPAddress = ipAddress;
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
            const string tipText2 = "If your peer cannot connect, make sure you are both running the same major/minor version of the game.";
            string tipText3 = $"YOUR CREEP CONFIG MUST MATCH YOUR PEER. Change it in the menu below if it does not match your peer.";
            string tipText4 = $"Check to make sure you have port forwarding enabled on your router for port {ConnectionManager.NetworkPort}.";

            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {new RenderText(AssetManager.MainMenuFont, displayIpAddress)},
                        {new RenderText(AssetManager.WindowFont, tipText1)},
                        {new RenderText(AssetManager.WindowFont, tipText2)},
                        {new RenderText(AssetManager.WindowFont, tipText3)},
                        {new RenderText(AssetManager.WindowFont, tipText4)},
                        {new RenderText(AssetManager.MainMenuFont, statusMessage)}
                    },
                    2,
                    HorizontalAlignment.Centered
                ),
                MainMenuHUD.MenuColor, HorizontalAlignment.Centered
            );
        }


        public void EnterCharacter(char character)
        {
            var matcher = new Regex("[0-9]|.");

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
            hostIPAddress = string.Empty;
            inputIPAddress = string.Empty;
            GameDriver.ConnectionManager.CloseServer();
            GameDriver.ConnectionManager.DisconnectClient();
        }

        public void AttemptConnection()
        {
            var ipAddressRegex =
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
            GlobalContext.CurrentGameState = GlobalContext.GameState.MainMenu;
            AssetManager.MapUnitCancelSFX.Play();
            ResetIPAddress();
        }

        

        public void Draw(SpriteBatch spriteBatch)
        {
            (float halfScreenWidth, _) = GameDriver.ScreenSize / 2;

            const int titleVertCoordinate = 15;
            (float halfTitleWidth, _) = new Vector2(title.Width, title.Height) / 2;
            var titlePosition = new Vector2(halfScreenWidth - halfTitleWidth, titleVertCoordinate);
            title.Draw(spriteBatch, titlePosition);

            const int titlePadding = 0;
            (float halfStatusWidth, _) = new Vector2(networkStatusWindow.Width, networkStatusWindow.Height) / 2;
            var statusWindowPosition =
                new Vector2(halfScreenWidth - halfStatusWidth, titlePosition.Y + title.Height + titlePadding);
            networkStatusWindow.Draw(spriteBatch, statusWindowPosition);


            if (DialMenu != null)
            {
                const int statusPadding = 10;
                (float halfDialWidth, _) = new Vector2(DialMenu.Width, DialMenu.Height) / 2;
                var dialMenuPosition = new Vector2(
                    halfScreenWidth - halfDialWidth,
                    statusWindowPosition.Y + networkStatusWindow.Height + statusPadding
                );
                DialMenu.Draw(spriteBatch, dialMenuPosition);
            }

            if (HostMenu != null)
            {
                const int statusPadding = 10;
                (float halfHostWidth, _) = new Vector2(HostMenu.Width, HostMenu.Height) / 2;
                var hostMenuPosition = new Vector2(
                    halfScreenWidth - halfHostWidth,
                    statusWindowPosition.Y + networkStatusWindow.Height + statusPadding
                );
                HostMenu.Draw(spriteBatch, hostMenuPosition);
            }

            const int windowPadding = 10;
            versionNumber.Draw(
                spriteBatch,
                new Vector2(GameDriver.ScreenSize.X - versionNumber.Width - windowPadding, windowPadding)
            );
        }
    }
}