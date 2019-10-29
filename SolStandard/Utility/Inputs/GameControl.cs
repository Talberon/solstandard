using System;

namespace SolStandard.Utility.Inputs
{
    [Serializable]
    public abstract class GameControl
    {
        public int InputCounter { get; private set; }

        protected GameControl()
        {
            InputCounter = 0;
        }

        public abstract bool Pressed { get; }
        public abstract IRenderable GetInputIcon(int iconSize);

        public bool Released => !Pressed && InputCounter > 0;

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