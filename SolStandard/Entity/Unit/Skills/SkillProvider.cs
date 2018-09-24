using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Skills.Champion;
using SolStandard.Entity.Unit.Skills.Mage;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public static class SkillProvider
    {
        public static BasicAttack BasicAttack(int[] range)
        {
            return new BasicAttack("Basic Attack", AttackTile, range);
        }

        public static Shove Shove
        {
            get { return new Shove("Shove", ActionTile); }
        }

        public static Tackle Tackle
        {
            get { return new Tackle("Tackle", AttackTile); }
        }

        public static Blink Blink
        {
            get { return new Blink("Blink", ActionTile); }
        }

        private static SpriteAtlas ActionTile
        {
            get
            {
                return new SpriteAtlas(
                    AssetManager.ActionTiles,
                    new Vector2(GameDriver.CellSize),
                    (int) MapDistanceTile.TileType.Action
                );
            }
        }

        private static SpriteAtlas AttackTile
        {
            get
            {
                return new SpriteAtlas(
                    AssetManager.ActionTiles,
                    new Vector2(GameDriver.CellSize),
                    (int) MapDistanceTile.TileType.Attack
                );
            }
        }
    }
}