using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class DeploySelectNextUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.DeploymentContext.SelectNextUnit();
            Complete = true;
        }
    }
}