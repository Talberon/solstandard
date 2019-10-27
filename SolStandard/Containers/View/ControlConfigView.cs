using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu;
using SolStandard.Utility;
using SolStandard.Utility.Inputs;

namespace SolStandard.Containers.View
{
    public class ControlConfigView : IUserInterface
    {
        public enum ControlMenuStates
        {
            DeviceSelect,
            InputRemapSelect,
            ListeningForInput
        }

        private enum Devices
        {
            Keyboard,
            P1Gamepad,
            P2Gamepad
        }

        private static readonly Color WindowColor = new Color(80, 80, 100);
        private static readonly Color KeyboardOptionColor = TeamUtility.DetermineTeamColor(Team.Creep);
        private static readonly Color PlayerOneColor = TeamUtility.DetermineTeamColor(GameContext.P1Team);
        private static readonly Color PlayerTwoColor = TeamUtility.DetermineTeamColor(GameContext.P2Team);

        public ControlMenuStates CurrentState { get; set; }

        private IMenu DeviceSelectMenu { get; }
        private IMenu InputRemapSelectMenu { get; }

        private IMenu MappingMenu { get; }
        private IRenderable MappingInfoWindow { get; }

        private IRenderable menuCursorSprite;

        public ControlConfigView()
        {
            //TODO Initialize menus
        }


        private IMenu CurrentMenu
        {
            get
            {
                switch (CurrentState)
                {
                    case ControlMenuStates.DeviceSelect:
                        return DeviceSelectMenu;
                    case ControlMenuStates.InputRemapSelect:
                        return InputRemapSelectMenu;
                    case ControlMenuStates.ListeningForInput:
                        return MappingMenu;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void SelectCurrentOption()
        {
            CurrentMenu.SelectOption();
        }

        private static IMenu GenerateConfigMenuForDevice(Devices device, IRenderable cursorSprite)
        {
            GameControlParser selectedParser;
            Color windowColor;

            switch (device)
            {
                case Devices.Keyboard:
                    selectedParser = GameDriver.KeyboardParser;
                    windowColor = KeyboardOptionColor;
                    break;
                case Devices.P1Gamepad:
                    selectedParser = GameDriver.P1GamepadParser;
                    windowColor = PlayerOneColor;
                    break;
                case Devices.P2Gamepad:
                    selectedParser = GameDriver.P2GamepadParser;
                    windowColor = PlayerTwoColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(device), device, null);
            }

            int inputs = Enum.GetValues(typeof(Input)).Length;
            MenuOption[] configOptions = new MenuOption[inputs];

            for (int i = 0; i < inputs; i++)
            {
                configOptions[i] = new RemapInputOption(selectedParser.Controller, (Input) i, windowColor);
            }

            return new VerticalMenu(configOptions, cursorSprite, KeyboardOptionColor);
        }

        private static IMenu GenerateDeviceMenu(IRenderable cursorSprite, Color windowColor)
        {
            TwoDimensionalMenu menu = new TwoDimensionalMenu(
                new MenuOption[,]
                {
                    {
                        new DeviceSelectOption("Keyboard Config", KeyboardOptionColor),
                        new DeviceSelectOption("Gamepad 1 Config", PlayerOneColor),
                        new DeviceSelectOption("Gamepad 2 Config", PlayerTwoColor)
                    }
                },
                cursorSprite,
                windowColor,
                TwoDimensionalMenu.CursorType.Pointer
            );

            return menu;
        }

        public void ToggleVisible()
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}