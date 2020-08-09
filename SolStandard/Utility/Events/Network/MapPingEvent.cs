using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class MapPingEvent : NetworkEvent
    {
        public override void Continue()
        {
            Vector2 coordinates = (GameContext.MapCursor != null) ? GameContext.MapCursor.MapCoordinates : Vector2.Zero;

            GameContext.GameMapContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.Ping, GameDriver.CellSizeVector),
                coordinates
            );
            AssetManager.PingSFX.Play();

            Complete = true;
        }
    }
}