using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Skills
{
    public abstract class UnitSkill
    {
        public string Name { get; private set; }
        protected readonly SpriteAtlas TileSprite;
        protected readonly int[] Range;

        protected UnitSkill(string name, SpriteAtlas tileSprite, int[] range)
        {
            Name = name;
            TileSprite = tileSprite;
            Range = range;
        }

        public abstract void GenerateActionGrid(Vector2 origin);
        public abstract void ExecuteAction(GameEntity target, MapContext mapContext, BattleContext battleContext);
    }
}