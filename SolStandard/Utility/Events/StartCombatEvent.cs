using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills;

namespace SolStandard.Utility.Events
{
    public class StartCombatEvent : IEvent
    {
        private readonly int[] range;
        private readonly Vector2 origin;
        private readonly GameUnit targetUnit;
        private readonly MapContext mapContext;
        private readonly BattleContext battleContext;
        private readonly SpriteAtlas tileSprite;

        public StartCombatEvent(Vector2 origin, int[] range, ref GameUnit targetUnit, ref MapContext mapContext,
            ref BattleContext battleContext, SpriteAtlas tileSprite)
        {
            this.origin = origin;
            this.range = range;
            this.targetUnit = targetUnit;
            this.mapContext = mapContext;
            this.battleContext = battleContext;
            this.tileSprite = tileSprite;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(tileSprite);
            unitTargetingContext.GenerateRealTargetingGrid(origin, range);
            BasicAttack.StartCombat(targetUnit, mapContext, battleContext);
            mapContext.SetPromptWindowText("Confirm End Turn");
            mapContext.CurrentTurnState = MapContext.TurnState.UnitActing;
            Complete = true;
        }
    }
}