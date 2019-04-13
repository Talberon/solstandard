using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events
{
    public class UpdateTurnOrderEvent : IEvent
    {
        private readonly InitiativeContext initiativeContext;

        public UpdateTurnOrderEvent(InitiativeContext initiativeContext)
        {
            this.initiativeContext = initiativeContext;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            initiativeContext.UpdateTurnOrder();
            Complete = true;
        }
    }
}