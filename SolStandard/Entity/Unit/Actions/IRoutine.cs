using SolStandard.Map.Elements.Cursor;

namespace SolStandard.Entity.Unit.Actions
{
    public interface IRoutine
    {
        void ExecuteAction(MapSlice mapSlice);
    }
}