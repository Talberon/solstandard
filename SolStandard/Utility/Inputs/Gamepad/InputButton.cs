using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Inputs.Gamepad
{
    [Serializable]
    public class InputButton : GameControl
    {
        public static readonly IReadOnlyDictionary<Buttons, ButtonIcon> ButtonIcons =
            new Dictionary<Buttons, ButtonIcon>
            {
                {Buttons.A, ButtonIcon.A},
                {Buttons.B, ButtonIcon.B},
                {Buttons.X, ButtonIcon.X},
                {Buttons.Y, ButtonIcon.Y},
                {Buttons.DPadUp, ButtonIcon.DpadUp},
                {Buttons.DPadDown, ButtonIcon.DpadDown},
                {Buttons.DPadLeft, ButtonIcon.DpadLeft},
                {Buttons.DPadRight, ButtonIcon.DpadRight},
                {Buttons.LeftThumbstickUp, ButtonIcon.LeftStickUp},
                {Buttons.LeftThumbstickDown, ButtonIcon.LeftStickDown},
                {Buttons.LeftThumbstickLeft, ButtonIcon.LeftStickLeft},
                {Buttons.LeftThumbstickRight, ButtonIcon.LeftStickRight},
                {Buttons.Back, ButtonIcon.Windows},
                {Buttons.Start, ButtonIcon.Menu},
                {Buttons.RightThumbstickUp, ButtonIcon.RightStickUp},
                {Buttons.RightThumbstickDown, ButtonIcon.RightStickDown},
                {Buttons.RightThumbstickLeft, ButtonIcon.RightStickLeft},
                {Buttons.RightThumbstickRight, ButtonIcon.RightStickRight},
                {Buttons.LeftShoulder, ButtonIcon.Lb},
                {Buttons.LeftTrigger, ButtonIcon.Lt},
                {Buttons.RightShoulder, ButtonIcon.Rb},
                {Buttons.RightTrigger, ButtonIcon.Rt},
            };

        private readonly PlayerIndex playerIndex;
        private readonly Buttons button;

        public override bool Pressed => GamePad.GetState(playerIndex).IsButtonDown(button);

        public InputButton(PlayerIndex playerIndex, Buttons button)
        {
            this.playerIndex = playerIndex;
            this.button = button;
        }

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcons[button], new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is InputButton inputButton &&
                   (inputButton.playerIndex == playerIndex && inputButton.button == button);
        }

        public override int GetHashCode()
        {
            return (int) button ^ 5;
        }
    }
}