using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class SelectActionMenuOptionEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.WorldContext.SelectActionMenuOption();
            Complete = true;
        }
    }
}