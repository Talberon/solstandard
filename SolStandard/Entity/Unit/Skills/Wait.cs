using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public class Wait : UnitSkill
    {
        private static readonly int[] Range = {0};
        
        public Wait(string name, SpriteAtlas tileSprite) : base(name, tileSprite)
        {
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            MapContainer.ClearDynamicGrid();
            SkipCombatPhase(mapContext);
            AssetManager.MapUnitSelectSFX.Play();
        }
    }
}