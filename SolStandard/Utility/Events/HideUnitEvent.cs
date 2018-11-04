using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events
{
    public class HideUnitEvent : IEvent
    {
        private readonly UnitEntity unit;

        public HideUnitEvent(UnitEntity unit)
        {
            this.unit = unit;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            unit.Visible = false;
            Complete = true;
        }
    }
}