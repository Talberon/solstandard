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
        RecoverHealth,
        FallingCoins,
        Ping
    }

    public static class AnimatedIconProvider
    {
        public static TriggeredAnimation GetAnimatedIcon(AnimatedIconType iconType, Vector2 iconSize)
        {
            AnimatedSpriteSheet animation;
            switch (iconType)
            {
                case AnimatedIconType.Death:
                    animation =
                        AnimatedSpriteProvider.GetAnimatedSprite(AnimationType.Death, iconSize, 4);
                    return new TriggeredAnimation(animation);
                case AnimatedIconType.Interact:
                    animation =
                        AnimatedSpriteProvider.GetAnimatedSprite(AnimationType.Interact, iconSize, 3);
                    return new TriggeredAnimation(animation);
                case AnimatedIconType.Damage:
                    animation =
                        AnimatedSpriteProvider.GetAnimatedSprite(AnimationType.Damage, iconSize * 3, 6);
                    return new TriggeredAnimation(animation);
                case AnimatedIconType.RecoverArmor:
                    animation =
                        AnimatedSpriteProvider.GetAnimatedSprite(AnimationType.RecoverArmor, iconSize * 3, 6);
                    return new TriggeredAnimation(animation);
                case AnimatedIconType.RecoverHealth:
                    animation =
                        AnimatedSpriteProvider.GetAnimatedSprite(AnimationType.RecoverHealth, iconSize * 3, 6);
                    return new TriggeredAnimation(animation);
                case AnimatedIconType.FallingCoins:
                    animation =
                        AnimatedSpriteProvider.GetAnimatedSprite(AnimationType.FallingCoins, iconSize * 3, 3);
                    return new TriggeredAnimation(animation);
                case AnimatedIconType.Ping:
                    animation =
                        AnimatedSpriteProvider.GetAnimatedSprite(AnimationType.Ping, iconSize * 3, 3);
                    return new TriggeredAnimation(animation);
                default:
                    throw new ArgumentOutOfRangeException(nameof(iconType), iconType, null);
            }
        }
    }
}