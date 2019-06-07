using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions.Terrain;

namespace SolStandard.Utility.Events
{
    public class EscapeObjectiveEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly GameUnit escapingUnit;

        public EscapeObjectiveEvent(GameUnit escapingUnit)
        {
            this.escapingUnit = escapingUnit;
        }

        public void Continue()
        {
            EscapeAction.EscapeWithUnit(escapingUnit);
            Complete = true;
        }
    }
}