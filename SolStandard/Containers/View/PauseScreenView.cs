using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.HUD.Menu.Options.PauseMenu;
using SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu;
using SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons.Gamepad;
using SolStandard.Utility.Buttons.KeyboardInput;

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
        private VerticalMenu PauseMenu { get; }
        private VerticalMenu ConfigMenu { get; }
        private TwoDimensionalMenu ControlsMenu { get; }
        private PauseMenus currentMenu;
        private bool visible;

        public PauseScreenView()
        {
            SpriteAtlas cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            PauseMenu = new VerticalMenu(
                new MenuOption[]
                {
                    new ContinueOption(OptionsColor),
                    new OpenCodexOption(OptionsColor),
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
                    new MusicVolumeUpOption(OptionsColor),
                    new MusicVolumeDownOption(OptionsColor),
                    new SoundEffectMuteOption(OptionsColor),
                    new ReturnToPauseMenuOption(OptionsColor, this)
                },
                cursorSprite,
                OptionsColor
            );

            ControlsMenu = new TwoDimensionalMenu(
                new MenuOption[,]
                {
                    {
                        new ReturnToPauseMenuOption(OptionsColor, this),
                        new GamepadOption(new GamepadController(PlayerIndex.Four), OptionsColor),
                        new KeyboardOption(new KeyboardController(), OptionsColor)
                    }
                },
                cursorSprite,
                OptionsColor,
                TwoDimensionalMenu.CursorType.Pointer
            );

            visible = true;
            currentMenu = PauseMenus.Primary;
        }

        public IMenu CurrentMenu
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