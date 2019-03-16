using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class SelectUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.SelectUnitAndStartMoving();
            Complete = true;
        }
    }
}