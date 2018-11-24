namespace SolStandard.Entity.Unit.Actions
{
    public interface IIncrementableAction
    {
        int Value { get; }
        void Increment(int amountToIncrement);
        void Decrement(int amountToDecrement);
    }
}