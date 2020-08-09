using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class CancelMoveEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.GameMapContext.CancelMove();
            Complete = true;
        }
    }
}