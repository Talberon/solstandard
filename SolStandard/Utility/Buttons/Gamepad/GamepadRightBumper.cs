using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRightBumper : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Rb;

        public GamepadRightBumper(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.RightShoulder == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.Rb, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRightBumper;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.Rb;
        }
    }
}