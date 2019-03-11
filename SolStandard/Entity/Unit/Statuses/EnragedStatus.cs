namespace SolStandard.Entity.Unit.Statuses
{
    public class EnragedStatus : AtkStatModifier
    {
        public EnragedStatus(int turnDuration, int atkModifier) :
            base(
                turnDuration: turnDuration,
                atkModifier: atkModifier,
                name: "Enraged! <+" + atkModifier + " " + UnitStatistics.Abbreviation[Stats.Atk] + ">"
            )
        {
        }
    }
}