using System;
using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Assets
{
    public enum AnimatedIconType
    {
        Death,
        Interact
    }

    public static class AnimatedIconProvider
    {
        public static TriggeredAnimation GetAnimatedIcon(AnimatedIconType iconType, Vector2 iconSize)
        {
            switch (iconType)
            {
                case AnimatedIconType.Death:
                    return new TriggeredAnimation(
                        AssetManager.DeathTexture,
                        AssetManager.DeathTexture.Height,
                        iconSize,
                        4,
                        Color.White
                    );
                case AnimatedIconType.Interact:
                    return new TriggeredAnimation(AssetManager.InteractTexture,
                        AssetManager.InteractTexture.Height,
                        iconSize,
                        3,
                        Color.White
                    );
                default:
                    throw new ArgumentOutOfRangeException(nameof(iconType), iconType, null);
            }
        }
    }
}