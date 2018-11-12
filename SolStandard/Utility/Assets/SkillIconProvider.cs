using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum SkillIcon
    {
        BasicAttack,
        Blink,
        Cover,
        DoubleTime,
        Draw,
        Inspire,
        Shove,
        Tackle,
        Wait,
        Harpoon,
        Ignite,
        Bulwark,
        Atrophy,
        Trap
    }

    public static class SkillIconProvider
    {
        private static Dictionary<SkillIcon, ITexture2D> _skillIconDictionary;

        public static SpriteAtlas GetSkillIcon(SkillIcon icon, Vector2 iconSize)
        {
            return new SpriteAtlas(_skillIconDictionary[icon],
                new Vector2(_skillIconDictionary[icon].Width, _skillIconDictionary[icon].Height), iconSize);
        }

        public static void LoadSkillIcons(List<ITexture2D> skillIconTextures)
        {
            ITexture2D basicAttack = skillIconTextures.Find(texture => texture.Name.EndsWith("BasicAttack"));
            ITexture2D blink = skillIconTextures.Find(texture => texture.Name.EndsWith("Blink"));
            ITexture2D cover = skillIconTextures.Find(texture => texture.Name.EndsWith("Cover"));
            ITexture2D doubleTime = skillIconTextures.Find(texture => texture.Name.EndsWith("DoubleTime"));
            ITexture2D draw = skillIconTextures.Find(texture => texture.Name.EndsWith("Draw"));
            ITexture2D inspire = skillIconTextures.Find(texture => texture.Name.EndsWith("Inspire"));
            ITexture2D shove = skillIconTextures.Find(texture => texture.Name.EndsWith("Shove"));
            ITexture2D tackle = skillIconTextures.Find(texture => texture.Name.EndsWith("Tackle"));
            ITexture2D wait = skillIconTextures.Find(texture => texture.Name.EndsWith("Wait"));
            ITexture2D harpoon = skillIconTextures.Find(texture => texture.Name.EndsWith("Harpoon"));
            ITexture2D ignite = skillIconTextures.Find(texture => texture.Name.EndsWith("Ignite"));
            ITexture2D bulwark = skillIconTextures.Find(texture => texture.Name.EndsWith("Bulwark"));
            ITexture2D atrophy = skillIconTextures.Find(texture => texture.Name.EndsWith("Atrophy"));
            ITexture2D trap = skillIconTextures.Find(texture => texture.Name.EndsWith("Trap"));

            _skillIconDictionary = new Dictionary<SkillIcon, ITexture2D>
            {
                {SkillIcon.BasicAttack, basicAttack},
                {SkillIcon.Blink, blink},
                {SkillIcon.Cover, cover},
                {SkillIcon.DoubleTime, doubleTime},
                {SkillIcon.Draw, draw},
                {SkillIcon.Inspire, inspire},
                {SkillIcon.Shove, shove},
                {SkillIcon.Tackle, tackle},
                {SkillIcon.Wait, wait},
                {SkillIcon.Harpoon, harpoon},
                {SkillIcon.Ignite, ignite},
                {SkillIcon.Bulwark, bulwark},
                {SkillIcon.Atrophy, atrophy},
                {SkillIcon.Trap, trap}
            };
        }
    }
}