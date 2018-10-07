using SolStandard.Entity.Unit.Skills;
using SolStandard.Utility;

namespace SolStandard.Entity
{
    public interface IItem
    {
        IRenderable Icon { get; }
        string Name { get; }
        UnitAction UseAction();
        UnitAction DropAction();
    }
}