using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadSelect : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Select;

        public GamepadSelect(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.Back == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.Windows, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadSelect;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.Windows;
        }
    }
}