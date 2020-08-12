using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Draft;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Components.Codex
{
    public class CodexContext 
    {
        public static List<GameUnit> UnitArchetypes => _unitArchetypes ??= GenerateUnitArchetypes();
        private static List<GameUnit> _unitArchetypes;

        public readonly CodexView CodexView;
        private GlobalContext.GameState previousGameState;

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
                    GlobalContext.GameState.MainMenu => GlobalContext.P1Team,
                    GlobalContext.GameState.ArmyDraft => GlobalContext.DraftContext.CurrentTurn,
                    GlobalContext.GameState.Deployment => GlobalContext.DeploymentContext.CurrentTurn,
                    GlobalContext.GameState.PauseScreen => GlobalContext.InitiativePhase.CurrentActiveTeam,
                    GlobalContext.GameState.InGame => GlobalContext.InitiativePhase.CurrentActiveTeam,
                    GlobalContext.GameState.NetworkMenu => GlobalContext.P1Team,
                    GlobalContext.GameState.MapSelect => GlobalContext.P1Team,
                    GlobalContext.GameState.Results => GlobalContext.P1Team,
                    GlobalContext.GameState.ItemPreview => GlobalContext.InitiativePhase.CurrentActiveTeam,
                    GlobalContext.GameState.Credits => GlobalContext.P1Team,
                    _ => throw new ArgumentOutOfRangeException(nameof(previousGameState),
                        $"Should not have arrived here via {previousGameState} state!")
                };
            }
        }

        public void OpenMenu()
        {
            if (GlobalContext.CurrentGameState == GlobalContext.GameState.Codex) return;

            previousGameState = GlobalContext.CurrentGameState;
            GlobalContext.CurrentGameState = GlobalContext.GameState.Codex;
        }

        public void CloseMenu()
        {
            AssetManager.MapUnitCancelSFX.Play();
            GlobalContext.CurrentGameState = previousGameState;
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
        }public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}