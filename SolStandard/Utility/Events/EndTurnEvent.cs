using SolStandard.Containers.Contexts;

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
            GameMapContext.FinishTurn(skipProcs);

            Complete = true;
        }
    }
}