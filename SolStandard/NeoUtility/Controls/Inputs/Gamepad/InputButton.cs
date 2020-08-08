using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.NeoUtility.Monogame.Assets;

namespace SolStandard.NeoUtility.Controls.Inputs.Gamepad
{
    [Serializable]
    public class InputButton : GameControl
    {
        public static readonly IReadOnlyDictionary<Buttons, GamepadIcon> GamepadIcons =
            new Dictionary<Buttons, GamepadIcon>
            {
                {Buttons.A, GamepadIcon.A},
                {Buttons.B, GamepadIcon.B},
                {Buttons.X, GamepadIcon.X},
                {Buttons.Y, GamepadIcon.Y},
                {Buttons.DPadUp, GamepadIcon.DpadUp},
                {Buttons.DPadDown, GamepadIcon.DpadDown},
                {Buttons.DPadLeft, GamepadIcon.DpadLeft},
                {Buttons.DPadRight, GamepadIcon.DpadRight},
                {Buttons.LeftThumbstickUp, GamepadIcon.LeftStickUp},
                {Buttons.LeftThumbstickDown, GamepadIcon.LeftStickDown},
                {Buttons.LeftThumbstickLeft, GamepadIcon.LeftStickLeft},
                {Buttons.LeftThumbstickRight, GamepadIcon.LeftStickRight},
                {Buttons.Back, GamepadIcon.Windows},
                {Buttons.Start, GamepadIcon.Menu},
                {Buttons.RightThumbstickUp, GamepadIcon.RightStickUp},
                {Buttons.RightThumbstickDown, GamepadIcon.RightStickDown},
                {Buttons.RightThumbstickLeft, GamepadIcon.RightStickLeft},
                {Buttons.RightThumbstickRight, GamepadIcon.RightStickRight},
                {Buttons.LeftShoulder, GamepadIcon.Lb},
                {Buttons.LeftTrigger, GamepadIcon.Lt},
                {Buttons.RightShoulder, GamepadIcon.Rb},
                {Buttons.RightTrigger, GamepadIcon.Rt},
            };

        private readonly PlayerIndex playerIndex;
        private readonly Buttons button;

        public override bool Pressed => GamePad.GetState(playerIndex).IsButtonDown(button);

        public GamepadIcon InputIcon => GamepadIcons[button];

        public InputButton(PlayerIndex playerIndex, Buttons button)
        {
            this.playerIndex = playerIndex;
            this.button = button;
        }

        // public override IRenderable GetInputIcon(int iconSize)
        // {
        //     return GamepadIconProvider.GetButton(GamepadIcons[button], new Vector2(iconSize));
        // }

        public override bool Equals(object? obj)
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