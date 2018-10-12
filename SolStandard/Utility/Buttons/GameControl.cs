using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons
{
    public abstract class GameControl
    {
        protected readonly PlayerIndex PlayerIndex;

        public int InputCounter { get; private set; }

        protected GameControl(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public abstract bool Pressed();
        public abstract bool Released();

        protected GameControl()
        {
            InputCounter = 0;
        }

        public void IncrementInputCounter()
        {
            InputCounter++;
        }

        public void ResetInputCounter()
        {
            InputCounter = 0;
        }
    }
}