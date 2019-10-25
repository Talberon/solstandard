using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeftBumper : GamePadControl
    {
        public GamepadLeftBumper(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.LeftShoulder == ButtonState.Pressed;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcon.Lb, new Vector2(iconSize));
        }
    }
}