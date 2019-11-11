using System;
using NLog;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class InitializeRandomizerNet : NetworkEvent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly int rngSeed;

        public InitializeRandomizerNet(int rngSeed)
        {
            this.rngSeed = rngSeed;
        }
        
        public override void Continue()
        {
            GameDriver.Random = new Random(rngSeed);
            Logger.Debug($"New rng seed: {rngSeed}. Next value: {GameDriver.Random.Next()}");
            Complete = true;
        }
    }
}