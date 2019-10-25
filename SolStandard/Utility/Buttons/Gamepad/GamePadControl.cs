using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public abstract class GamePadControl : GameControl
    {
        protected readonly PlayerIndex PlayerIndex;

        protected GamePadControl(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}