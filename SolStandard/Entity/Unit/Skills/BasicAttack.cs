using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills
{
    public class BasicAttack : UnitAction
    {
        public BasicAttack() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, new Vector2(32)),
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
            Queue<IEvent> eventQueue = new Queue<IEvent>();
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                eventQueue.Enqueue(
                    new StartCombatEvent(
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates,
                        GameContext.ActiveUnit.Stats.AtkRange,
                        ref targetUnit,
                        ref mapContext,
                        ref battleContext,
                        TileSprite
                    )
                );
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else if (TargetIsABreakableObstacleInRange(targetSlice))
            {
                //deal damage to terrain
                BreakableObstacle targetObstacle = (BreakableObstacle) targetSlice.TerrainEntity;
                targetObstacle.DealDamage(1);
                eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else if (mapContext.SelectedUnit == targetUnit)
            {
                //Skip the combat state if player selects the same unit
                AssetManager.MapUnitSelectSFX.Play();

                eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
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