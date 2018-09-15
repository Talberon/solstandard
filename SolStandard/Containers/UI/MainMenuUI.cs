using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.Utility;

namespace SolStandard.Containers.UI
{
    public class MainMenuUI : IUserInterface
    {
        private readonly VerticalMenu mainMenu;
        private readonly SpriteAtlas title;
        private readonly AnimatedSprite logo;
        private readonly SpriteAtlas background;
        private bool visible;

        public MainMenuUI(SpriteAtlas title, AnimatedSprite logo, SpriteAtlas background)
        {
            this.title = title;
            this.logo = logo;
            this.background = background;
            visible = true;
            mainMenu = GenerateMainMenu();
        }

        public VerticalMenu MainMenu
        {
            get { return mainMenu; }
        }

        private static VerticalMenu GenerateMainMenu()
        {
            IOption[] options =
            {
                new NewGameOption(GameDriver.WindowTexture),
                new QuitGameOption(GameDriver.WindowTexture)
            };
            IRenderable cursorSprite = new SpriteAtlas(GameDriver.MenuCursorTexture,
                new Vector2(GameDriver.MenuCursorTexture.Width, GameDriver.MenuCursorTexture.Height), 1);

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

                Vector2 backgroundCenter = new Vector2(background.Width, background.Height) / 2;
                background.Draw(spriteBatch, centerScreen - backgroundCenter);

                const int titleVertCoordinate = 30;
                Vector2 titleCenter = new Vector2(title.Width, title.Height) / 2;
                Vector2 titlePosition = new Vector2(centerScreen.X - titleCenter.X, titleVertCoordinate);
                logo.Draw(spriteBatch, titlePosition);
                title.Draw(spriteBatch, titlePosition + new Vector2(100));

                const int titlePadding = 200;
                Vector2 mainMenuCenter = new Vector2(mainMenu.Width, mainMenu.Height) / 2;
                Vector2 mainMenuPosition =
                    new Vector2(centerScreen.X - mainMenuCenter.X, titlePosition.Y + title.Height + titlePadding);
                mainMenu.Draw(spriteBatch, mainMenuPosition);
            }
        }
    }
}