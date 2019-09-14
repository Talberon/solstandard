using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class StartCombatEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly bool freeAction;
        private readonly UnitStatistics attackerStatsOverride;

        public bool Complete { get; private set; }

        public StartCombatEvent(GameUnit targetUnit, bool freeAction = false,
            UnitStatistics attackerStatsOverride = null)
        {
            this.targetUnit = targetUnit;
            this.freeAction = freeAction;

            this.attackerStatsOverride = attackerStatsOverride ?? GameContext.ActiveUnit.Stats;
        }

        public void Continue()
        {
            GameUnit attackingUnit = GameContext.ActiveUnit;
            GameUnit defendingUnit = targetUnit;

            GameContext.BattleContext.StartNewCombat(attackingUnit, defendingUnit, attackerStatsOverride,
                defendingUnit.Stats, freeAction);

            AssetManager.CombatStartSFX.Play();

            GameMapContext.SetPromptWindowText("Confirm End Turn");
            GameContext.GameMapContext.CurrentTurnState = GameMapContext.TurnState.UnitActing;
            Complete = true;
        }
    }
}