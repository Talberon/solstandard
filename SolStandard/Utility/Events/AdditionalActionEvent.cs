using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;

namespace SolStandard.Utility.Events
{
    public class AdditionalActionEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            if (GlobalContext.ActiveUnit.IsAlive)
            {
                StartExtraAction("Extra action!");
            }
            else
            {
                WorldContext.FinishTurn(true);
            }

            Complete = true;
        }

        public static void StartExtraAction(string message)
        {
            GlobalContext.WorldContext.ResetToActionMenu();
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(message, 50);
            WorldContext.UpdateWindowsEachTurn();
        }
    }
}