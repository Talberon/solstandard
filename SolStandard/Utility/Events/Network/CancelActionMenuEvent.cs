using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class CancelActionMenuEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.CancelActionMenu();
            Complete = true;
        }
    }
}