using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public abstract class GamePadControl: GameControl
    {
        protected readonly PlayerIndex PlayerIndex;

        public GamePadControl(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}