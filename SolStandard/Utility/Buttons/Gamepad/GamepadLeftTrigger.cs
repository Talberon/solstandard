using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeftTrigger: GamePadControl
    {
        public GamepadLeftTrigger(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get { return GamePad.GetState(PlayerIndex).Triggers.Left > ControlMapper.TriggerDeadzone; }
        }
    }
}