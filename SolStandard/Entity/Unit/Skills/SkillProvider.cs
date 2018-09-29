using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Skills.Archer;
using SolStandard.Entity.Unit.Skills.Champion;
using SolStandard.Entity.Unit.Skills.Mage;
using SolStandard.Entity.Unit.Skills.Monarch;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public static class SkillProvider
    {
        public static Wait Wait
        {
            get { return new Wait(ActionTile); }
        }

        public static BasicAttack BasicAttack
        {
            get { return new BasicAttack(AttackTile); }
        }

        public static Shove Shove
        {
            get { return new Shove(ActionTile); }
        }

        public static Tackle Tackle
        {
            get { return new Tackle(AttackTile); }
        }

        public static Blink Blink
        {
            get { return new Blink(ActionTile); }
        }

        public static Draw Draw
        {
            get { return new Draw(ActionTile); }
        }

        public static DoubleTime DoubleTime
        {
            get { return new DoubleTime(ActionTile); }
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