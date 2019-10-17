using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class MapPingEvent : NetworkEvent
    {
        public override void Continue()
        {
            //TODO Play a soft sound effect

            Vector2 coordinates = (GameContext.MapCursor != null) ? GameContext.MapCursor.MapCoordinates : Vector2.Zero;

            GameContext.GameMapContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.Ping, GameDriver.CellSizeVector),
                coordinates
            );
            Complete = true;
        }
    }
}