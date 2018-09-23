using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Skills
{
    public abstract class UnitSkill
    {
        public string Name { get; private set; }
        protected readonly SpriteAtlas TileSprite;

        protected UnitSkill(string name, SpriteAtlas tileSprite)
        {
            Name = name;
            TileSprite = tileSprite;
        }

        public abstract void GenerateActionGrid(Vector2 origin);
        public abstract void ExecuteAction(GameEntity target, MapContext mapContext, BattleContext battleContext);
    }
}