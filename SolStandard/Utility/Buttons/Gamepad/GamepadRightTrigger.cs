using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRightTrigger: GamePadControl
    {
        public GamepadRightTrigger(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Triggers.Right > ControlMapper.TriggerDeadzone;
    }
}