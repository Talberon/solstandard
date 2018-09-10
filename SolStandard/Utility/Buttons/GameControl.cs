using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons
{
    public abstract class GameControl
    {
        protected readonly PlayerIndex PlayerIndex;

        protected GameControl(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }
        
        private int inputCounter;

        public abstract bool Pressed();
        public abstract bool Released();

        protected GameControl()
        {
            inputCounter = 0;
        }

        public void IncrementInputCounter()
        {
            inputCounter++;
        }

        public void ResetInputCounter()
        {
            inputCounter = 0;
        }

        public int GetInputCounter()
        {
            return inputCounter;
        }
    }
}