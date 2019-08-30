﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadB: GamePadControl
    {
        public GamepadB(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.B == ButtonState.Pressed;
    }
}