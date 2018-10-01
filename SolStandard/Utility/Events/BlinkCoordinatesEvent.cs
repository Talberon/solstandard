using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class BlinkCoordinatesEvent : IEvent
    {
        private readonly Vector2 targetCoordinates;

        public BlinkCoordinatesEvent(Vector2 targetCoordinates)
        {
            this.targetCoordinates = targetCoordinates;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.ActiveUnit.UnitEntity.MapCoordinates = targetCoordinates;
            AssetManager.SkillBlinkSFX.Play();
            Complete = true;
        }
    }
}