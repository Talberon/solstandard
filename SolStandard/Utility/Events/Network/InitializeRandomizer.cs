using System;

namespace SolStandard.Utility.Events.Network
{
    public class InitializeRandomizer : NetworkEvent
    {
        private readonly int rngSeed;

        public InitializeRandomizer(int rngSeed)
        {
            this.rngSeed = rngSeed;
        }
        
        public override void Continue()
        {
            GameDriver.Random = new Random(rngSeed);
            Complete = true;
        }
    }
}