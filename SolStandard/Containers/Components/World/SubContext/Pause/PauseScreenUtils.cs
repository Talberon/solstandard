using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.HUD.Menu.Options.PauseMenu;
using SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu;
using SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Components.World.SubContext.Pause
{
    public static class PauseScreenUtils
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
            var cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
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
                    new OpenControlsMenuOption(OptionsColor), 
                    new MusicMuteOption(OptionsColor),
                    new MusicVolumeUpOption(OptionsColor),
                    new MusicVolumeDownOption(OptionsColor),
                    new SoundEffectMuteOption(OptionsColor),
                    new ToggleFullscreenOption(OptionsColor, gameDriver),
                    new CreepDisableOption(OptionsColor),
                    new ReturnToPauseMenuOption(OptionsColor),
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
                return _currentMenu switch
                {
                    PauseMenus.Primary => PauseMenu,
                    PauseMenus.PauseConfig => ConfigMenu,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public static void ChangeMenu(PauseMenus menu)
        {
            _currentMenu = menu;
        }

        public static void OpenScreen(PauseMenus menuType)
        {
            ChangeMenu(menuType);
            GlobalContext.CurrentGameState = GlobalContext.GameState.PauseScreen;
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