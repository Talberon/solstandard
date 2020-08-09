using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Inputs;

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

        public static void UpdateLastInputType(ControlType controlType)
        {
            _lastInput = controlType;
        }

        public static IRenderable GetInputIcon(Input inputType, int iconSize)
        {
            switch (_lastInput)
            {
                case ControlType.Keyboard:
                    return GameDriver.KeyboardParser.Controller.GetInput(inputType).GetInputIcon(iconSize);
                case ControlType.Gamepad:
                    GameControlParser activePlayerParser = (GlobalContext.ActivePlayer == PlayerIndex.One)
                        ? GameDriver.P1GamepadParser
                        : GameDriver.P2GamepadParser;

                    return activePlayerParser.Controller.GetInput(inputType).GetInputIcon(iconSize);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}