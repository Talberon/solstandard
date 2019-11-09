using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum BannerType
    {
        White,
        SolTurnStart,
        LunaTurnStart,
        CreepTurnStart,
        RoundStart,
    }
    
    public static class BannerIconProvider
    {
        private const int StandardFrameDelay = 6;
        private static Dictionary<BannerType, ITexture2D> _bannerDictionary;

        public static AnimatedSpriteSheet GetBanner(BannerType icon, Vector2 renderSize,
            int frameDelay = StandardFrameDelay)
        {
            return GetBanner(icon, renderSize, frameDelay, Color.White);
        }

        private static AnimatedSpriteSheet GetBanner(BannerType icon, Vector2 renderSize, int frameDelay,
            Color color)
        {
            return new AnimatedSpriteSheet(
                _bannerDictionary[icon],
                _bannerDictionary[icon].Height,
                renderSize,
                frameDelay,
                false,
                color
            );
        }

        public static void LoadBannerTextures(List<ITexture2D> animationTextures)
        {
            ITexture2D whiteBanner = animationTextures.Find(texture => texture.Name.EndsWith("Banner_White"));
            ITexture2D solBanner = animationTextures.Find(texture => texture.Name.EndsWith("Banner_Sol"));
            ITexture2D lunaBanner = animationTextures.Find(texture => texture.Name.EndsWith("Banner_Luna"));
            ITexture2D creepBanner = animationTextures.Find(texture => texture.Name.EndsWith("Banner_Creep"));
            ITexture2D newRoundBanner = animationTextures.Find(texture => texture.Name.EndsWith("Banner_Round"));

            _bannerDictionary = new Dictionary<BannerType, ITexture2D>
            {
                {BannerType.White, whiteBanner},
                {BannerType.SolTurnStart, solBanner},
                {BannerType.LunaTurnStart, lunaBanner},
                {BannerType.CreepTurnStart, creepBanner},
                {BannerType.RoundStart, newRoundBanner},
            };
        }
    }
}