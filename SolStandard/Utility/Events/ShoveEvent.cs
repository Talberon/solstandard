using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class ShoveEvent : IEvent
    {
        private readonly GameUnit target;

        public ShoveEvent(GameUnit target)
        {
            this.target = target;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            Vector2 actorCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = target.UnitEntity.MapCoordinates;
            Vector2 oppositeCoordinates = Shove.DetermineShovePosition(actorCoordinates, targetCoordinates);
            target.UnitEntity.MapCoordinates = oppositeCoordinates;
            AssetManager.CombatBlockSFX.Play();
            Complete = true;
        }
    }
}