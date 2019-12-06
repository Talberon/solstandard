using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum MiscIcon
    {
        First,
        Second,
        Independent,
        Clock,
        Crown,
        Context,
        Durability,
        Gold,
        Hand,
        Spoils,
        SkillBook
    }

    public static class MiscIconProvider
    {
        private static Dictionary<MiscIcon, ITexture2D> _miscIconDictionary;

        public static SpriteAtlas GetMiscIcon(MiscIcon icon, Vector2 iconSize)
        {
            return new SpriteAtlas(
                _miscIconDictionary[icon],
                new Vector2(_miscIconDictionary[icon].Width, _miscIconDictionary[icon].Height),
                iconSize
            );
        }

        public static void LoadMiscIcons(List<ITexture2D> miscIconTextures)
        {
            ITexture2D first = miscIconTextures.Find(texture => texture.Name.EndsWith("1st"));
            ITexture2D second = miscIconTextures.Find(texture => texture.Name.EndsWith("2nd"));
            ITexture2D independent = miscIconTextures.Find(texture => texture.Name.EndsWith("Independent"));
            ITexture2D clock = miscIconTextures.Find(texture => texture.Name.EndsWith("clock"));
            ITexture2D crown = miscIconTextures.Find(texture => texture.Name.EndsWith("CommanderCrown"));
            ITexture2D context = miscIconTextures.Find(texture => texture.Name.EndsWith("Context"));
            ITexture2D durability = miscIconTextures.Find(texture => texture.Name.EndsWith("durability"));
            ITexture2D gold = miscIconTextures.Find(texture => texture.Name.EndsWith("gold"));
            ITexture2D hand = miscIconTextures.Find(texture => texture.Name.EndsWith("hand"));
            ITexture2D spoils = miscIconTextures.Find(texture => texture.Name.EndsWith("spoils"));
            ITexture2D skillBook = miscIconTextures.Find(texture => texture.Name.EndsWith("SkillBook"));

            _miscIconDictionary = new Dictionary<MiscIcon, ITexture2D>
            {
                {MiscIcon.First, first},
                {MiscIcon.Second, second},
                {MiscIcon.Independent, independent},
                {MiscIcon.Clock, clock},
                {MiscIcon.Crown, crown},
                {MiscIcon.Context, context},
                {MiscIcon.Durability, durability},
                {MiscIcon.Gold, gold},
                {MiscIcon.Hand, hand},
                {MiscIcon.Spoils, spoils},
                {MiscIcon.SkillBook, skillBook},
            };
        }
    }
}