using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeftBumper : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Lb;

        public GamepadLeftBumper(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.LeftShoulder == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.Lb, new Vector2(iconSize));
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadLeftBumper;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) ButtonIcon.Lb;
        }
    }
}