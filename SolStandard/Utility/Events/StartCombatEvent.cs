using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills;

namespace SolStandard.Utility.Events
{
    public class StartCombatEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly GameMapContext gameMapContext;
        private readonly BattleContext battleContext;

        public StartCombatEvent(GameUnit targetUnit, GameMapContext gameMapContext, BattleContext battleContext)
        {
            this.targetUnit = targetUnit;
            this.gameMapContext = gameMapContext;
            this.battleContext = battleContext;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            BasicAttack.StartCombat(targetUnit, gameMapContext, battleContext);
            gameMapContext.SetPromptWindowText("Confirm End Turn");
            gameMapContext.CurrentTurnState = GameMapContext.TurnState.UnitActing;
            Complete = true;
        }
    }
}