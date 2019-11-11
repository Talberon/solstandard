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
                StartExtraAction("Extra action!");
            }
            else
            {
                GameMapContext.FinishTurn(true);
            }

            Complete = true;
        }

        public static void StartExtraAction(string message)
        {
            GameContext.GameMapContext.ResetToActionMenu();
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(message, 50);
            GameMapContext.UpdateWindowsEachTurn();
        }
    }
}