using SolStandard.Entity.Unit.Actions;

namespace SolStandard.Entity.Unit.Statuses.Creep
{
    public class NextRoutineStatus : StatusEffect
    {
        public NextRoutineStatus(
            IRoutine routine
        ) : base(
            routine.Icon,
            routine.Name,
            "Next AI Routine",
            2,
            false,
            false
        )
        {
        }

        public override void ApplyEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            //Do nothing
        }
    }
}