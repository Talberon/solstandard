using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Inputs.Joystick
{
    [Serializable]
    public class InputJoystickDPad : GameControl
    {
        private static readonly IReadOnlyDictionary<Direction, ButtonIcon> ButtonIcons =
            new Dictionary<Direction, ButtonIcon>
            {
                {Direction.Up, ButtonIcon.DpadUp},
                {Direction.Down, ButtonIcon.DpadDown},
                {Direction.Left, ButtonIcon.DpadLeft},
                {Direction.Right, ButtonIcon.DpadRight},
                {Direction.None, ButtonIcon.Dpad},
            };

        private readonly PlayerIndex playerIndex;
        private readonly Direction direction;
        private const int DPadIndex = 0;

        private ButtonState DPadState
        {
            get
            {
                if (!Microsoft.Xna.Framework.Input.Joystick.GetCapabilities((int) playerIndex).IsConnected)
                {
                    return ButtonState.Released;
                }

                JoystickHat joystickHat =
                    Microsoft.Xna.Framework.Input.Joystick.GetState((int) playerIndex).Hats[DPadIndex];
                switch (direction)
                {
                    case Direction.Up:
                        return joystickHat.Up;
                    case Direction.Right:
                        return joystickHat.Right;
                    case Direction.Down:
                        return joystickHat.Down;
                    case Direction.Left:
                        return joystickHat.Left;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override bool Pressed => DPadState == ButtonState.Pressed;

        public InputJoystickDPad(PlayerIndex playerIndex, Direction direction)
        {
            this.direction = direction;
            this.playerIndex = playerIndex;
        }

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcons[direction], new Vector2(iconSize));
        }
    }
}