using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.DXGI;
using SolStandard.Containers.View;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.Gamepad;
using SolStandard.Utility.Buttons.KeyboardInput;

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

        private ControlConfigView view;
        private IController metakeyboard;
        private IController metaP1Gamepad;
        private IController metaP2Gamepad;

        private enum ControlMenuStates
        {
            DeviceSelect,
            InputSelect,
            MappingInput
        }

        private ControlMenuStates currentState;

        public ControlConfigContext(ControlConfigView configView)
        {
            view = configView;
            metakeyboard = new KeyboardController();
            metaP1Gamepad = new GamepadController(PlayerIndex.One);
            metaP2Gamepad = new GamepadController(PlayerIndex.Two);
            currentState = ControlMenuStates.DeviceSelect;
        }

        public void ListenForKeyboardInput(Input inputToMap)
        {
            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                if (InputKey.KeyIcons.ContainsKey(key))
                {
                    //TODO Capture key, map it, and go back to config key list menu
                    UpdateKeyboardControls(inputToMap, key);
                    break;
                }
            }
        }

        public void ListenForGamepadInputFromPlayer(PlayerIndex playerIndex, Input inputToMap)
        {
        }

        private void SaveControlMappings()
        {
            GameDriver.KeyboardParser = new GameControlParser(metakeyboard);
            GameDriver.P1GamepadParser = new GameControlParser(metakeyboard);
            GameDriver.P2GamepadParser = new GameControlParser(metakeyboard);
            GameDriver.InitializeControlMappers(GameContext.P1Team);

            //TODO Save to disk
        }

        private void UpdateKeyboardControls(Input controlType, Keys keyToMap)
        {
            GameControl keyControl = new InputKey(keyToMap);
            metakeyboard.RemapControl(controlType, keyControl);
        }

        private void UpdateGamepadControls(PlayerIndex playerIndex, Input controlType, GamepadInputs buttonToMap)
        {
            GameControl gamepadControl = GamepadControlFactory.GetGamepadControl(playerIndex, buttonToMap);

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
    }
}