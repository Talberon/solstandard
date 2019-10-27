using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers.View;
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

        private readonly ControlConfigView view;
        private IController metakeyboard;
        private IController metaP1Gamepad;
        private IController metaP2Gamepad;

        public ControlConfigContext(ControlConfigView configView)
        {
            view = configView;
            InitializeMetaControls();
            view.CurrentState = ControlConfigView.ControlMenuStates.DeviceSelect;
        }

        public void SelectCurrentOption()
        {
            view.SelectCurrentOption();
        }

        //TODO Continuously check this during the Listening state (in View)
        public void ListenForKeyboardInput(Input inputToMap)
        {
            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                if (!InputKey.KeyIcons.ContainsKey(key)) continue;

                UpdateKeyboardControls(inputToMap, key);
                view.CurrentState = ControlConfigView.ControlMenuStates.InputRemapSelect;
                break;
            }
        }

        //TODO Continuously check this during the Listening state (in View)
        public void ListenForGamepadInputFromPlayer(PlayerIndex playerIndex, Input inputToMap)
        {
            foreach (Buttons pressedButton in GetPressedButtons(GamePad.GetState(playerIndex)))
            {
                if (!InputButton.ButtonIcons.ContainsKey(pressedButton)) continue;

                UpdateGamepadControls(playerIndex, inputToMap, pressedButton);
                view.CurrentState = ControlConfigView.ControlMenuStates.InputRemapSelect;
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