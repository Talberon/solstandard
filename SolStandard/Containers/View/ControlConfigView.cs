using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;

namespace SolStandard.Containers.View
{
    public class ControlConfigView : IUserInterface
    {
        private static readonly Color WindowColor = new Color(80, 80, 100);
        private static readonly Color KeyboardOptionColor = TeamUtility.DetermineTeamColor(Team.Creep);
        private static readonly Color PlayerOneColor = TeamUtility.DetermineTeamColor(GameContext.P1Team);
        private static readonly Color PlayerTwoColor = TeamUtility.DetermineTeamColor(GameContext.P2Team);

        public ControlConfigContext.ControlMenuState CurrentState { get; set; }
        private readonly IRenderable cursorSprite;

        private readonly IMenu deviceSelectMenu;
        private IMenu inputRemapSelectMenu;
        private readonly IRenderable mappingInfoWindow;

        public ControlConfigView()
        {
            cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            deviceSelectMenu = GenerateDeviceMenu(cursorSprite, WindowColor);
            inputRemapSelectMenu = GenerateConfigMenuForDevice(ControlConfigContext.Device.Keyboard,
                GameDriver.KeyboardParser.Controller, cursorSprite);

            mappingInfoWindow = new Window(
                new RenderText(AssetManager.MainMenuFont, "Listening for input..."),
                WindowColor
            );
        }

        public IMenu CurrentMenu
        {
            get
            {
                switch (CurrentState)
                {
                    case ControlConfigContext.ControlMenuState.DeviceSelect:
                        return deviceSelectMenu;
                    case ControlConfigContext.ControlMenuState.InputRemapSelect:
                        return inputRemapSelectMenu;
                    case ControlConfigContext.ControlMenuState.ListeningForInput:
                        return inputRemapSelectMenu;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void SelectCurrentOption()
        {
            CurrentMenu.SelectOption();
        }

        public void OpenInputRemapMenu(ControlConfigContext.Device device, IController controller)
        {
            inputRemapSelectMenu = GenerateConfigMenuForDevice(device, controller, cursorSprite);
            CurrentState = ControlConfigContext.ControlMenuState.InputRemapSelect;
        }

        public void GoToPreviousMenu()
        {
            if (CurrentState != ControlConfigContext.ControlMenuState.DeviceSelect)
            {
                CurrentState--;
            }
        }

        private static IMenu GenerateConfigMenuForDevice(
            ControlConfigContext.Device device,
            IController controller,
            IRenderable cursorSprite
        )
        {
            Color windowColor;

            switch (device)
            {
                case ControlConfigContext.Device.Keyboard:
                    windowColor = KeyboardOptionColor;
                    break;
                case ControlConfigContext.Device.P1Gamepad:
                    windowColor = PlayerOneColor;
                    break;
                case ControlConfigContext.Device.P2Gamepad:
                    windowColor = PlayerTwoColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(device), device, null);
            }

            //FIXME Don''t show "None" as an option

            int inputs = Enum.GetValues(typeof(Input)).Length;
            MenuOption[] configOptions = new MenuOption[inputs];

            for (int i = 0; i < inputs; i++)
            {
                configOptions[i] = new RemapInputOption(controller, (Input) i, device, windowColor);
            }

            return new VerticalMenu(configOptions, cursorSprite, KeyboardOptionColor);
        }

        private static IMenu GenerateDeviceMenu(IRenderable cursorSprite, Color windowColor)
        {
            TwoDimensionalMenu menu = new TwoDimensionalMenu(
                new MenuOption[,]
                {
                    {
                        new DeviceSelectOption("Keyboard Config", ControlConfigContext.Device.Keyboard,
                            KeyboardOptionColor),
                        new DeviceSelectOption("Gamepad 1 Config", ControlConfigContext.Device.P1Gamepad,
                            PlayerOneColor),
                        new DeviceSelectOption("Gamepad 2 Config", ControlConfigContext.Device.P2Gamepad,
                            PlayerTwoColor)
                    }
                },
                cursorSprite,
                windowColor,
                TwoDimensionalMenu.CursorType.Pointer
            );

            return menu;
        }

        #region DrawCoordinates

        private static Vector2 CenterItemOnScreen(IRenderable item)
        {
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - (float) item.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) item.Height / 2
            );
        }

        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (CurrentState)
            {
                case ControlConfigContext.ControlMenuState.DeviceSelect:
                    CurrentMenu.Draw(spriteBatch, CenterItemOnScreen(CurrentMenu));
                    break;
                case ControlConfigContext.ControlMenuState.InputRemapSelect:
                    CurrentMenu.Draw(spriteBatch, CenterItemOnScreen(CurrentMenu));
                    break;
                case ControlConfigContext.ControlMenuState.ListeningForInput:
                    mappingInfoWindow.Draw(spriteBatch, CenterItemOnScreen(mappingInfoWindow));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}