using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class DraftMenuMoveEvent : NetworkEvent
    {
        private readonly Direction direction;

        public DraftMenuMoveEvent(Direction direction)
        {
            this.direction = direction;
        }
        public override void Continue()
        {
            GlobalContext.DraftContext.MoveCursor(direction);
            Complete = true;
        }
    }
}