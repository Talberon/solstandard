using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Steelbreakers.Utility.GUI.HUD.Menus.Implementations
{
    public class VerticalMenu : Menu
    {
        protected VerticalMenu(IEnumerable<MenuOption> options, Vector2 centerPosition, int elementSpacing = 0,
            Point? initialCursorPosition = null, HorizontalAlignment alignment = HorizontalAlignment.Centered) :
            base(ConvertToVerticalList(options), centerPosition, elementSpacing, initialCursorPosition, alignment)
        {
        }

        public static List<List<MenuOption>> ConvertToVerticalList(IEnumerable<MenuOption> options)
        {
            return options.Select(option => new List<MenuOption> {option}).ToList();
        }

        protected override void OnOpen()
        {
            //Do nothing
        }
    }
}