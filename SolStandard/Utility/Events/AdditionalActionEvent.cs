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
            }
            else
            {
                GameMapContext.FinishTurn(skipProcs: true);
            }

            Complete = true;
        }
    }
}