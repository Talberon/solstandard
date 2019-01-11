namespace SolStandard.Utility.Buttons
{
    public abstract class GameControl
    {
        public int InputCounter { get; private set; }

        protected GameControl()
        {
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