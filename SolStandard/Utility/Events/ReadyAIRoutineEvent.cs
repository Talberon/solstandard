using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events
{
    public class ReadyAIRoutineEvent : IEvent
    {
        private readonly CreepUnit creepUnit;
        public bool Complete { get; private set; }

        public ReadyAIRoutineEvent(CreepUnit creepUnit)
        {
            this.creepUnit = creepUnit;
        }

        public void Continue()
        {
            creepUnit.ReadyNextRoutine();
            Complete = true;
        }
    }
}