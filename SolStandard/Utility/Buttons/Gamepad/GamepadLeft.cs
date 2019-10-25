﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeft : GamePadControl
    {
        public GamepadLeft(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).DPad.Left == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex).ThumbSticks.Left.X < (-ControlMapper.StickDeadzone);

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.DpadLeft, new Vector2(iconSize));
        }
    }
}