using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class FinishMovingEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.GameMapContext.FinishMoving();
            Complete = true;
        }
    }
}