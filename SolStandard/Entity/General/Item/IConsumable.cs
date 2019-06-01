using SolStandard.Entity.Unit;

namespace SolStandard.Entity.General.Item
{
    public interface IConsumable : IItem
    {
        void Consume(GameUnit targetUnit);
    }
}