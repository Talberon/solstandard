using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Actions
{
    public interface IRoutine
    {
        IRenderable MapIcon { get; }
        bool CanBeReadied(CreepUnit creep);
        bool CanExecute { get; }
        void ExecuteAction(MapSlice mapSlice);
    }
}