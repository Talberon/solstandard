using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class SelectActionMenuOptionEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.GameMapContext.SelectActionMenuOption();
            Complete = true;
        }
    }
}