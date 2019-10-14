using System;

namespace SolStandard.Utility.Events
{
    public class InitializeRandomizerLocal : IEvent
    {
        private readonly int rngSeed;
        public bool Complete { get; private set; }

        public InitializeRandomizerLocal(int rngSeed)
        {
            this.rngSeed = rngSeed;
        }

        public void Continue()
        {
            GameDriver.Random = new Random(rngSeed);
            Complete = true;
        }
    }
}