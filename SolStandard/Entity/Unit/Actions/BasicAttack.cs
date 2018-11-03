using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class BasicAttack : UnitAction
    {
        public BasicAttack() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, new Vector2(GameDriver.CellSize)),
            name: "Basic Attack",
            description: "Attack a target with dice based on your ATK statistic.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, GameContext.ActiveUnit.Stats.AtkRange, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            Queue<IEvent> eventQueue = new Queue<IEvent>();
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                eventQueue.Enqueue(new StartCombatEvent(targetUnit));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else if (TargetIsABreakableObstacleInRange(targetSlice))
            {
                //deal damage to terrain
                BreakableObstacle targetObstacle = (BreakableObstacle) targetSlice.TerrainEntity;
                targetObstacle.DealDamage(1);
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else if (GameContext.ActiveUnit == targetUnit)
            {
                //Skip the combat state if player selects the same unit
                AssetManager.MapUnitSelectSFX.Play();

                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static void StartCombat(GameUnit target)
        {
            GameUnit attackingUnit = GameContext.ActiveUnit;
            GameUnit defendingUnit = target;
            MapContainer.ClearDynamicAndPreviewGrids();
            GameContext.BattleContext.StartNewCombat(attackingUnit, defendingUnit);
            AssetManager.CombatStartSFX.Play();
        }
    }
}