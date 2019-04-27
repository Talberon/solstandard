using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class AdhocDraftEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.OpenDraftMenu();
            Complete = true;
        }
    }
}