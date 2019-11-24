using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Inputs.Joystick
{
    public enum JoystickAxisInput
    {
        LeftThumbstickUp, //Negative
        LeftThumbstickDown, //Positive
        LeftThumbstickLeft, //Negative
        LeftThumbstickRight, //Positive
        RightThumbstickUp, //Negative
        RightThumbstickDown, //Positive
        RightThumbstickLeft, //Negative
        RightThumbstickRight, //Positive
        LeftTrigger, //Negative?
        RightTrigger //Positive?
    }

    [Serializable]
    public class InputJoystickAxis : GameControl
    {
        private static readonly IReadOnlyDictionary<JoystickAxisInput, ButtonIcon> ButtonIcons =
            new Dictionary<JoystickAxisInput, ButtonIcon>
            {
                {JoystickAxisInput.LeftThumbstickUp, ButtonIcon.LeftStickUp},
                {JoystickAxisInput.LeftThumbstickDown, ButtonIcon.LeftStickDown},
                {JoystickAxisInput.LeftThumbstickLeft, ButtonIcon.LeftStickLeft},
                {JoystickAxisInput.LeftThumbstickRight, ButtonIcon.LeftStickRight},
                {JoystickAxisInput.RightThumbstickUp, ButtonIcon.RightStickUp},
                {JoystickAxisInput.RightThumbstickDown, ButtonIcon.RightStickDown},
                {JoystickAxisInput.RightThumbstickLeft, ButtonIcon.RightStickLeft},
                {JoystickAxisInput.RightThumbstickRight, ButtonIcon.RightStickRight},
                {JoystickAxisInput.LeftTrigger, ButtonIcon.Lt},
                {JoystickAxisInput.RightTrigger, ButtonIcon.Rt},
            };

        private enum Axes
        {
            LeftStickHorizontal,
            LeftStickVertical,
            RightStickHorizontal,
            RightStickVertical,
            AnalogTriggers
        }

        private const int Deadzone = 10000;
        private readonly PlayerIndex playerIndex;
        private readonly JoystickAxisInput input;

        public InputJoystickAxis(PlayerIndex playerIndex, JoystickAxisInput input)
        {
            this.playerIndex = playerIndex;
            this.input = input;
        }

        public override bool Pressed
        {
            get
            {
                if (!Microsoft.Xna.Framework.Input.Joystick.GetCapabilities((int) playerIndex).IsConnected)
                {
                    return false;
                }

                int[] axes = Microsoft.Xna.Framework.Input.Joystick.GetState((int) playerIndex).Axes;
                int axisValue = (axes.Contains((int) GetAxis(input))) ? axes[(int) GetAxis(input)] : 0;
                return input switch
                {
                    JoystickAxisInput.LeftThumbstickUp => (axisValue < -Deadzone),
                    JoystickAxisInput.LeftThumbstickDown => (axisValue > Deadzone),
                    JoystickAxisInput.LeftThumbstickLeft => (axisValue < -Deadzone),
                    JoystickAxisInput.LeftThumbstickRight => (axisValue > Deadzone),
                    JoystickAxisInput.RightThumbstickUp => (axisValue < -Deadzone),
                    JoystickAxisInput.RightThumbstickDown => (axisValue > Deadzone),
                    JoystickAxisInput.RightThumbstickLeft => (axisValue < -Deadzone),
                    JoystickAxisInput.RightThumbstickRight => (axisValue > Deadzone),
                    JoystickAxisInput.LeftTrigger => (axisValue < -Deadzone),
                    JoystickAxisInput.RightTrigger => (axisValue > Deadzone),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private Axes GetAxis(JoystickAxisInput joystickAxisInput)
        {
            return joystickAxisInput switch
            {
                JoystickAxisInput.LeftThumbstickUp => Axes.LeftStickVertical,
                JoystickAxisInput.LeftThumbstickDown => Axes.LeftStickVertical,
                JoystickAxisInput.LeftThumbstickLeft => Axes.LeftStickHorizontal,
                JoystickAxisInput.LeftThumbstickRight => Axes.LeftStickHorizontal,
                JoystickAxisInput.RightThumbstickUp => Axes.RightStickVertical,
                JoystickAxisInput.RightThumbstickDown => Axes.RightStickVertical,
                JoystickAxisInput.RightThumbstickLeft => Axes.RightStickHorizontal,
                JoystickAxisInput.RightThumbstickRight => Axes.RightStickHorizontal,
                JoystickAxisInput.LeftTrigger => Axes.AnalogTriggers,
                JoystickAxisInput.RightTrigger => Axes.AnalogTriggers,
                _ => throw new ArgumentOutOfRangeException(nameof(joystickAxisInput), joystickAxisInput, null)
            };
        }

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcons[input], new Vector2(iconSize));
        }
    }
}