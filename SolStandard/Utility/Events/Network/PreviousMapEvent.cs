using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class PreviousMapEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.MapSelectContext.MoveCursorToPreviousMap();
            Complete = true;
        }
    }
}