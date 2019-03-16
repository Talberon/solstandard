using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class SelectActionMenuOptionEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.SelectActionMenuOption();
            Complete = true;
        }
    }
}