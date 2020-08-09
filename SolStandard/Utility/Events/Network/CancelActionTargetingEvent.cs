using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class CancelActionTargetingEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.CancelUnitTargeting();
            Complete = true;
        }
    }
}