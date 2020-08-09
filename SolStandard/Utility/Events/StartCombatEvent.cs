using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
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

            this.attackerStatsOverride = attackerStatsOverride ?? GlobalContext.ActiveUnit.Stats;
        }

        public void Continue()
        {
            GameUnit attackingUnit = GlobalContext.ActiveUnit;
            GameUnit defendingUnit = targetUnit;

            GlobalContext.BattleContext.StartNewCombat(attackingUnit, defendingUnit, attackerStatsOverride,
                defendingUnit.Stats, freeAction);

            AssetManager.CombatStartSFX.Play();

            
            GlobalContext.GameMapContext.CurrentTurnState = GameMapContext.TurnState.UnitActing;
            Complete = true;
        }
    }
}