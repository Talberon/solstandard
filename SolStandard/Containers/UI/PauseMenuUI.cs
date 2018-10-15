using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.PauseMenu;
using SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.UI
{
    public class PauseMenuUI : IUserInterface
    {
        public enum PauseMenus
        {
            Primary,
            Config
        }

        private static readonly Color OptionsColor = new Color(30, 30, 30, 180);
        private VerticalMenu PauseMenu { get; set; }
        private VerticalMenu ConfigMenu { get; set; }
        private PauseMenus currentMenu;
        private bool visible;

        public PauseMenuUI(GameMapContext gameMapContext)
        {
            SpriteAtlas cursorTexture = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height), 1);

            PauseMenu = new VerticalMenu(
                new MenuOption[]
                {
                    new ContinueOption(OptionsColor),
                    new ControlsOption(OptionsColor),
                    new ConfigOption(OptionsColor, this),
                    new EndTurnOption(OptionsColor, gameMapContext),
                    new ConcedeOption(OptionsColor),
                },
                cursorTexture,
                OptionsColor
            );

            ConfigMenu = new VerticalMenu(
                new MenuOption[]
                {
                    new MusicMuteOption(OptionsColor),
                    new SoundEffectMuteOption(OptionsColor),
                    new ReturnToPauseMenuOption(OptionsColor, this)
                },
                cursorTexture,
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

            Vector2 centerScreen = GameDriver.ScreenSize / 2 - new Vector2(PauseMenu.Width, PauseMenu.Height) / 2;

            switch (currentMenu)
            {
                case PauseMenus.Primary:
                    PauseMenu.Draw(spriteBatch, centerScreen);
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