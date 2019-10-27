using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeftTrigger : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Lt;

        public GamepadLeftTrigger(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Triggers.Left > ControlMapper.TriggerDeadzone;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.Lt, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadLeftTrigger;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.Lt;
        }
    }
}