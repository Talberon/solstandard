using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.PauseMenu;
using SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class PauseScreenView : IUserInterface
    {
        public enum PauseMenus
        {
            Primary,
            Controller,
            Config
        }

        private static readonly Color OptionsColor = new Color(40, 40, 40, 180);
        private VerticalMenu PauseMenu { get; set; }
        private VerticalMenu ConfigMenu { get; set; }
        private VerticalMenu ControlsMenu { get; set; }
        private PauseMenus currentMenu;
        private bool visible;

        public PauseScreenView()
        {
            SpriteAtlas cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            SpriteAtlas gamepadHelp = new SpriteAtlas(AssetManager.GamepadControlHelp,
                new Vector2(AssetManager.GamepadControlHelp.Width, AssetManager.GamepadControlHelp.Height));
            SpriteAtlas keyboardHelp = new SpriteAtlas(AssetManager.KeyboardControlHelp,
                new Vector2(AssetManager.KeyboardControlHelp.Width, AssetManager.KeyboardControlHelp.Height));

            PauseMenu = new VerticalMenu(
                new MenuOption[]
                {
                    new ContinueOption(OptionsColor),
                    new ControlsOption(OptionsColor, this),
                    new ConfigOption(OptionsColor, this),
                    new ConcedeOption(OptionsColor)
                },
                cursorSprite,
                OptionsColor
            );

            ConfigMenu = new VerticalMenu(
                new MenuOption[]
                {
                    new MusicMuteOption(OptionsColor),
                    new SoundEffectMuteOption(OptionsColor),
                    new ReturnToPauseMenuOption(OptionsColor, this)
                },
                cursorSprite,
                OptionsColor
            );

            ControlsMenu = new VerticalMenu(
                new MenuOption[]
                {
                    new ReturnToPauseMenuOption(OptionsColor, this),
                    new UnselectableOption(gamepadHelp, OptionsColor),
                    new UnselectableOption(keyboardHelp, OptionsColor),
                },
                cursorSprite,
                OptionsColor
            );

            visible = true;
            currentMenu = PauseMenus.Primary;
        }

        public VerticalMenu CurrentMenu
        {
            get
            {
                switch (currentMenu)
                {
                    case PauseMenus.Primary:
                        return PauseMenu;
                    case PauseMenus.Controller:
                        return ControlsMenu;
                    case PauseMenus.Config:
                        return ConfigMenu;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void ChangeMenu(PauseMenus menu)
        {
            currentMenu = menu;
        }

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            Vector2 centerScreen = GameDriver.ScreenSize / 2 - new Vector2(CurrentMenu.Width, CurrentMenu.Height) / 2;

            switch (currentMenu)
            {
                case PauseMenus.Primary:
                    PauseMenu.Draw(spriteBatch, centerScreen);
                    break;
                case PauseMenus.Controller:
                    ControlsMenu.Draw(spriteBatch, centerScreen);
                    break;
                case PauseMenus.Config:
                    ConfigMenu.Draw(spriteBatch, centerScreen);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}