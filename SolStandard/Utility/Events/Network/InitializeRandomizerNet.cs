using System;

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
            Complete = true;
        }
    }
}