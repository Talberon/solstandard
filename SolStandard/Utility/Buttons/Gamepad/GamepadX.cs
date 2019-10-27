using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadX : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.X;

        public GamepadX(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.X == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.X, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadX;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.X;
        }
    }
}