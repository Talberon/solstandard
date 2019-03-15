using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class ToggleCombatMenuEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.GameMapContext.ToggleCombatMenu();
            Complete = true;
        }
    }
}