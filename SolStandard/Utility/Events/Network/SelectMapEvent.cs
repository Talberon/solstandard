using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class SelectMapEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            GameContext.MapSelectContext.SelectMap();
            Complete = true;
        }
    }
}