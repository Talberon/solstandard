using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class CancelMoveEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.CancelMove();
            Complete = true;
        }
    }
}