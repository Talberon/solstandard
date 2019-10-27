﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRight : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Right;

        public GamepadRight(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).DPad.Right == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex).ThumbSticks.Left.X > (ControlMapper.StickDeadzone);

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.DpadRight, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRight;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.DpadRight;
        }
    }
}