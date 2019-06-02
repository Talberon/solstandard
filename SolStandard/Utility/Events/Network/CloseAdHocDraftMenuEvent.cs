using System;
using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class CloseAdHocDraftMenuEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.ClearDraftMenu();
            Complete = true;
        }
    }
}