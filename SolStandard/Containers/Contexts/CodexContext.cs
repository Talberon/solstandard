using System;
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
        public static List<GameUnit> UnitArchetypes => _unitArchetypes ??= GenerateUnitArchetypes();
        private static List<GameUnit> _unitArchetypes;

        public readonly CodexView CodexView;
        private GameContext.GameState previousGameState;

        public CodexContext()
        {
            CodexView = new CodexView(UnitArchetypes);
            ShowUnitDetails(UnitArchetypes.First());
        }

        private static List<GameUnit> GenerateUnitArchetypes()
        {
            var units = new List<GameUnit>();

            foreach (Role role in DraftContext.AvailableRoles)
            {
                units.Add(UnitGenerator.GenerateAdHocUnit(role, Team.Red, true));
            }

            return units;
        }

        public Team CurrentTeam
        {
            get
            {
                return previousGameState switch
                {
                    GameContext.GameState.MainMenu => GameContext.P1Team,
                    GameContext.GameState.ArmyDraft => GameContext.DraftContext.CurrentTurn,
                    GameContext.GameState.Deployment => GameContext.DeploymentContext.CurrentTurn,
                    GameContext.GameState.PauseScreen => GameContext.InitiativeContext.CurrentActiveTeam,
                    GameContext.GameState.InGame => GameContext.InitiativeContext.CurrentActiveTeam,
                    GameContext.GameState.NetworkMenu => GameContext.P1Team,
                    GameContext.GameState.MapSelect => GameContext.P1Team,
                    GameContext.GameState.Results => GameContext.P1Team,
                    GameContext.GameState.ItemPreview => GameContext.InitiativeContext.CurrentActiveTeam,
                    GameContext.GameState.Credits => GameContext.P1Team,
                    _ => throw new ArgumentOutOfRangeException(nameof(previousGameState),
                        $"Should not have arrived here via {previousGameState} state!")
                };
            }
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