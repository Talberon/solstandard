using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;

namespace SolStandard.Utility.Events
{
    public class StartCombatEvent : IEvent
    {
        private readonly GameUnit targetUnit;

        public StartCombatEvent(GameUnit targetUnit)
        {
            this.targetUnit = targetUnit;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            BasicAttack.StartCombat(targetUnit);
            GameMapContext.SetPromptWindowText("Confirm End Turn");
            GameContext.GameMapContext.CurrentTurnState = GameMapContext.TurnState.UnitActing;
            Complete = true;
        }
    }
}