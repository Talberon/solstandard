using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Inputs.Joystick
{
    public enum JoystickButton
    {
        BottomFaceButton,
        RightFaceButton,
        Unknown2,
        LeftFaceButton,
        TopFaceButton,
        Unknown5,
        LeftShoulder,
        RightShoulder,
        Unknown8,
        Unknown9,
        Unknown10,
        Start,
        Unknown12,
        LeftStickIn,
        RightStickIn,
        Unknown15,
        Back,
    }

    [Serializable]
    public class InputJoystick : GameControl
    {
        private readonly PlayerIndex playerIndex;
        private readonly JoystickButton button;


        public static readonly IReadOnlyDictionary<JoystickButton, ButtonIcon> ButtonIcons =
            new Dictionary<JoystickButton, ButtonIcon>
            {
                {JoystickButton.BottomFaceButton, ButtonIcon.A},
                {JoystickButton.RightFaceButton, ButtonIcon.B},
                {JoystickButton.LeftFaceButton, ButtonIcon.X},
                {JoystickButton.TopFaceButton, ButtonIcon.Y},
                {JoystickButton.Back, ButtonIcon.Windows},
                {JoystickButton.Start, ButtonIcon.Menu},
                {JoystickButton.LeftShoulder, ButtonIcon.Lb},
                {JoystickButton.RightShoulder, ButtonIcon.Rb},
            };

        public InputJoystick(PlayerIndex playerIndex, JoystickButton button)
        {
            this.playerIndex = playerIndex;
            this.button = button;
        }

        public override bool Pressed
        {
            get
            {
                if (!Microsoft.Xna.Framework.Input.Joystick.GetCapabilities((int) playerIndex).IsConnected)
                {
                    return false;
                }

                return Microsoft.Xna.Framework.Input.Joystick.GetState((int) playerIndex).Buttons[(int) button] ==
                       ButtonState.Pressed;
            }
        }

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcons[button], new Vector2(iconSize));
        }
    }
}