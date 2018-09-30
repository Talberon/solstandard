using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public class BasicAttack : UnitSkill
    {
        public BasicAttack() : base(
            name: "Basic Attack",
            description: "Attack a target with dice based on your ATK statistic.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateRealTargetingGrid(origin, GameContext.ActiveUnit.Stats.AtkRange);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                StartCombat(targetUnit, mapContext, battleContext);
                EnterCombatPhase(mapContext);
            }
            else if (mapContext.SelectedUnit == targetUnit)
            {
                //Skip the combat state if player selects the same unit
                MapContainer.ClearDynamicAndPreviewGrids();
                mapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                SkipCombatPhase(mapContext);
                AssetManager.MapUnitSelectSFX.Play();
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        public static void StartCombat(GameUnit target, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit attackingUnit = mapContext.SelectedUnit;
            GameUnit defendingUnit = target;
            MapContainer.ClearDynamicAndPreviewGrids();
            battleContext.StartNewCombat(
                attackingUnit,
                MapContainer.GetMapSliceAtCoordinates(attackingUnit.UnitEntity.MapCoordinates),
                defendingUnit,
                MapContainer.GetMapSliceAtCoordinates(defendingUnit.UnitEntity.MapCoordinates)
            );
            AssetManager.CombatStartSFX.Play();
        }
    }
}