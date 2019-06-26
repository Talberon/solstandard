using System;
using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Assets
{
    public enum AnimatedIconType
    {
        Death,
        Interact,
        Damage,
        RecoverArmor,
        RecoverHealth
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
                case AnimatedIconType.Damage:
                    return new TriggeredAnimation(
                        AssetManager.DamageTexture,
                        AssetManager.DamageTexture.Height,
                        iconSize * 3,
                        6,
                        Color.White
                    );
                case AnimatedIconType.RecoverArmor:
                    return new TriggeredAnimation(
                        AssetManager.RecoverArmorTexture,
                        AssetManager.RecoverArmorTexture.Height,
                        iconSize * 3,
                        6,
                        Color.White
                    );
                case AnimatedIconType.RecoverHealth:
                    return new TriggeredAnimation(
                        AssetManager.RecoverHealthTexture,
                        AssetManager.RecoverHealthTexture.Height,
                        iconSize * 3,
                        6,
                        Color.White
                    );
                default:
                    throw new ArgumentOutOfRangeException(nameof(iconType), iconType, null);
            }
        }
    }
}