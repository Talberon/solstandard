using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRightBumper: GamePadControl
    {
        public GamepadRightBumper(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.RightShoulder == ButtonState.Pressed;
    }
}