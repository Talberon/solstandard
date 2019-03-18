using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum StatusIcon
    {
        AtkUp,
        AtkRangeUp,
        DefUp,
        HpUp,
        MvUp,
        SpUp,
        MoraleBroken,
        Gold
    }

    public static class StatusIconProvider
    {
        private static Dictionary<StatusIcon, ITexture2D> _skillIconDictionary;

        public static SpriteAtlas GetStatusIcon(StatusIcon icon, Vector2 iconSize)
        {
            return new SpriteAtlas(_skillIconDictionary[icon],
                new Vector2(_skillIconDictionary[icon].Width, _skillIconDictionary[icon].Height), iconSize);
        }

        public static void LoadStatusIcons(List<ITexture2D> skillIconTextures)
        {
            ITexture2D atkUp = skillIconTextures.Find(texture => texture.Name.EndsWith("atkUp"));
            ITexture2D atkRangeUp = skillIconTextures.Find(texture => texture.Name.EndsWith("atkRangeUp"));
            ITexture2D defUp = skillIconTextures.Find(texture => texture.Name.EndsWith("defUp"));
            ITexture2D hpUp = skillIconTextures.Find(texture => texture.Name.EndsWith("hpUp"));
            ITexture2D mvUp = skillIconTextures.Find(texture => texture.Name.EndsWith("mvUp"));
            ITexture2D spUp = skillIconTextures.Find(texture => texture.Name.EndsWith("spUp"));
            ITexture2D moraleBroken = skillIconTextures.Find(texture => texture.Name.EndsWith("moraleBroken"));

            _skillIconDictionary = new Dictionary<StatusIcon, ITexture2D>
            {
                {StatusIcon.AtkUp, atkUp},
                {StatusIcon.AtkRangeUp, atkRangeUp},
                {StatusIcon.DefUp, defUp},
                {StatusIcon.HpUp, hpUp},
                {StatusIcon.MvUp, mvUp},
                {StatusIcon.SpUp, spUp},
                {StatusIcon.MoraleBroken, moraleBroken},
            };
        }
    }
}