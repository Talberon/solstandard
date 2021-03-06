using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class PlayAnimationAtCoordinatesEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly TriggeredAnimation animation;
        private readonly Vector2 mapCoordinates;

        public PlayAnimationAtCoordinatesEvent(AnimatedIconType iconType, Vector2 mapCoordinates)
        {
            this.mapCoordinates = mapCoordinates;
            animation = AnimatedIconProvider.GetAnimatedIcon(iconType, GameDriver.CellSizeVector);
        }

        public void Continue()
        {
            GlobalContext.WorldContext.PlayAnimationAtCoordinates(animation, mapCoordinates);
            Complete = true;
        }
    }
}