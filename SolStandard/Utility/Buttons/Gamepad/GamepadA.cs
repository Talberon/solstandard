using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadA : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.A;

        public GamepadA(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.A == ButtonState.Pressed;


        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.A, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadA;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.A;
        }
    }
}