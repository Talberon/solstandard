using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum AnimationType
    {
        SongHymn,
        SongAttack,
        SongLuck,
        SongMove,
        SongRetribution,

        Fire,
        Ice,

        Damage,
        Death,
        Interact,
        RecoverArmor,
        RecoverHealth,
        FallingCoins,
        Ping
    }

    public static class AnimatedSpriteProvider
    {
        private const int StandardFrameDelay = 12;
        private static Dictionary<AnimationType, ITexture2D> _animatedIconDictionary;

        public static AnimatedSpriteSheet GetAnimatedSprite(AnimationType icon, Vector2 renderSize,
            int frameDelay = StandardFrameDelay)
        {
            return GetAnimatedSprite(icon, renderSize, frameDelay, Color.White);
        }

        public static AnimatedSpriteSheet GetAnimatedSprite(AnimationType icon, Vector2 renderSize, int frameDelay,
            Color color)
        {
            return new AnimatedSpriteSheet(
                _animatedIconDictionary[icon],
                _animatedIconDictionary[icon].Height,
                renderSize,
                frameDelay,
                false,
                color
            );
        }

        public static void LoadAnimatedSprites(List<ITexture2D> animationTextures)
        {
            ITexture2D songAura = animationTextures.Find(texture => texture.Name.EndsWith("SongAura"));
            ITexture2D attackAura = animationTextures.Find(texture => texture.Name.EndsWith("AuraAttack"));
            ITexture2D luckAura = animationTextures.Find(texture => texture.Name.EndsWith("AuraLuck"));
            ITexture2D moveAura = animationTextures.Find(texture => texture.Name.EndsWith("AuraMove"));
            ITexture2D retributionAura = animationTextures.Find(texture => texture.Name.EndsWith("AuraRetribution"));

            ITexture2D fire = animationTextures.Find(texture => texture.Name.EndsWith("Fire"));
            ITexture2D ice = animationTextures.Find(texture => texture.Name.EndsWith("IceTrap"));

            ITexture2D damage = animationTextures.Find(texture => texture.Name.EndsWith("Damage"));
            ITexture2D death = animationTextures.Find(texture => texture.Name.EndsWith("Death"));
            ITexture2D interact = animationTextures.Find(texture => texture.Name.EndsWith("Interact"));
            ITexture2D recoverArmor = animationTextures.Find(texture => texture.Name.EndsWith("RecoverArmor"));
            ITexture2D recoverHealth = animationTextures.Find(texture => texture.Name.EndsWith("RecoverHealth"));
            ITexture2D fallingCoins = animationTextures.Find(texture => texture.Name.EndsWith("FallingCoins"));
            ITexture2D ping = animationTextures.Find(texture => texture.Name.EndsWith("Ping"));


            _animatedIconDictionary = new Dictionary<AnimationType, ITexture2D>
            {
                {AnimationType.SongHymn, songAura},
                {AnimationType.SongAttack, attackAura},
                {AnimationType.SongLuck, luckAura},
                {AnimationType.SongMove, moveAura},
                {AnimationType.SongRetribution, retributionAura},

                {AnimationType.Fire, fire},
                {AnimationType.Ice, ice},

                {AnimationType.Damage, damage},
                {AnimationType.Death, death},
                {AnimationType.Interact, interact},
                {AnimationType.RecoverArmor, recoverArmor},
                {AnimationType.RecoverHealth, recoverHealth},
                {AnimationType.FallingCoins, fallingCoins},
                {AnimationType.Ping, ping}
            };
        }
    }
}