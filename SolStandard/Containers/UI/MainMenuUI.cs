using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.UI
{
    public class MainMenuUI : IUserInterface
    {
        private readonly VerticalMenu mainMenu;
        private readonly SpriteAtlas background;
        private bool visible;

        public MainMenuUI(ITexture2D menuCursorSprite, ITexture2D windowTexture, SpriteAtlas background)
        {
            this.background = background;
            visible = true;
            mainMenu = GenerateMainMenu(menuCursorSprite, windowTexture);
        }

        public VerticalMenu MainMenu
        {
            get { return mainMenu; }
        }

        private static VerticalMenu GenerateMainMenu(ITexture2D menuCursorSprite, ITexture2D windowTexture)
        {
            IOption[] options =
            {
                new NewGameOption(windowTexture),
                new QuitGameOption(windowTexture)
            };
            IRenderable cursorSprite = new SpriteAtlas(menuCursorSprite,
                new Vector2(menuCursorSprite.Width, menuCursorSprite.Height), 1);

            return new VerticalMenu(options, cursorSprite);
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
                const int titleVertCoordinate = 30;

                Vector2 backgroundCenter = new Vector2(background.Width, background.Height) / 2;
                Vector2 backgroundPosition = new Vector2(centerScreen.X - backgroundCenter.X, titleVertCoordinate);
                background.Draw(spriteBatch, backgroundPosition);

                const int padding = 100;
                Vector2 mainMenuCenter = new Vector2(mainMenu.Width, mainMenu.Height) / 2;
                Vector2 mainMenuPosition =
                    new Vector2(centerScreen.X - mainMenuCenter.X, backgroundPosition.Y + background.Height + padding);
                mainMenu.Draw(spriteBatch, mainMenuPosition);
            }
        }
    }
}