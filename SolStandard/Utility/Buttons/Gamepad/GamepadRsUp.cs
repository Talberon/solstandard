using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRsUp : GamePadControl
    {
        public GamepadRsUp(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).ThumbSticks.Right.Y > ControlMapper.StickDeadzone;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.RightStick, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRsUp;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.RightStick;
        }
    }
}