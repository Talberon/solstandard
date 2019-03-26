using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events
{
    public class AdditionalActionEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            if (GameContext.ActiveUnit.IsAlive)
            {
                GameContext.GameMapContext.ResetToActionMenu();
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Extra action!", 50);
                GameMapContext.UpdateWindowsEachTurn();
            }
            else
            {
                GameMapContext.FinishTurn(skipProcs: true);
            }

            Complete = true;
        }
    }
}