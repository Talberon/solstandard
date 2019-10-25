using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRightTrigger : GamePadControl
    {
        public GamepadRightTrigger(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Triggers.Right > ControlMapper.TriggerDeadzone;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.Rt, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRightTrigger;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.Rt;
        }
    }
}