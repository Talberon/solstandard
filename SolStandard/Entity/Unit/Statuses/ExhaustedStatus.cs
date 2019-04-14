namespace SolStandard.Entity.Unit.Statuses
{
    public class ExhaustedStatus : MoveStatDown
    {
        public ExhaustedStatus(int turnDuration, int pointsToReduce) :
            base(
                turnDuration: turnDuration,
                pointsToReduce: pointsToReduce,
                name: "Exhausted!"
            )
        {
        }
    }
}