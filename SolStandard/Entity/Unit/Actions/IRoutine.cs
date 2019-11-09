using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Actions
{
    public interface IRoutine
    {
        string Name { get; }
        IRenderable Icon { get; }
        IRenderable MapIcon { get; }
        bool CanBeReadied(CreepUnit creep);
        bool CanExecute { get; }
        void ExecuteAction(MapSlice mapSlice);
    }
}