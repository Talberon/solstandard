using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum SongTypes
    {
        Song,
        Attack,
        Luck,
        Move,
        Retribution
    }

    public static class SongAtlasProvider
    {
        private const int IconSize = 16;
        private static Dictionary<SongTypes, ITexture2D> _songIconDictionary;

        public static AnimatedSpriteSheet GetSongSheet(SongTypes icon, Vector2 iconSize, Color color)
        {
            return new AnimatedSpriteSheet(
                _songIconDictionary[icon],
                IconSize,
                iconSize,
                120,
                false,
                color
            );
        }

        public static void LoadSongAtlases(List<ITexture2D> skillIconTextures)
        {
            ITexture2D songAura = skillIconTextures.Find(texture => texture.Name.EndsWith("SongAura"));
            ITexture2D attackAura = skillIconTextures.Find(texture => texture.Name.EndsWith("AuraAttack"));
            ITexture2D luckAura = skillIconTextures.Find(texture => texture.Name.EndsWith("AuraLuck"));
            ITexture2D moveAura = skillIconTextures.Find(texture => texture.Name.EndsWith("AuraMove"));
            ITexture2D retributionAura = skillIconTextures.Find(texture => texture.Name.EndsWith("AuraRetribution"));

            _songIconDictionary = new Dictionary<SongTypes, ITexture2D>
            {
                {SongTypes.Song, songAura},
                {SongTypes.Attack, attackAura},
                {SongTypes.Luck, luckAura},
                {SongTypes.Move, moveAura},
                {SongTypes.Retribution, retributionAura},
            };
        }
    }
}