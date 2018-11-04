using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class ArtilleryAction : UnitAction
    {
        public ArtilleryAction(IRenderable tileIcon, int[] range) : base(
            icon: tileIcon,
            name: "Artillery",
            description: "Attack a target at an extended range based on the range of this weapon." +
                         "\nAttack using your own attack statistic.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: range
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            new BasicAttack().ExecuteAction(targetSlice);
        }
    }
}