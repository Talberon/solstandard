using System;
using System.Diagnostics;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class InitializeRandomizerNet : NetworkEvent
    {
        private readonly int rngSeed;

        public InitializeRandomizerNet(int rngSeed)
        {
            this.rngSeed = rngSeed;
        }
        
        public override void Continue()
        {
            GameDriver.Random = new Random(rngSeed);
            Trace.WriteLine($"New rng seed: {rngSeed}. Next value: {GameDriver.Random.Next()}");
            Complete = true;
        }
    }
}