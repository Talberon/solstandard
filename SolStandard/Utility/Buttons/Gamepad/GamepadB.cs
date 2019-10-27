﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadB : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.B;
        
        public GamepadB(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.B == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.B, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadB;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.B;
        }
    }
}