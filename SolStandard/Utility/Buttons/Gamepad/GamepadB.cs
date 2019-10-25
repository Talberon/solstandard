using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadB : GamePadControl
    {
        public GamepadB(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.B == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.B, new Vector2(iconSize));
        }
    }
}