using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts
{
    public class CodexContext
    {
        public readonly CodexView CodexView;
        private GameContext.GameState previousGameState;

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

        public void OpenMenu()
        {
            if (GameContext.CurrentGameState == GameContext.GameState.Codex) return;

            previousGameState = GameContext.CurrentGameState;
            GameContext.CurrentGameState = GameContext.GameState.Codex;
        }

        public void CloseMenu()
        {
            AssetManager.MapUnitCancelSFX.Play();
            GameContext.CurrentGameState = previousGameState;
        }

        public void MoveMenuCursor(MenuCursorDirection direction)
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