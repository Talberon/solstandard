using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events
{
    public class UnhideUnitEvent : IEvent
    {
        private readonly UnitEntity unit;

        public UnhideUnitEvent(ref UnitEntity unit)
        {
            this.unit = unit;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            unit.Visible = true;
            Complete = true;
        }
    }
}