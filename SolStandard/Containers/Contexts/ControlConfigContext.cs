using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers.View;
using SolStandard.HUD.Menu;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Inputs.Gamepad;
using SolStandard.Utility.Inputs.KeyboardInput;

namespace SolStandard.Containers.Contexts
{
    public class ControlConfigContext
    {
        /*
         * TODO Flow:
         * Open the controller select menu first
         * Select one of 3 controllers: Keyboard/P1Gamepad/P2Gamepad
         * Keyboard:
         *     List all keyboard controller options and a Save and Cancel option
         *     Select a config option:
         *         Present a window that will accept a keyboard input if it matches one of the available keys
         *         After inputting a key, add that to a temporary IController in the context
         *         Return to the option list menu and update the icons for the options to match the meta-controller's current inputs
         *         Maintain that IController in the keyboard options until it is Saved
         *         When Saved, validate that all controls in the IController are unique (same key can't be used twice) and none are empty
         *         If valid, replace the current Keyboard Controller with the new one and exit the menu
         *
         * Gamepad:
         *     List all gamepad controller options and a Save and Cancel option
         *     Select a config option:
         *         Present a window that will accept a gamepad input ONLY from the active controller
         *         After inputting something, add that option to a temporary IController in the context
         *         Return to the option list menu and update the icons for the options to match the meta-controller's current inputs
         *         Maintain that IController in the gamepad options until it is Saved
         *         When Saved, validate that all controls in the IController are unique and none are empty
         *         If valid, replace the current player's GamepadController with the new one and exit the menu
         */


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

        private readonly ControlConfigView view;
        private IController metakeyboard;
        private IController metaP1Gamepad;
        private IController metaP2Gamepad;
        private GameContext.GameState previousGameState;

        private Device currentListeningDevice;
        private Input currentListeningInput;

        public IUserInterface View => view;

        public ControlConfigContext(ControlConfigView configView)
        {
            view = configView;
            InitializeMetaControls();
            view.CurrentState = ControlMenuState.DeviceSelect;
            currentListeningDevice = Device.Keyboard;
            currentListeningInput = Input.None;
        }

        #region MenuControls

        public void OpenRemapMenu(Device device)
        {
            IController controller;

            switch (device)
            {
                case Device.Keyboard:
                    controller = metakeyboard;
                    break;
                case Device.P1Gamepad:
                    controller = metaP1Gamepad;
                    break;
                case Device.P2Gamepad:
                    controller = metaP2Gamepad;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(device), device, null);
            }

            view.OpenInputRemapMenu(device, controller);
        }

        public void SelectCurrentOption()
        {
            view.SelectCurrentOption();
        }

        public void Cancel()
        {
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
            if (GameContext.CurrentGameState == GameContext.GameState.ControlConfig) return;

            previousGameState = GameContext.CurrentGameState;
            GameContext.CurrentGameState = GameContext.GameState.ControlConfig;
        }

        private void CloseMenu()
        {
            AssetManager.MapUnitCancelSFX.Play();
            GameContext.CurrentGameState = previousGameState;
        }

        public void MoveMenuCursor(MenuCursorDirection direction)
        {
            view.CurrentMenu.MoveMenuCursor(direction);
        }

        #endregion


        public void StartListeningForInput(Device device, Input input)
        {
            currentListeningDevice = device;
            currentListeningInput = input;
            view.CurrentState = ControlMenuState.ListeningForInput;
        }

        public void Update()
        {
            if (view.CurrentState != ControlMenuState.ListeningForInput || currentListeningInput == Input.None) return;

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

        private void SaveControlMappings()
        {
            GameDriver.KeyboardParser = new GameControlParser(metakeyboard);
            GameDriver.P1GamepadParser = new GameControlParser(metakeyboard);
            GameDriver.P2GamepadParser = new GameControlParser(metakeyboard);
            GameDriver.InitializeControlMappers(GameContext.P1Team);
            InitializeMetaControls();
            GlobalHudView.AddNotification("Saved control inputs.");

            //TODO Save to disk
        }

        private void UpdateKeyboardControls(Input controlType, Keys keyToMap)
        {
            GameControl keyControl = new InputKey(keyToMap);
            metakeyboard.RemapControl(controlType, keyControl);
        }

        private void UpdateGamepadControls(PlayerIndex playerIndex, Input controlType, Buttons buttonToMap)
        {
            //TODO Consider multiple mappings to the same input
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
            metakeyboard = new KeyboardController();
            metaP1Gamepad = new GamepadController(PlayerIndex.One);
            metaP2Gamepad = new GamepadController(PlayerIndex.Two);
        }
    }
}