using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ExecuteActionEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.GameMapContext.ExecuteAction();
            Complete = true;
        }
    }
}