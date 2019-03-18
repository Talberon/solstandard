using System;
using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class FinishMovingEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.FinishMoving();
            Complete = true;
        }
    }
}