using System;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Menu;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class MoveActionMenuEvent: NetworkEvent
    {
        private readonly VerticalMenu.MenuCursorDirection direction;

        public MoveActionMenuEvent(VerticalMenu.MenuCursorDirection direction)
        {
            this.direction = direction;
        }
        
        public override void Continue()
        {
            GameContext.GameMapContext.MoveActionMenuCursor(direction);
            
            Complete = true;
        }
    }
}