using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeftTrigger : GamePadControl
    {
        public GamepadLeftTrigger(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Triggers.Left > ControlMapper.TriggerDeadzone;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.Lt, new Vector2(iconSize));
        }
    }
}