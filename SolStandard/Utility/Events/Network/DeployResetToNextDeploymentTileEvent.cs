using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class DeployResetToNextDeploymentTileEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.DeploymentContext.MoveToNextDeploymentTile();
            Complete = true;
        }
    }
}