using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeftBumper: GamePadControl
    {
        public GamepadLeftBumper(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.LeftShoulder == ButtonState.Pressed;
    }
}