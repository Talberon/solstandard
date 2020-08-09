using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SolStandard.NeoGFX.GUI.Menus.Implementations
{
    public class GridNeoMenu : NeoMenu
    {
        public GridNeoMenu(List<List<MenuOption>> options, Vector2 centerPosition, int elementSpacing = 0,
            Point? initialCursorPosition = null, HorizontalAlignment alignment = HorizontalAlignment.Centered) :
            base(options, centerPosition, elementSpacing, initialCursorPosition, alignment)
        {
        }

        protected override void OnOpen()
        {
            //Do nothing
        }
    }
}