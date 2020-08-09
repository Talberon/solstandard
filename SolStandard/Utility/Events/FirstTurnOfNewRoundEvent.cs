using SolStandard.Containers.Components.World.SubContext;

namespace SolStandard.Utility.Events
{
    public class FirstTurnOfNewRoundEvent : IEvent
    {
        private readonly InitiativeContext initiativeContext;

        public FirstTurnOfNewRoundEvent(InitiativeContext initiativeContext)
        {
            this.initiativeContext = initiativeContext;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            initiativeContext.StartFirstTurnOfNewRound();
            Complete = true;
        }
    }
}