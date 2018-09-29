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
        
        public Wait(SpriteAtlas tileSprite) : base(tileSprite)
        {
            Name = "Wait";
            Description = "Take no action and end your turn.";
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