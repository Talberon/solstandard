using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.HUD.Menu.Options.PauseMenu;
using SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public static class PauseScreenView
    {
        public enum PauseMenus
        {
            Primary,
            PauseConfig
        }

        private static readonly Color OptionsColor = new Color(40, 40, 40, 180);
        private static IMenu PauseMenu { get; set; }
        private static IMenu ConfigMenu { get; set; }
        private static PauseMenus _currentMenu;
        private static bool _visible;

        public static void Initialize(GameDriver gameDriver)
        {
            SpriteAtlas cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            PauseMenu = new VerticalMenu(
                new MenuOption[]
                {
                    new ContinueOption(OptionsColor),
                    new OpenCodexOption(OptionsColor),
                    new ConfigOption(OptionsColor),
                    new ConcedeOption(OptionsColor)
                },
                cursorSprite,
                OptionsColor
            );

            ConfigMenu = new VerticalMenu(
                new MenuOption[]
                {
                    new MusicMuteOption(OptionsColor),
                    new MusicVolumeUpOption(OptionsColor),
                    new MusicVolumeDownOption(OptionsColor),
                    new SoundEffectMuteOption(OptionsColor),
                    new ToggleFullscreenOption(OptionsColor, gameDriver),
                    new ReturnToPauseMenuOption(OptionsColor)
                },
                cursorSprite,
                OptionsColor
            );

            _visible = true;
            _currentMenu = PauseMenus.Primary;
        }


        public static IMenu CurrentMenu
        {
            get
            {
                switch (_currentMenu)
                {
                    case PauseMenus.Primary:
                        return PauseMenu;
                    case PauseMenus.PauseConfig:
                        return ConfigMenu;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void ChangeMenu(PauseMenus menu)
        {
            _currentMenu = menu;
        }

        public static void OpenScreen(PauseMenus menuType)
        {
            ChangeMenu(menuType);
            GameContext.CurrentGameState = GameContext.GameState.PauseScreen;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (!_visible) return;

            Vector2 centerScreen = GameDriver.ScreenSize / 2 - new Vector2(CurrentMenu.Width, CurrentMenu.Height) / 2;

            switch (_currentMenu)
            {
                case PauseMenus.Primary:
                    PauseMenu.Draw(spriteBatch, centerScreen);
                    break;
                case PauseMenus.PauseConfig:
                    ConfigMenu.Draw(spriteBatch, centerScreen);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}