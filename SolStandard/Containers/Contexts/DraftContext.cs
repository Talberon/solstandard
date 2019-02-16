using System;
using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

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

        private Dictionary<Role, int> blueUnitCount;
        private Dictionary<Role, int> redUnitCount;

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
            blueUnitCount = new Dictionary<Role, int>();
            redUnitCount = new Dictionary<Role, int>();

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

        public void ConfirmSelection()
        {
            DraftView.UnitSelect.SelectOption();
        }

        public void AddUnitToList(Role role, Team team)
        {
            Dictionary<Role, int> unitLimit;

            switch (team)
            {
                case Team.Blue:
                    unitLimit = blueUnitCount;
                    break;
                case Team.Red:
                    unitLimit = redUnitCount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("team", team, null);
            }

            if (unitLimit.ContainsKey(role))
            {
                if (unitLimit[role] >= maxDuplicateUnitType)
                {
                    AssetManager.WarningSFX.Play();
                    //TODO Display a warning that the unit has been selected too many times for that team
                    return;
                }
                else
                {
                    unitLimit[role]++;
                }
            }
            else
            {
                unitLimit.Add(role, 1);
            }


            GameUnit generatedUnit = UnitGenerator.GenerateDraftUnit(role, team, false);

            switch (team)
            {
                case Team.Blue:
                    BlueUnits.Add(generatedUnit);
                    List<IRenderable> blueUnitSprites = BlueUnits.Select(unit => unit.UnitEntity.RenderSprite).ToList();
                    DraftView.UpdateTeamUnitsWindow(blueUnitSprites, team);
                    break;
                case Team.Red:
                    RedUnits.Add(generatedUnit);
                    List<IRenderable> redUnitSprites = RedUnits.Select(unit => unit.UnitEntity.RenderSprite).ToList();
                    DraftView.UpdateTeamUnitsWindow(redUnitSprites, team);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("team", team, null);
            }
        }
    }
}