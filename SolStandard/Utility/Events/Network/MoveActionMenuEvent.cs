using SolStandard.Containers.Contexts;
using SolStandard.HUD.Menu;

namespace SolStandard.Utility.Events.Network
{
    public class MoveActionMenuEvent: IEvent
    {
        private readonly VerticalMenu.MenuCursorDirection direction;
        public bool Complete { get; private set; }

        public MoveActionMenuEvent(VerticalMenu.MenuCursorDirection direction)
        {
            this.direction = direction;
        }
        
        public void Continue()
        {
            GameContext.GameMapContext.MoveActionMenuCursor(direction);
            
            Complete = true;
        }
    }
}