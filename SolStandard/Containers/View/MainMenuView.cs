﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class MainMenuView : IUserInterface
    {
        private readonly VerticalMenu mainMenu;
        public static readonly Color MenuColor = new Color(10, 35, 50, 100);
        private readonly SpriteAtlas title;
        private readonly AnimatedSpriteSheet logo;
        private readonly SpriteAtlas background;
        private readonly RenderText copyright;
        private bool visible;

        public MainMenuView(SpriteAtlas title, AnimatedSpriteSheet logo, SpriteAtlas background)
        {
            this.title = title;
            this.logo = logo;
            this.background = background;
            visible = true;
            mainMenu = GenerateMainMenu();
            copyright = new RenderText(AssetManager.WindowFont, "Copyright @Talberon 2019",
                new Color(100, 100, 100, 100));
        }

        public VerticalMenu MainMenu
        {
            get { return mainMenu; }
        }

        private VerticalMenu GenerateMainMenu()
        {
            MenuOption[] options =
            {
                new NewGameOption(MenuColor),
                new HostGameOption(MenuColor),
                new JoinGameOption(MenuColor),
                new OpenCodexOption(MenuColor),
                new CreditsOption(MenuColor),
                new QuitGameOption(MenuColor)
            };
            IRenderable cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            return new VerticalMenu(options, cursorSprite, MenuColor);
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

                DrawBackground(spriteBatch, centerScreen);

                const int titleVertCoordinate = 20;
                Vector2 titleCenter = new Vector2(title.Width, title.Height) / 2;
                Vector2 titlePosition = new Vector2(centerScreen.X - titleCenter.X, titleVertCoordinate);
                logo.Draw(spriteBatch, titlePosition);
                title.Draw(spriteBatch, titlePosition + new Vector2(100));

                DrawMenu(spriteBatch, centerScreen, titlePosition);

                copyright.Draw(spriteBatch, GameDriver.ScreenSize - new Vector2(copyright.Width, copyright.Height));
            }
        }

        private void DrawBackground(SpriteBatch spriteBatch, Vector2 centerScreen)
        {
            Vector2 backgroundCenter = new Vector2(background.Width, background.Height) / 2;
            background.Draw(spriteBatch, centerScreen - backgroundCenter);
        }

        private void DrawMenu(SpriteBatch spriteBatch, Vector2 centerScreen, Vector2 titlePosition)
        {
            const int titlePadding = 110;
            Vector2 mainMenuCenter = new Vector2(mainMenu.Width, mainMenu.Height) / 2;
            Vector2 mainMenuPosition =
                new Vector2(centerScreen.X - mainMenuCenter.X, titlePosition.Y + title.Height + titlePadding);
            mainMenu.Draw(spriteBatch, mainMenuPosition);
        }
    }
}