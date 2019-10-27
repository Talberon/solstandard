using System;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Inputs.Gamepad;
using SolStandard.Utility.Inputs.KeyboardInput;

namespace SolStandard.Utility.Assets
{
    public enum ControlType
    {
        Keyboard,
        Gamepad
    }

    public static class InputIconProvider
    {
        private static ControlType _lastInput = ControlType.Keyboard;
        private static readonly IController Keyboard = new KeyboardController();
        private static readonly IController Gamepad = new GamepadController(PlayerIndex.Four);

        public static void UpdateLastInputType(ControlType controlType)
        {
            _lastInput = controlType;
        }

        public static IRenderable GetInputIcon(Input inputType, int iconSize)
        {
            switch (_lastInput)
            {
                case ControlType.Keyboard:
                    return Keyboard.GetInput(inputType).GetInputIcon(iconSize);
                case ControlType.Gamepad:
                    return Gamepad.GetInput(inputType).GetInputIcon(iconSize);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}