using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class NextMapEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.MapSelectContext.MoveCursorToNextMap();
            Complete = true;
        }
    }
}