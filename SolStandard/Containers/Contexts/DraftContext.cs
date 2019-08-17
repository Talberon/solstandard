using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts.WinConditions;
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

        public DraftView DraftView { get; }

        private List<GameUnit> BlueUnits { get; set; }
        private List<GameUnit> RedUnits { get; set; }

        private int blueMaxUnits;
        private int blueUnitsSelected;

        private int redMaxUnits;
        private int redUnitsSelected;

        private int maxDuplicateUnitType;

        public Team CurrentTurn { get; private set; }

        private Dictionary<Role, int> blueUnitCount;
        private Dictionary<Role, int> redUnitCount;

        public DraftContext()
        {
            DraftView = new DraftView();
        }

        public void StartNewSoloDraft(int maxUnits, int maxDuplicates, Team soloPlayerTeam, Scenario scenario)
        {
            StartNewDraft(
                soloPlayerTeam == Team.Blue ? maxUnits : 0,
                soloPlayerTeam == Team.Red ? maxUnits : 0,
                maxDuplicates, soloPlayerTeam, scenario
            );
        }

        public void StartNewDraft(int blueTeamUnitMax, int redTeamUnitMax, int maxUnitDuplicates, Team firstTurn,
            Scenario scenario)
        {
            NameGenerator.ClearNameHistory();

            currentPhase = DraftPhase.UnitSelect;
            blueUnitsSelected = 0;
            blueMaxUnits = blueTeamUnitMax;
            redUnitsSelected = 0;
            redMaxUnits = redTeamUnitMax;

            maxDuplicateUnitType = maxUnitDuplicates;
            CurrentTurn = firstTurn;

            BlueUnits = new List<GameUnit>();
            RedUnits = new List<GameUnit>();
            blueUnitCount = new Dictionary<Role, int>();
            redUnitCount = new Dictionary<Role, int>();

            DraftView.UpdateCommanderPortrait(Role.Silhouette, Team.Creep);
            DraftView.UpdateTeamUnitsWindow(new List<IRenderable> {new RenderBlank()}, Team.Blue);
            DraftView.UpdateTeamUnitsWindow(new List<IRenderable> {new RenderBlank()}, Team.Red);
            DraftView.UpdateUnitSelectMenu(firstTurn, GetRolesEnabled(blueUnitCount, maxDuplicateUnitType));

            DraftView.UpdateHelpWindow(
                "SELECT A UNIT" + Environment.NewLine +
                "Max Units: " + Team.Blue + " " + blueMaxUnits + "/" + Team.Red + " " + redMaxUnits +
                Environment.NewLine +
                "Max Dupes: " + maxUnitDuplicates
            );

            DraftView.UpdateObjectivesWindow(scenario.ScenarioInfo());
            DraftView.UpdateControlsTextWindow();
        }

        public void MoveCursor(Direction direction)
        {
            switch (currentPhase)
            {
                case DraftPhase.UnitSelect:
                    MoveMenuCursor(direction);
                    break;
                case DraftPhase.CommanderSelect:
                    MoveMenuCursor(direction);
                    break;
                case DraftPhase.DraftComplete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveMenuCursor(Direction direction)
        {
            switch (direction)
            {
                case Direction.None:
                    break;
                case Direction.Up:
                    ActiveMenu.MoveMenuCursor(MenuCursorDirection.Up);
                    break;
                case Direction.Right:
                    ActiveMenu.MoveMenuCursor(MenuCursorDirection.Right);
                    break;
                case Direction.Down:
                    ActiveMenu.MoveMenuCursor(MenuCursorDirection.Down);
                    break;
                case Direction.Left:
                    ActiveMenu.MoveMenuCursor(MenuCursorDirection.Left);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private TwoDimensionalMenu ActiveMenu
        {
            get
            {
                switch (currentPhase)
                {
                    case DraftPhase.UnitSelect:
                        return DraftView.UnitSelect;
                    case DraftPhase.CommanderSelect:
                        return DraftView.CommanderSelect;
                    default:
                        return DraftView.UnitSelect;
                }
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
            GameContext.StartNewDeployment(BlueUnits, RedUnits, CurrentTurn);
        }

        public void SelectCommander(GameUnit unit)
        {
            unit.IsCommander = true;
            unit.UnitEntity.IsCommander = true;
            unit.Actions = UnitGenerator.GetUnitActions(unit.Role, unit.IsCommander);
            DraftView.UpdateCommanderPortrait(unit.Role, unit.Team);
            PassTurnCommanderSelect();

            if (!AllCommandersHaveBeenSelected)
            {
                DraftView.UpdateCommanderSelect(GetTeamUnits(CurrentTurn), CurrentTurn);
            }
            else
            {
                DraftView.HideCommanderSelect();
                currentPhase = DraftPhase.DraftComplete;
                DraftView.UpdateHelpWindow("DRAFT COMPLETE." + Environment.NewLine +
                                           "PRESS " + Input.Confirm.ToString().ToUpper() + " TO CONTINUE.");
            }
        }

        public void AddUnitToList(Role role, Team team)
        {
            Dictionary<Role, int> unitLimit = GetUnitTypeCountForTeam(team);

            if (unitLimit.ContainsKey(role))
            {
                unitLimit[role]++;
            }
            else
            {
                unitLimit.Add(role, 1);
            }

            UpdateSelectedUnitWindow(role, team);
            PassTurnUnitSelect();

            DraftView.UpdateUnitSelectMenu(
                CurrentTurn,
                GetRolesEnabled(GetUnitTypeCountForTeam(CurrentTurn), maxDuplicateUnitType)
            );

            switch (team)
            {
                case Team.Blue:
                    blueUnitsSelected++;
                    break;
                case Team.Red:
                    redUnitsSelected++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(team), team, null);
            }

            if (blueUnitsSelected >= blueMaxUnits && redUnitsSelected >= redMaxUnits)
            {
                StartCommanderSelectPhase();
            }
        }

        private void UpdateSelectedUnitWindow(Role role, Team team)
        {
            GameUnit generatedUnit = UnitGenerator.GenerateAdHocUnit(role, team, false);
            GetTeamUnits(team).Add(generatedUnit);
            const int spriteSize = 128;
            List<IRenderable> unitSprites =
                GetTeamUnits(team)
                    .Select(unit => unit.UnitEntity.UnitSpriteSheet.Resize(new Vector2(spriteSize)))
                    .ToList();

            DraftView.UpdateTeamUnitsWindow(unitSprites, team);
        }

        private void StartCommanderSelectPhase()
        {
            DraftView.HideUnitSelect();
            DraftView.UpdateCommanderSelect(GetTeamUnits(CurrentTurn), CurrentTurn);
            DraftView.UpdateHelpWindow("SELECT A COMMANDER.");
            currentPhase = DraftPhase.CommanderSelect;
        }

        private Dictionary<Role, int> GetUnitTypeCountForTeam(Team team)
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
                    throw new ArgumentOutOfRangeException(nameof(team), team, null);
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
                    throw new ArgumentOutOfRangeException(nameof(team), team, null);
            }
        }

        private void PassTurnUnitSelect()
        {
            switch (CurrentTurn)
            {
                case Team.Blue:
                    CurrentTurn = redUnitsSelected >= redMaxUnits ? Team.Blue : Team.Red;
                    break;
                case Team.Red:
                    CurrentTurn = blueUnitsSelected >= blueMaxUnits ? Team.Red : Team.Blue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PassTurnCommanderSelect()
        {
            switch (CurrentTurn)
            {
                case Team.Blue:
                    CurrentTurn = redMaxUnits == 0 ? Team.Blue : Team.Red;
                    break;
                case Team.Red:
                    CurrentTurn = blueMaxUnits == 0 ? Team.Red : Team.Blue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool AllCommandersHaveBeenSelected
        {
            get
            {
                return (blueMaxUnits == 0 || BlueUnits.Count(blueUnit => blueUnit.IsCommander) > 0) &&
                       (redMaxUnits == 0 || RedUnits.Count(redUnit => redUnit.IsCommander) > 0);
            }
        }

        public static List<Role> AvailableRoles =>
            new List<Role>
            {
                Role.Champion,
                Role.Marauder,
                Role.Paladin,
                Role.Cavalier,
                Role.Cleric,
                Role.Bard,

                Role.Duelist,
                Role.Pugilist,
                Role.Lancer,
                Role.Archer,
                Role.Mage,
                Role.Boar
            };
    }
}