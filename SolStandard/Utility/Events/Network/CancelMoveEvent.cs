using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class CancelMoveEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.CancelMove();
            Complete = true;
        }
    }
}