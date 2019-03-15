using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class SelectUnitEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.GameMapContext.SelectUnitAndStartMoving();
            Complete = true;
        }
    }
}