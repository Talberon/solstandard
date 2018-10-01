namespace SolStandard.Utility.Events
{
    public class WaitFramesEvent : IEvent
    {
        public bool Complete { get; private set; }
        private int framesRemaining;

        public WaitFramesEvent(int waitTimeInFrames)
        {
            Complete = false;
            framesRemaining = waitTimeInFrames;
        }

        public void Continue()
        {
            if (framesRemaining > 0)
            {
                framesRemaining--;
            }
            else
            {
                Complete = true;
            }
        }
    }
}