using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class MainMenuView : IUserInterface
    {
        private readonly VerticalMenu mainMenu;
        private readonly Color menuColor;
        private readonly SpriteAtlas title;
        private readonly AnimatedSpriteSheet logo;
        private readonly SpriteAtlas background;
        private bool visible;

        public MainMenuView(SpriteAtlas title, AnimatedSpriteSheet logo, SpriteAtlas background)
        {
            this.title = title;
            this.logo = logo;
            this.background = background;
            visible = true;
            menuColor = new Color(127, 148, 217);
            mainMenu = GenerateMainMenu();
        }

        public VerticalMenu MainMenu
        {
            get { return mainMenu; }
        }

        private VerticalMenu GenerateMainMenu()
        {
            MenuOption[] options =
            {
                new NewGameOption(menuColor),
                new QuitGameOption(menuColor)
            };
            IRenderable cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            return new VerticalMenu(options, cursorSprite, menuColor);
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