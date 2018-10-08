using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Skills.Terrain
{
    public class ArtilleryAction : UnitAction
    {
        private readonly int[] range;

        public ArtilleryAction(IRenderable tileIcon, int[] range) : base(
            icon: tileIcon,
            name: "Artillery",
            description: "Attack a target at an extended range based on the range of this weapon." +
                         "\nAttack using your own attack statistic.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
            this.range = range;
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateRealTargetingGrid(origin, range);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            new BasicAttack().ExecuteAction(targetSlice, mapContext, battleContext);
        }
    }
}