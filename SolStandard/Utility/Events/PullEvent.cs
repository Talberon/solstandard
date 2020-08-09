using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions.Champion;
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
            Vector2 pullCoordinates = Challenge.DeterminePullPosition(actorCoordinates, targetCoordinates);
            target.UnitEntity.SlideToCoordinates(pullCoordinates);
            AssetManager.CombatBlockSFX.Play();
            Complete = true;
        }
    }
}