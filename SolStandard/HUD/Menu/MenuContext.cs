using System.Collections.Generic;

namespace SolStandard.HUD.Menu
{
    public class MenuContext
    {
        private readonly Stack<IMenu> menuStack;
        public IMenu CurrentMenu => menuStack.Count > 0 ? menuStack.Peek() : null;
        public bool IsAtRootMenu => menuStack.Count < 2;

        public MenuContext(IMenu initialMenu)
        {
            menuStack = new Stack<IMenu>();
            menuStack.Push(initialMenu);
        }

        public void OpenSubMenu(IMenu submenu)
        {
            menuStack.Push(submenu);
        }

        public void GoToPreviousMenu()
        {
            menuStack.Pop();
        }

        public void ClearMenuStack()
        {
            menuStack.Clear();
        }

        public void Hide()
        {
            foreach (IMenu menu in menuStack)
            {
                menu.IsVisible = false;
            }
        }

        public void Unhide()
        {
            foreach (IMenu menu in menuStack)
            {
                menu.IsVisible = true;
            }
        }
    }
}