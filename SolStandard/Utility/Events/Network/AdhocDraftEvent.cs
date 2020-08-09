using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class AdhocDraftEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.GameMapContext.OpenDraftMenu();
            Complete = true;
        }
    }
}