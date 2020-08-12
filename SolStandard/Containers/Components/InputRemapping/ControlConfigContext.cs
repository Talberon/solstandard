using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Menu;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Inputs.Gamepad;
using SolStandard.Utility.Inputs.KeyboardInput;

namespace SolStandard.Containers.Components.InputRemapping
{
    public class ControlConfigContext 
    {
        public enum ControlMenuState
        {
            DeviceSelect,
            InputRemapSelect,
            ListeningForInput
        }

        public enum Device
        {
            Keyboard,
            P1Gamepad,
            P2Gamepad
        }

        public const string KeyboardConfigFileName = "KeyboardConfig";
        public const string P1GamepadConfigFileName = "P1GamepadConfig";
        public const string P2GamepadConfigFileName = "P2GamepadConfig";

        private readonly ControlConfigView view;
        private IController metakeyboard;
        private IController metaP1Gamepad;
        private IController metaP2Gamepad;
        private GlobalContext.GameState previousGameState;

        private Device currentListeningDevice;
        private Input currentListeningInput;

        public IUserInterface View => view;
        public ControlMenuState CurrentState => view.CurrentState;

        private const int CooldownInterval = 15;
        private int frameCooldown;

        public ControlConfigContext(ControlConfigView configView)
        {
            view = configView;
            InitializeMetaControls();
            view.CurrentState = ControlMenuState.DeviceSelect;
            currentListeningDevice = Device.Keyboard;
            currentListeningInput = Input.None;
            frameCooldown = 0;
        }
        
        #region MenuControls

        public void OpenRemapMenu(Device device)
        {
            IController controller = device switch
            {
                Device.Keyboard => metakeyboard,
                Device.P1Gamepad => metaP1Gamepad,
                Device.P2Gamepad => metaP2Gamepad,
                _ => throw new ArgumentOutOfRangeException(nameof(device), device, null)
            };

            currentListeningDevice = device;
            view.OpenInputRemapMenu(device, controller);
        }

        public void SelectCurrentOption()
        {
            view.SelectCurrentOption();
        }

        public void Cancel()
        {
            if (frameCooldown > 0) return;

            AssetManager.MapUnitCancelSFX.Play();

            if (view.CurrentState == ControlMenuState.DeviceSelect)
            {
                CloseMenu();
            }
            else
            {
                view.GoToPreviousMenu();
            }
        }

        public void OpenMenu()
        {
            if (GlobalContext.CurrentGameState == GlobalContext.GameState.ControlConfig) return;

            previousGameState = GlobalContext.CurrentGameState;
            GlobalContext.CurrentGameState = GlobalContext.GameState.ControlConfig;
            view.CurrentState = ControlMenuState.DeviceSelect;
        }

        private void CloseMenu()
        {
            AssetManager.MapUnitCancelSFX.Play();
            GlobalContext.CurrentGameState = previousGameState;
        }

        public void MoveMenuCursor(MenuCursorDirection direction)
        {
            view.CurrentMenu.MoveMenuCursor(direction);
        }

        #endregion

        public void ResetMetaController(Device deviceType)
        {
            switch (deviceType)
            {
                case Device.Keyboard:
                    metakeyboard = new KeyboardController();
                    break;
                case Device.P1Gamepad:
                    metaP1Gamepad = new GamepadController(PlayerIndex.One);
                    break;
                case Device.P2Gamepad:
                    metaP2Gamepad = new GamepadController(PlayerIndex.Two);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OpenRemapMenu(currentListeningDevice);
        }

        public void StartListeningForInput(Device device, Input input)
        {
            currentListeningDevice = device;
            currentListeningInput = input;
            view.CurrentState = ControlMenuState.ListeningForInput;
            frameCooldown = CooldownInterval;
        }

        public void Update()
        {
            frameCooldown--;

            if (view.CurrentState != ControlMenuState.ListeningForInput || currentListeningInput == Input.None ||
                frameCooldown > 0) return;

            if (frameCooldown == -CooldownInterval * 10)
            {
                Cancel();
                return;
            }

            switch (currentListeningDevice)
            {
                case Device.Keyboard:
                    ListenForKeyboardInput(currentListeningInput);
                    break;
                case Device.P1Gamepad:
                    ListenForGamepadInputFromPlayer(PlayerIndex.One, currentListeningInput);
                    break;
                case Device.P2Gamepad:
                    ListenForGamepadInputFromPlayer(PlayerIndex.Two, currentListeningInput);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void ListenForKeyboardInput(Input inputToMap)
        {
            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                if (!InputKey.KeyIcons.ContainsKey(key)) continue;

                UpdateKeyboardControls(inputToMap, key);
                OpenRemapMenu(currentListeningDevice);
                AssetManager.CoinSFX.Play();
                frameCooldown = CooldownInterval;
                break;
            }
        }

        private void ListenForGamepadInputFromPlayer(PlayerIndex playerIndex, Input inputToMap)
        {
            foreach (Buttons pressedButton in GetPressedButtons(GamePad.GetState(playerIndex)))
            {
                if (!InputButton.ButtonIcons.ContainsKey(pressedButton)) continue;

                UpdateGamepadControls(playerIndex, inputToMap, pressedButton);
                OpenRemapMenu(currentListeningDevice);
                AssetManager.CoinSFX.Play();
                frameCooldown = CooldownInterval;
                break;
            }
        }

        private static IEnumerable<Buttons> GetPressedButtons(GamePadState gamePadState)
        {
            return Enum.GetValues(typeof(Buttons))
                .Cast<Buttons>()
                .Where(gamePadState.IsButtonDown)
                .ToList();
        }

        public void SaveControlMappings()
        {
            if (InputsAreValid())
            {
                GameDriver.KeyboardParser = new GameControlParser(metakeyboard);
                GameDriver.P1GamepadParser = new GameControlParser(metaP1Gamepad);
                GameDriver.P2GamepadParser = new GameControlParser(metaP2Gamepad);
                GameDriver.InitializeControlMappers(GlobalContext.P1Team);
                InitializeMetaControls();
                GlobalHUDUtils.AddNotification("Saved control inputs.");
                frameCooldown = CooldownInterval;
                view.CurrentState = ControlMenuState.DeviceSelect;

                GameDriver.FileIO.Save(KeyboardConfigFileName, GameDriver.KeyboardParser.Controller);
                GameDriver.FileIO.Save(P1GamepadConfigFileName, GameDriver.P1GamepadParser.Controller);
                GameDriver.FileIO.Save(P2GamepadConfigFileName, GameDriver.P2GamepadParser.Controller);
            }
            else
            {
                AssetManager.WarningSFX.Play();
                GlobalHUDUtils.AddNotification("All controls must be unique!");
            }
        }

        private bool InputsAreValid()
        {
            return AllInputsAreUnique(metakeyboard.Inputs.Values) &&
                   AllInputsAreUnique(metaP1Gamepad.Inputs.Values) &&
                   AllInputsAreUnique(metaP2Gamepad.Inputs.Values);
        }

        private static bool AllInputsAreUnique(IReadOnlyCollection<GameControl> enumerable)
        {
            return enumerable.Distinct().Count() == enumerable.Count;
        }

        private void UpdateKeyboardControls(Input controlType, Keys keyToMap)
        {
            GameControl keyControl = new InputKey(keyToMap);
            metakeyboard.RemapControl(controlType, keyControl);
        }

        private void UpdateGamepadControls(PlayerIndex playerIndex, Input controlType, Buttons buttonToMap)
        {
            GameControl gamepadControl = new InputButton(playerIndex, buttonToMap);

            switch (playerIndex)
            {
                case PlayerIndex.One:
                    metaP1Gamepad.RemapControl(controlType, gamepadControl);
                    break;
                case PlayerIndex.Two:
                    metaP2Gamepad.RemapControl(controlType, gamepadControl);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
            }
        }

        private void InitializeMetaControls()
        {
            metakeyboard = KeyboardController.From((KeyboardController) GameDriver.KeyboardParser.Controller);
            metaP1Gamepad = GamepadController.From((GamepadController) GameDriver.P1GamepadParser.Controller);
            metaP2Gamepad = GamepadController.From((GamepadController) GameDriver.P2GamepadParser.Controller);
        }
    }
}