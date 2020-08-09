using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class DeploySelectPreviousUnitEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.DeploymentContext.SelectPreviousUnit();
            Complete = true;
        }
    }
}