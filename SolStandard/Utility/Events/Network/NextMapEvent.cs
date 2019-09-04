using System;
using SolStandard.Containers.Contexts;

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