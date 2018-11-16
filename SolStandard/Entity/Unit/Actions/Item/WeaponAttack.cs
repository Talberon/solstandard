using System;
using Microsoft.Xna.Framework;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class WeaponAttack : UnitAction
    {
        private readonly UnitStatistics stats;

        public WeaponAttack(IRenderable icon, string weaponName, UnitStatistics stats) : base(
            icon: icon,
            name: weaponName + " Attack",
            description: "Attack a target with dice based on your weapon's statistics.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
            this.stats = stats;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Range = stats.CurrentAtkRange;
            base.GenerateActionGrid(origin, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            throw new NotImplementedException();
        }
    }
}