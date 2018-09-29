using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Mage
{
    public class Blink : UnitSkill
    {
        private static readonly int[] Range = {1, 2, 3, 4};

        public Blink() : base(
            name: "Blink",
            description: "Move to an unoccupied space within " + Range.Max() + "spaces.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action)
        )
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