using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class StartCombatEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly UnitStatistics attackerStatsOverride;

        public bool Complete { get; private set; }

        public StartCombatEvent(GameUnit targetUnit, UnitStatistics attackerStatsOverride = null)
        {
            this.targetUnit = targetUnit;

            this.attackerStatsOverride = attackerStatsOverride ?? GameContext.ActiveUnit.Stats;
        }

        public void Continue()
        {
            GameUnit attackingUnit = GameContext.ActiveUnit;
            GameUnit defendingUnit = targetUnit;
            MapContainer.ClearDynamicAndPreviewGrids();

            GameContext.BattleContext.StartNewCombat(attackingUnit, defendingUnit, attackerStatsOverride,
                defendingUnit.Stats);

            AssetManager.CombatStartSFX.Play();

            GameMapContext.SetPromptWindowText("Confirm End Turn");
            GameContext.GameMapContext.CurrentTurnState = GameMapContext.TurnState.UnitActing;
            Complete = true;
        }
    }
}