using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRsRight : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.RightStickRight;

        public GamepadRsRight(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).ThumbSticks.Right.X > (ControlMapper.StickDeadzone);

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.RightStick, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRsRight;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.RightStick;
        }
    }
}