using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Actions
{
    public interface IRoutine
    {
        IRenderable MapIcon { get; }
        bool CanExecute { get; }
        void ExecuteAction(MapSlice mapSlice);
    }
}