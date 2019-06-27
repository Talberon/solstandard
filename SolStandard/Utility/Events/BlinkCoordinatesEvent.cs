using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class BlinkCoordinatesEvent : IEvent
    {
        private readonly Vector2 targetCoordinates;
        private readonly MapEntity entity;

        public BlinkCoordinatesEvent(MapEntity entity, Vector2 targetCoordinates)
        {
            this.targetCoordinates = targetCoordinates;
            this.entity = entity;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            entity.SnapToCoordinates(targetCoordinates);
            AssetManager.SkillBlinkSFX.Play();
            Complete = true;
        }
    }
}