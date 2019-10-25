﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadY : GamePadControl
    {
        public GamepadY(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.Y == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.Y, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadY;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.Y;
        }
    }
}