using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;

namespace SolStandard.Containers.Components.Draft
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

        public DraftHUD DraftHUD { get; }

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
            DraftHUD = new DraftHUD();
        }


        public void StartNewSoloDraft(int maxUnits, int maxDuplicates, Team soloPlayerTeam, Scenario.Scenario scenario)
        {
            StartNewDraft(
                soloPlayerTeam == Team.Blue ? maxUnits : 0,
                soloPlayerTeam == Team.Red ? maxUnits : 0,
                maxDuplicates, soloPlayerTeam, scenario
            );
        }

        public void StartNewDraft(int blueTeamUnitMax, int redTeamUnitMax, int maxUnitDuplicates, Team firstTurn,
            Scenario.Scenario scenario)
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

            DraftHUD.UpdateCommanderPortrait(Role.Silhouette, Team.Creep);
            DraftHUD.UpdateTeamUnitsWindow(new List<IRenderable> {RenderBlank.Blank}, Team.Blue);
            DraftHUD.UpdateTeamUnitsWindow(new List<IRenderable> {RenderBlank.Blank}, Team.Red);
            DraftHUD.UpdateUnitSelectMenu(firstTurn, GetRolesEnabled(blueUnitCount, maxDuplicateUnitType));

            DraftHUD.UpdateHelpWindow(
                "SELECT A UNIT" + Environment.NewLine +
                "Max Units: " + Team.Blue + " " + blueMaxUnits + "/" + Team.Red + " " + redMaxUnits +
                Environment.NewLine +
                "Max Dupes: " + maxUnitDuplicates
            );

            DraftHUD.UpdateObjectivesWindow(scenario.ScenarioInfo());
            DraftHUD.UpdateControlsTextWindow();
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
                return currentPhase switch
                {
                    DraftPhase.UnitSelect => DraftHUD.UnitSelect,
                    DraftPhase.CommanderSelect => DraftHUD.CommanderSelect,
                    _ => DraftHUD.UnitSelect
                };
            }
        }

        public void ConfirmSelection()
        {
            switch (currentPhase)
            {
                case DraftPhase.UnitSelect:
                    DraftHUD.UnitSelect.SelectOption();
                    break;
                case DraftPhase.CommanderSelect:
                    DraftHUD.CommanderSelect.SelectOption();
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
            GlobalContext.StartNewDeployment(BlueUnits, RedUnits, CurrentTurn);
        }

        public void SelectCommander(GameUnit unit)
        {
            unit.IsCommander = true;
            unit.UnitEntity.IsCommander = true;
            unit.Actions = UnitGenerator.GetUnitActions(unit.Role, unit.IsCommander);
            DraftHUD.UpdateCommanderPortrait(unit.Role, unit.Team);
            PassTurnCommanderSelect();

            if (!AllCommandersHaveBeenSelected)
            {
                DraftHUD.UpdateCommanderSelect(GetTeamUnits(CurrentTurn), CurrentTurn);
            }
            else
            {
                DraftHUD.HideCommanderSelect();
                currentPhase = DraftPhase.DraftComplete;
                DraftHUD.UpdateHelpWindow("DRAFT COMPLETE." + Environment.NewLine +
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

            DraftHUD.UpdateUnitSelectMenu(
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

            DraftHUD.UpdateTeamUnitsWindow(unitSprites, team);
        }

        private void StartCommanderSelectPhase()
        {
            DraftHUD.HideUnitSelect();
            DraftHUD.UpdateCommanderSelect(GetTeamUnits(CurrentTurn), CurrentTurn);
            DraftHUD.UpdateHelpWindow("SELECT A COMMANDER.");
            currentPhase = DraftPhase.CommanderSelect;
        }

        private Dictionary<Role, int> GetUnitTypeCountForTeam(Team team)
        {
            Dictionary<Role, int> unitLimit = team switch
            {
                Team.Blue => blueUnitCount,
                Team.Red => redUnitCount,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };

            return unitLimit;
        }

        private static Dictionary<Role, bool> GetRolesEnabled(Dictionary<Role, int> roleCount, int limit)
        {
            var enabledRoles = new Dictionary<Role, bool>();

            foreach (KeyValuePair<Role, int> pair in roleCount)
            {
                enabledRoles.Add(pair.Key, (pair.Value < limit));
            }

            return enabledRoles;
        }

        private List<GameUnit> GetTeamUnits(Team team)
        {
            return team switch
            {
                Team.Blue => BlueUnits,
                Team.Red => RedUnits,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };
        }

        private void PassTurnUnitSelect()
        {
            CurrentTurn = CurrentTurn switch
            {
                Team.Blue => (redUnitsSelected >= redMaxUnits ? Team.Blue : Team.Red),
                Team.Red => (blueUnitsSelected >= blueMaxUnits ? Team.Red : Team.Blue),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void PassTurnCommanderSelect()
        {
            CurrentTurn = CurrentTurn switch
            {
                Team.Blue => (redMaxUnits == 0 ? Team.Blue : Team.Red),
                Team.Red => (blueMaxUnits == 0 ? Team.Red : Team.Blue),
                _ => throw new ArgumentOutOfRangeException()
            };
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
                Role.Cleric,
                Role.Bard,
                Role.Mage,

                Role.Cavalier,
                Role.Duelist,
                Role.Pugilist,
                Role.Lancer,
                Role.Rogue,
                Role.Archer,
            };public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}