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
            Vector2 coordinates = (GlobalContext.MapCursor != null) ? GlobalContext.MapCursor.MapCoordinates : Vector2.Zero;

            GlobalContext.WorldContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.Ping, GameDriver.CellSizeVector),
                coordinates
            );
            AssetManager.PingSFX.Play();

            Complete = true;
        }
    }
}