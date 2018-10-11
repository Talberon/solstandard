using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class MoveEntityToCoordinatesEvent : IEvent
    {
        private readonly MapEntity entity;
        private readonly Vector2 targetCoordinates;

        public MoveEntityToCoordinatesEvent(MapEntity entity, Vector2 targetCoordinates)
        {
            this.entity = entity;
            this.targetCoordinates = targetCoordinates;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            entity.MapCoordinates = targetCoordinates;
            AssetManager.MapUnitMoveSFX.Play();
            Complete = true;
        }
    }
}