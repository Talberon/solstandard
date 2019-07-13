using SolStandard.Entity.General;

namespace SolStandard.Utility.Events
{
    public class PlayEntityAnimationOnceEvent : IEvent
    {
        private readonly TerrainEntity terrainEntity;
        public bool Complete { get; private set; }

        public PlayEntityAnimationOnceEvent(TerrainEntity terrainEntity)
        {
            this.terrainEntity = terrainEntity;
        }

        public void Continue()
        {
            (terrainEntity.RenderSprite as AnimatedSpriteSheet)?.PlayOnce();
            Complete = true;
        }
    }
}