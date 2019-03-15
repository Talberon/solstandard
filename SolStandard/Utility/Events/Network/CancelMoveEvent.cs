using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class CancelMoveEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.GameMapContext.CancelMove();
            Complete = true;
        }
    }
}