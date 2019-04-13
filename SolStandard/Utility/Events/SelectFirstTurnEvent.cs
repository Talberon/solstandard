using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events
{
    public class SelectFirstTurnEvent : IEvent
    {
        private readonly InitiativeContext initiativeContext;

        public SelectFirstTurnEvent(InitiativeContext initiativeContext)
        {
            this.initiativeContext = initiativeContext;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            initiativeContext.SelectFirstTeam();
            Complete = true;
        }
    }
}