using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public abstract class GameControl
    {
        protected readonly PlayerIndex PlayerIndex;

        public int InputCounter { get; private set; }

        protected GameControl(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            InputCounter = 0;
        }

        public abstract bool Pressed { get; }

        public virtual bool Released
        {
            get { return !Pressed && InputCounter > 0; }
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