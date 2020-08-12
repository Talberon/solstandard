using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Monogame;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;


namespace SolStandard.Containers.Components.InputRemapping
{
    public class ControlConfigView : IUserInterface
    {
        private const int WindowPadding = 10;
        private static readonly Color PrimaryWindowColor = new Color(50, 50, 60);
        private static readonly Color KeyboardOptionColor = TeamUtility.DetermineTeamWindowColor(Team.Creep);
        private static readonly Color PlayerOneColor = TeamUtility.DetermineTeamWindowColor(GlobalContext.P1Team);
        private static readonly Color PlayerTwoColor = TeamUtility.DetermineTeamWindowColor(GlobalContext.P2Team);

        public ControlConfigContext.ControlMenuState CurrentState { get; set; }
        private readonly IRenderable cursorSprite;

        private readonly IMenu deviceSelectMenu;
        private IMenu inputRemapSelectMenu;
        private IRenderable inputRemapInfoWindow;
        private readonly IRenderable mappingInfoWindow;

        public ControlConfigView()
        {
            cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            deviceSelectMenu = GenerateDeviceMenu(cursorSprite, PrimaryWindowColor);
            inputRemapSelectMenu = GenerateConfigMenuForDevice(ControlConfigContext.Device.Keyboard,
                GameDriver.KeyboardParser.Controller, cursorSprite);

            mappingInfoWindow = new Window(
                new RenderText(AssetManager.MainMenuFont, "Listening for input..."),
                PrimaryWindowColor
            );
        }

        private static IRenderable GenerateHelpInfoWindowContent()
        {
            ISpriteFont font = AssetManager.MainMenuFont;

            return new WindowContentGrid(new[,]
            {
                {
                    new RenderText(font, "Select an input to remap it. Press the"),
                    InputIconProvider.GetInputIcon(Input.Cancel, (int) font.MeasureString("A").Y),
                    new RenderText(font, " button to exit config.")
                }
            }, 1, HorizontalAlignment.Centered);
        }


        public IMenu CurrentMenu
        {
            get
            {
                return CurrentState switch
                {
                    ControlConfigContext.ControlMenuState.DeviceSelect => deviceSelectMenu,
                    ControlConfigContext.ControlMenuState.InputRemapSelect => inputRemapSelectMenu,
                    ControlConfigContext.ControlMenuState.ListeningForInput => inputRemapSelectMenu,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public void SelectCurrentOption()
        {
            CurrentMenu.SelectOption();
        }

        public void OpenInputRemapMenu(ControlConfigContext.Device device, IController controller)
        {
            inputRemapSelectMenu = GenerateConfigMenuForDevice(device, controller, cursorSprite);
            inputRemapInfoWindow = new Window(GenerateHelpInfoWindowContent(), PrimaryWindowColor);
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
            Color windowColor = device switch
            {
                ControlConfigContext.Device.Keyboard => KeyboardOptionColor,
                ControlConfigContext.Device.P1Gamepad => PlayerOneColor,
                ControlConfigContext.Device.P2Gamepad => PlayerTwoColor,
                _ => throw new ArgumentOutOfRangeException(nameof(device), device, null)
            };

            var configOptions = new MenuOption[4, 5];
            configOptions[0, 0] = new RemapInputOption(controller, Input.CursorUp, device, Color.Transparent);
            configOptions[1, 0] = new RemapInputOption(controller, Input.CursorDown, device, Color.Transparent);
            configOptions[2, 0] = new RemapInputOption(controller, Input.CursorLeft, device, Color.Transparent);
            configOptions[3, 0] = new RemapInputOption(controller, Input.CursorRight, device, Color.Transparent);

            configOptions[0, 1] = new RemapInputOption(controller, Input.CameraUp, device, Color.Transparent);
            configOptions[1, 1] = new RemapInputOption(controller, Input.CameraDown, device, Color.Transparent);
            configOptions[2, 1] = new RemapInputOption(controller, Input.CameraLeft, device, Color.Transparent);
            configOptions[3, 1] = new RemapInputOption(controller, Input.CameraRight, device, Color.Transparent);

            configOptions[0, 2] = new RemapInputOption(controller, Input.Confirm, device, Color.Transparent);
            configOptions[1, 2] = new RemapInputOption(controller, Input.Cancel, device, Color.Transparent);
            configOptions[2, 2] = new RemapInputOption(controller, Input.PreviewUnit, device, Color.Transparent);
            configOptions[3, 2] = new RemapInputOption(controller, Input.PreviewItem, device, Color.Transparent);

            configOptions[0, 3] = new RemapInputOption(controller, Input.TabLeft, device, Color.Transparent);
            configOptions[1, 3] = new RemapInputOption(controller, Input.TabRight, device, Color.Transparent);
            configOptions[2, 3] = new RemapInputOption(controller, Input.ZoomOut, device, Color.Transparent);
            configOptions[3, 3] = new RemapInputOption(controller, Input.ZoomIn, device, Color.Transparent);

            configOptions[0, 4] = new RemapInputOption(controller, Input.Status, device, Color.Transparent);
            configOptions[1, 4] = new RemapInputOption(controller, Input.Menu, device, Color.Transparent);
            configOptions[2, 4] = new ResetControllerConfigOption("Reset Controller", device, PrimaryWindowColor);
            configOptions[3, 4] = new SaveControllerOption("Save Inputs", PrimaryWindowColor);

            return new TwoDimensionalMenu(configOptions, cursorSprite, windowColor,
                TwoDimensionalMenu.CursorType.Pointer);
        }

        private static IMenu GenerateDeviceMenu(IRenderable cursorSprite, Color windowColor)
        {
            var menu = new TwoDimensionalMenu(
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

        private static Vector2 AboveTarget(Vector2 targetCoords, IRenderable target, IRenderable thingToDraw)
        {
            return new Vector2(
                targetCoords.X + ((float) target.Width / 2) - ((float) thingToDraw.Width / 2),
                targetCoords.Y - thingToDraw.Height - WindowPadding
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
                    Vector2 menuCoordinates = CenterItemOnScreen(CurrentMenu);
                    inputRemapInfoWindow.Draw(
                        spriteBatch,
                        AboveTarget(menuCoordinates, CurrentMenu, inputRemapInfoWindow)
                    );
                    CurrentMenu.Draw(spriteBatch, menuCoordinates);
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