using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class CancelActionTargetingEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.WorldContext.CancelUnitTargeting();
            Complete = true;
        }
    }
}