using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadY : GamePadControl
    {
        public GamepadY(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.Y == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.Y, new Vector2(iconSize));
        }
    }
}