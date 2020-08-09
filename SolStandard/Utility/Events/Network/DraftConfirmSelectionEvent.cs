using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class DraftConfirmSelectionEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.DraftContext.ConfirmSelection();
            Complete = true;
        }
    }
}