using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Components.MainMenu
{
    public class MainMenuHUD : IUserInterface
    {
        private const int WindowPadding = 10;
        public static readonly Color MenuColor = new Color(10, 35, 50, 100);
        public static readonly Color ControlsColor = new Color(10, 35, 50, 200);
        private readonly IRenderable title;
        private readonly IRenderable versionNumber;
        private readonly IRenderable copyright;
        private readonly IRenderable controls;

        public MainMenuHUD(IRenderable title)
        {
            this.title = title;
            MainMenu = GenerateMainMenu();

            copyright = new RenderText(AssetManager.WindowFont, "Copyright @Talberon 2019",
                new Color(100, 100, 100, 100));

            controls = GenerateInputInstructions();

            versionNumber = new RenderText(AssetManager.WindowFont, $"v{GameDriver.VersionNumber}");
        }

        public VerticalMenu MainMenu { get; }

        private static VerticalMenu GenerateMainMenu()
        {
            MenuOption[] options =
            {
                new NewGameOption(MenuColor),
                new HostGameOption(MenuColor),
                new JoinGameOption(MenuColor),
                new OpenCodexOption(MenuColor),
                new MainMenuConfigOption(MenuColor),
                new CreditsOption(MenuColor),
                new HowToPlayOption(MenuColor),
                new QuitGameOption(MenuColor)
            };
            IRenderable cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            return new VerticalMenu(options, cursorSprite, MenuColor);
        }

        private static Window GenerateInputInstructions()
        {
            ISpriteFont windowFont = AssetManager.WindowFont;

            IRenderable[,] promptTextContent =
            {
                {
                    new WindowContentGrid(new[,]
                    {
                        {
                            new RenderText(windowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.CursorUp, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorDown, GameDriver.CellSize),
                            new RenderText(windowFont, " to move cursor.")
                        }
                    })
                },
                {
                    new WindowContentGrid(new[,]
                    {
                        {
                            new RenderText(windowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.Confirm, GameDriver.CellSize),
                            new RenderText(windowFont, " to confirm selection.")
                        }
                    })
                }
            };
            var promptWindowContentGrid = new WindowContentGrid(promptTextContent, 2);

            return new Window(promptWindowContentGrid, ControlsColor);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerScreen = GameDriver.ScreenSize / 2;

            const int titleVertCoordinate = 80;
            Vector2 titleCenter = new Vector2(title.Width, title.Height) / 2;
            var titlePosition = new Vector2(centerScreen.X - titleCenter.X, titleVertCoordinate);
            title.Draw(spriteBatch, titlePosition);

            DrawMenu(spriteBatch, centerScreen, titlePosition);

            copyright.Draw(spriteBatch, GameDriver.ScreenSize - new Vector2(copyright.Width, copyright.Height));

            controls.Draw(
                spriteBatch,
                new Vector2(WindowPadding, GameDriver.ScreenSize.Y - WindowPadding - controls.Height)
            );

            versionNumber.Draw(
                    spriteBatch,
                new Vector2(GameDriver.ScreenSize.X - versionNumber.Width - WindowPadding, WindowPadding)
                );
        }

        private void DrawMenu(SpriteBatch spriteBatch, Vector2 centerScreen, Vector2 titlePosition)
        {
            const int titlePadding = 5;
            Vector2 mainMenuCenter = new Vector2(MainMenu.Width, MainMenu.Height) / 2;
            var mainMenuPosition =
                new Vector2(centerScreen.X - mainMenuCenter.X, titlePosition.Y + title.Height + titlePadding);
            MainMenu.Draw(spriteBatch, mainMenuPosition);
        }
    }
}