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
        private readonly Vector2 mainMenuPosition;
        private readonly SpriteAtlas background;
        private bool visible;

        public MainMenuUI(ITexture2D menuCursorSprite, ITexture2D windowTexture, SpriteAtlas background)
        {
            this.background = background;
            visible = true;
            mainMenu = GenerateMainMenu(menuCursorSprite, windowTexture);
            mainMenuPosition = (GameDriver.ScreenSize / 2 - new Vector2(mainMenu.Width, mainMenu.Height) / 2);
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
            Vector2 centerScreen = new Vector2(GameDriver.ScreenSize.X / 2, GameDriver.ScreenSize.Y / 2);
            Vector2 backgroundCenter = new Vector2((float) background.Width / 2, (float) background.Height / 2);

            background.Draw(spriteBatch, centerScreen - backgroundCenter);

            if (visible)
            {
                mainMenu.Draw(spriteBatch, mainMenuPosition);
            }
        }
    }
}