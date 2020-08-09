using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class SelectMapEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.MapSelectContext.SelectMap();
            Complete = true;
        }
    }
}