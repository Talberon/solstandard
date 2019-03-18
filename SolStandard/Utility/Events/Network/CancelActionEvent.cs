using System;
using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class CancelActionEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.CancelTargetAction();
            Complete = true;
        }
    }
}