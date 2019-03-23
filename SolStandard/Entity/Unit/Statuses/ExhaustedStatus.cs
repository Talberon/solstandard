namespace SolStandard.Entity.Unit.Statuses
{
    public class ExhaustedStatus : MoveStatModifier
    {
        public ExhaustedStatus(int turnDuration, int mvModifier) :
            base(
                turnDuration: turnDuration,
                mvModifier: mvModifier,
                name: "Exhausted!",
            )
        {
        }
    }
}