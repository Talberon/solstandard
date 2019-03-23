using SolStandard.Utility;

namespace SolStandard.HUD.Menu
{
    public enum MenuCursorDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    
    public interface IMenu : IRenderable
    {
        
        void MoveMenuCursor(MenuCursorDirection direction);
        void SelectOption();
    }
}