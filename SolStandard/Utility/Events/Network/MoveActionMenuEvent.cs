using System;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Menu;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class MoveActionMenuEvent: NetworkEvent
    {
        private readonly MenuCursorDirection direction;

        public MoveActionMenuEvent(MenuCursorDirection direction)
        {
            this.direction = direction;
        }
        
        public override void Continue()
        {
            GlobalContext.GameMapContext.MoveActionMenuCursor(direction);
            
            Complete = true;
        }
    }
}