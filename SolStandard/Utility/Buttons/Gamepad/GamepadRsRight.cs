using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRsRight: GamePadControl
    {
        public GamepadRsRight(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).ThumbSticks.Right.X > (ControlMapper.StickDeadzone);
    }
}