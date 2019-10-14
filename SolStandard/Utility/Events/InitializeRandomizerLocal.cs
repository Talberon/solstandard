using System;
using System.Diagnostics;

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
            Trace.WriteLine($"New rng seed: {rngSeed}. Next value: {GameDriver.Random.Next()}");
            Complete = true;
        }
    }
}