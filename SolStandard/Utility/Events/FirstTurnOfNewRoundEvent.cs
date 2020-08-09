using SolStandard.Containers.Components.World.SubContext;
using SolStandard.Containers.Components.World.SubContext.Initiative;

namespace SolStandard.Utility.Events
{
    public class FirstTurnOfNewRoundEvent : IEvent
    {
        private readonly InitiativePhase initiativePhase;

        public FirstTurnOfNewRoundEvent(InitiativePhase initiativePhase)
        {
            this.initiativePhase = initiativePhase;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            initiativePhase.StartFirstTurnOfNewRound();
            Complete = true;
        }
    }
}