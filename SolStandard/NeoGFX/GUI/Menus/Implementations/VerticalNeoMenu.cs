using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SolStandard.NeoGFX.GUI.Menus.Implementations
{
    public class VerticalNeoMenu : NeoMenu
    {
        protected VerticalNeoMenu(IEnumerable<MenuOption> options, Vector2 centerPosition, int elementSpacing = 0,
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