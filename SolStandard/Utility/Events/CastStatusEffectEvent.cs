using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class CastStatusEffectEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly StatusEffect statusEffect;

        public CastStatusEffectEvent(GameUnit targetUnit, StatusEffect statusEffect)
        {
            this.targetUnit = targetUnit;
            this.statusEffect = statusEffect;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            targetUnit.AddStatusEffect(statusEffect);
            GlobalContext.GameMapContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.Interact, GameDriver.CellSizeVector),
                targetUnit.UnitEntity?.MapCoordinates ?? Vector2.Zero
            );
            Complete = true;
        }
    }
}