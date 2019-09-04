using SolStandard.Containers.Contexts;
using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class EndTurnEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly bool skipProcs;

        public EndTurnEvent(bool skipProcs = false)
        {
            this.skipProcs = skipProcs;
        }

        public void Continue()
        {
            //IMPORTANT Do not allow tiles that have been triggered to trigger again or the risk of soft-locking via infinite triggers can occur
            if (!GameMapContext.TriggerEffectTiles(EffectTriggerTime.EndOfTurn, false))
            {
                GameMapContext.FinishTurn(skipProcs);
            }

            Complete = true;
        }
    }
}