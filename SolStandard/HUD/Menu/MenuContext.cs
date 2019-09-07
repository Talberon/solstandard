using System.Collections.Generic;
using SolStandard.HUD.Menu.Options;

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

        private MenuContext(Stack<IMenu> menuStack)
        {
            this.menuStack = menuStack;
        }

        public void OpenSubMenu(IMenu submenu)
        {
            menuStack.Push(submenu);
        }

        public void GoToPreviousMenu()
        {
            menuStack.Pop();
        }

        public void SelectCurrentOption()
        {
            CurrentMenu?.SelectOption();

            if (CurrentMenu?.CurrentOption is SubmenuOption) return;

            menuStack.Clear();
        }

        public void ClearMenuStack()
        {
            menuStack.Clear();
        }

        public static MenuContext FromMenuStack(Stack<IMenu> menuStack)
        {
            return new MenuContext(menuStack);
        }
    }
}