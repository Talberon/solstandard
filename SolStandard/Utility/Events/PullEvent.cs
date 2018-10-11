using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills.Archer;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class PullEvent : IEvent
    {
        private readonly GameUnit target;

        public PullEvent(GameUnit target)
        {
            this.target = target;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            Vector2 actorCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = target.UnitEntity.MapCoordinates;
            Vector2 pullCoordinates = Harpoon.DeterminePullPosition(actorCoordinates, targetCoordinates);
            target.UnitEntity.MapCoordinates = pullCoordinates;
            AssetManager.CombatBlockSFX.Play();
            Complete = true;
        }
    }
}