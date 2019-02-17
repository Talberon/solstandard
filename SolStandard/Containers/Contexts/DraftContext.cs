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
using SolStandard.Utility.Buttons;

namespace SolStandard.Containers.Contexts
{
    public class DraftContext
    {
        private enum DraftPhase
        {
            UnitSelect,
            CommanderSelect,
            DraftComplete
        }

        private DraftPhase currentPhase;

        public DraftView DraftView { get; private set; }

        private List<GameUnit> BlueUnits { get; set; }
        private List<GameUnit> RedUnits { get; set; }

        private int maxUnitsPerTeam;
        private int unitsSelected;
        private int maxDuplicateUnitType;

        private Team currentTurn;

        private Dictionary<Role, int> blueUnitCount;
        private Dictionary<Role, int> redUnitCount;

        public DraftContext(DraftView draftView)
        {
            DraftView = draftView;
        }

        public void StartNewDraft(int maxUnits, int maxUnitDuplicates, Team firstTurn)
        {
            currentPhase = DraftPhase.UnitSelect;
            unitsSelected = 0;
            maxUnitsPerTeam = maxUnits;
            maxDuplicateUnitType = maxUnitDuplicates;
            currentTurn = firstTurn;

            BlueUnits = new List<GameUnit>();
            RedUnits = new List<GameUnit>();
            blueUnitCount = new Dictionary<Role, int>();
            redUnitCount = new Dictionary<Role, int>();

            DraftView.UpdateTeamUnitsWindow(new List<IRenderable> {new RenderBlank()}, Team.Blue);
            DraftView.UpdateTeamUnitsWindow(new List<IRenderable> {new RenderBlank()}, Team.Red);
            DraftView.UpdateUnitSelectMenu(firstTurn, GetRolesEnabled(blueUnitCount, maxDuplicateUnitType));
        }

        public void MoveCursor(Direction direction)
        {
            switch (currentPhase)
            {
                case DraftPhase.UnitSelect:
                    MoveUnitSelectCursor(direction);
                    break;
                case DraftPhase.CommanderSelect:
                    MoveCommanderSelectCursor(direction);
                    break;
                case DraftPhase.DraftComplete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveUnitSelectCursor(Direction direction)
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

        private void MoveCommanderSelectCursor(Direction direction)
        {
            switch (direction)
            {
                case Direction.None:
                    break;
                case Direction.Up:
                    DraftView.CommanderSelect.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
                    break;
                case Direction.Down:
                    DraftView.CommanderSelect.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
                    break;
                case Direction.Right:
                    break;
                case Direction.Left:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        public void ConfirmSelection()
        {
            switch (currentPhase)
            {
                case DraftPhase.UnitSelect:
                    DraftView.UnitSelect.SelectOption();
                    break;
                case DraftPhase.CommanderSelect:
                    DraftView.CommanderSelect.SelectOption();
                    break;
                case DraftPhase.DraftComplete:
                    FinishDraftPhase();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FinishDraftPhase()
        {
            AssetManager.MenuConfirmSFX.Play();
            //TODO Load the selected units and commanders into the Deployment context
            //TODO Change phase to Deployment (add Deployment phase to GameState)
        }

        public void SelectCommander(GameUnit unit)
        {
            unit.IsCommander = true;
            DraftView.UpdateCommanderPortrait(unit.Role, unit.Team);
            PassTurn();

            if (!BothTeamsHaveCommanders)
            {
                DraftView.UpdateCommanderSelect(GetTeamUnits(currentTurn), currentTurn);
            }
            else
            {
                currentPhase = DraftPhase.DraftComplete;
                DraftView.UpdateHelpWindow("DRAFT COMPLETE." + Environment.NewLine +
                                           "PRESS " + Input.Confirm.ToString().ToUpper() + " TO CONTINUE.");
            }
        }

        public void AddUnitToList(Role role, Team team)
        {
            Dictionary<Role, int> unitLimit = GetUnitLimitForTeam(team);

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
            GetTeamUnits(team).Add(generatedUnit);
            List<IRenderable> unitSprites = GetTeamUnits(team).Select(unit => unit.UnitEntity.RenderSprite).ToList();
            DraftView.UpdateTeamUnitsWindow(unitSprites, team);
            PassTurn();

            DraftView.UpdateUnitSelectMenu(
                currentTurn,
                GetRolesEnabled(GetUnitLimitForTeam(currentTurn), maxDuplicateUnitType)
            );

            unitsSelected++;

            if (unitsSelected >= maxUnitsPerTeam * 2)
            {
                DraftView.HideUnitSelect();
                DraftView.UpdateCommanderSelect(GetTeamUnits(currentTurn), currentTurn);
                currentPhase = DraftPhase.CommanderSelect;
            }
        }

        private Dictionary<Role, int> GetUnitLimitForTeam(Team team)
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

            return unitLimit;
        }

        private static Dictionary<Role, bool> GetRolesEnabled(Dictionary<Role, int> roleCount, int limit)
        {
            Dictionary<Role, bool> enabledRoles = new Dictionary<Role, bool>();

            foreach (KeyValuePair<Role, int> pair in roleCount)
            {
                enabledRoles.Add(pair.Key, (pair.Value < limit));
            }

            return enabledRoles;
        }

        private List<GameUnit> GetTeamUnits(Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    return BlueUnits;
                case Team.Red:
                    return RedUnits;
                default:
                    throw new ArgumentOutOfRangeException("team", team, null);
            }
        }

        private void PassTurn()
        {
            switch (currentTurn)
            {
                case Team.Blue:
                    currentTurn = Team.Red;
                    break;
                case Team.Red:
                    currentTurn = Team.Blue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool BothTeamsHaveCommanders
        {
            get
            {
                return BlueUnits.Count(blueUnit => blueUnit.IsCommander) > 0 &&
                       RedUnits.Count(redUnit => redUnit.IsCommander) > 0;
            }
        }

        public static List<Role> AvailableRoles
        {
            get
            {
                return new List<Role>
                {
                    Role.Champion,
                    Role.Lancer,
                    Role.Archer,
                    Role.Mage,
                    Role.Bard
                };
            }
        }
    }
}