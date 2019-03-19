using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;

namespace SolStandard.Containers.Contexts
{
    public class CodexContext
    {
        public readonly CodexView CodexView;

        public CodexContext()
        {
            List<GameUnit> unitArchetypes = GenerateUnitArchetypes();
            CodexView = new CodexView(unitArchetypes);
            ShowUnitDetails(unitArchetypes.First());
        }

        private static List<GameUnit> GenerateUnitArchetypes()
        {
            List<GameUnit> units = new List<GameUnit>();

            foreach (Role role in DraftContext.AvailableRoles)
            {
                units.Add(UnitGenerator.GenerateDraftUnit(role, Team.Blue, false));
            }

            return units;
        }

        public void MoveMenuCursor(TwoDimensionalMenu.MenuCursorDirection direction)
        {
            CodexView.UnitListMenu.MoveMenuCursor(direction);
        }

        public void SelectUnit()
        {
            CodexView.UnitListMenu.SelectOption();
        }

        public void ShowUnitDetails(GameUnit unit)
        {
            CodexView.ShowUnitDetails(unit);
        }
    }
}