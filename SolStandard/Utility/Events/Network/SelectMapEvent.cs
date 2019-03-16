using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class SelectMapEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.MapSelectContext.SelectMap();
            Complete = true;
        }
    }
}