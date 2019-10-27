using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadUp : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Up;

        public GamepadUp(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).DPad.Up == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y > ControlMapper.StickDeadzone;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.DpadUp, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadUp;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.DpadUp;
        }
    }
}