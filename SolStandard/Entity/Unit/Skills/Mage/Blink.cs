using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Mage
{
    public class Blink : UnitSkill
    {
        private static readonly int[] Range = {1, 2, 3, 4};

        public Blink(string name, SpriteAtlas tileSprite) : base(name, tileSprite)
        {
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            if (
                UnitMovingContext.CanMoveAtCoordinates(targetSlice.MapCoordinates) &&
                MapContainer.GetMapSliceAtCoordinates(targetSlice.MapCoordinates).DynamicEntity != null
            )
            {
                GameContext.ActiveUnit.UnitEntity.MapCoordinates = targetSlice.MapCoordinates;
                MapContainer.ClearDynamicGrid();
                AssetManager.SkillBlinkSFX.Play();
                SkipCombatPhase(mapContext);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }
    }
}