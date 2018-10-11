using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills;

namespace SolStandard.Utility.Events
{
    public class StartCombatEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly MapContext mapContext;
        private readonly BattleContext battleContext;

        public StartCombatEvent(GameUnit targetUnit, MapContext mapContext, BattleContext battleContext)
        {
            this.targetUnit = targetUnit;
            this.mapContext = mapContext;
            this.battleContext = battleContext;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            BasicAttack.StartCombat(targetUnit, mapContext, battleContext);
            mapContext.SetPromptWindowText("Confirm End Turn");
            mapContext.CurrentTurnState = MapContext.TurnState.UnitActing;
            Complete = true;
        }
    }
}