using System;
using System.Collections.Generic;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Containers.Contexts
{
    public class DraftContext
    {
        public DraftView DraftView { get; private set; }

        private List<GameUnit> BlueUnits { get; set; }
        private List<GameUnit> RedUnits { get; set; }

        private int maxUnitsPerTeam;
        private int maxDuplicateUnitType;

        private Team currentTurn;

        public DraftContext(DraftView draftView)
        {
            DraftView = draftView;
        }

        public void StartNewDraft(int maxUnitsPerTeam, int maxDuplicateUnitType, Team firstTurn)
        {
            this.maxUnitsPerTeam = maxUnitsPerTeam;
            this.maxDuplicateUnitType = maxDuplicateUnitType;
            currentTurn = firstTurn;

            BlueUnits = new List<GameUnit>();
            RedUnits = new List<GameUnit>();

            DraftView.UpdateTeamUnitsWindow(new List<IRenderable> {new RenderBlank()}, Team.Blue);
            DraftView.UpdateTeamUnitsWindow(new List<IRenderable> {new RenderBlank()}, Team.Red);
            DraftView.UpdateUnitSelectMenu(firstTurn);
        }

        public void MoveCursor(Direction direction)
        {
            switch (direction)
            {
                case Direction.None:
                    break;
                case Direction.Up:
                    DraftView.UnitSelect.MoveMenuCursor(TwoDimensionalMenu.MenuCursorDirection.Up);
                    break;
                case Direction.Right:
                    DraftView.UnitSelect.MoveMenuCursor(TwoDimensionalMenu.MenuCursorDirection.Right);
                    break;
                case Direction.Down:
                    DraftView.UnitSelect.MoveMenuCursor(TwoDimensionalMenu.MenuCursorDirection.Down);
                    break;
                case Direction.Left:
                    DraftView.UnitSelect.MoveMenuCursor(TwoDimensionalMenu.MenuCursorDirection.Left);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }
    }
}