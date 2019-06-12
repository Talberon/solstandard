using System;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.Gamepad;
using SolStandard.Utility.Buttons.KeyboardInput;

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

        public static IRenderable GetInputIcon(Input inputType, float iconSize)
        {
            return GetInputIcon(inputType, new Vector2(iconSize));
        }

        public static IRenderable GetInputIcon(Input inputType, Vector2 iconSize)
        {
            switch (_lastInput)
            {
                case ControlType.Keyboard:
                    return Keyboard.GetInputIcon(inputType, iconSize);
                case ControlType.Gamepad:
                    return Gamepad.GetInputIcon(inputType, iconSize);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}