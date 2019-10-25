using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadX : GamePadControl
    {
        public GamepadX(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.X == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.X, new Vector2(iconSize));
        }
    }
}