using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class RoamingRoutine : UnitAction, IRoutine
    {
        private readonly bool independent;
        private readonly bool aggressive;

        public RoamingRoutine(bool independent = false, bool aggressive = true)
            : base(
                icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, new Vector2(GameDriver.CellSize)),
                name: "Roaming Routine",
                description: "Execute Roaming AI routine.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {0},
                freeAction: false
            )
        {
            this.independent = independent;
            this.aggressive = aggressive;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit roamer = GameContext.ActiveUnit;

            List<KeyValuePair<GameUnit, Vector2>> targetsInRange = TilesWithinThreatRangeForUnit(roamer, independent);

            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
            if (targetsInRange.Count > 0 && aggressive)
            {
                PathToTargetAndAttack(targetsInRange, roamer);
            }
            else
            {
                Roam(roamer);
            }
        }

        private static void PathToTargetAndAttack(IReadOnlyList<KeyValuePair<GameUnit, Vector2>> targetsInRange, GameUnit roamer)
        {
            KeyValuePair<GameUnit, Vector2> targetUnitCoordinatePair = targetsInRange[GameDriver.Random.Next(targetsInRange.Count)];
            Vector2 roamerMapCoordinates = roamer.UnitEntity.MapCoordinates;

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(roamerMapCoordinates, "Targeting " + targetUnitCoordinatePair.Key.Id + "!", 50)
            );

            List<Direction> directionsToDestination =
                AStarAlgorithm.DirectionsToDestination(roamerMapCoordinates, targetUnitCoordinatePair.Value, false, false);

            Queue<IEvent> pathAndAttackQueue = new Queue<IEvent>();
            foreach (Direction direction in directionsToDestination)
            {
                if (direction == Direction.None) continue;

                pathAndAttackQueue.Enqueue(new UnitMoveEvent(roamer, direction));
                pathAndAttackQueue.Enqueue(new WaitFramesEvent(15));
            }

            pathAndAttackQueue.Enqueue(new UnitMoveEvent(roamer, Direction.None));
            pathAndAttackQueue.Enqueue(new StartCombatEvent(targetUnitCoordinatePair.Key));
            GlobalEventQueue.QueueEvents(pathAndAttackQueue);
        }

        private static void Roam(GameUnit roamer)
        {
            Queue<IEvent> roamEventQueue = new Queue<IEvent>();
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(
                    roamer.UnitEntity.MapCoordinates,
                    "Roaming...",
                    50
                )
            );
            //Move randomly up to max movement
            for (int i = 0; i < roamer.Stats.Mv; i++)
            {
                Direction randomDirection =
                    (Direction) GameDriver.Random.Next(1, Enum.GetValues(typeof(Direction)).Length);

                roamEventQueue.Enqueue(new UnitMoveEvent(roamer, randomDirection));
                roamEventQueue.Enqueue(new WaitFramesEvent(20));
            }

            roamEventQueue.Enqueue(new UnitMoveEvent(roamer, Direction.None));
            roamEventQueue.Enqueue(new WaitFramesEvent(30));
            roamEventQueue.Enqueue(new CreepEndTurnEvent());
            GlobalEventQueue.QueueEvents(roamEventQueue);
        }


        private static List<KeyValuePair<GameUnit, Vector2>> TilesWithinThreatRangeForUnit(GameUnit creep, bool isIndependent)
        {
            //Check movement range
            UnitMovingContext unitMovingContext = new UnitMovingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Dark));
            unitMovingContext.GenerateMoveGrid(creep.UnitEntity.MapCoordinates, creep.MvRange, creep.Team);
            
            //Generate a range ring around each unit on the map using this creep's AtkRange
            //Keep a tally of KV-Pairs tracking the tiles that overlap with the move range and the unit associated with the check

            List<KeyValuePair<GameUnit, Vector2>> attackPositionsInRange = new List<KeyValuePair<GameUnit, Vector2>>();
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Dark));
            
            foreach (GameUnit targetUnit in GameContext.Units)
            {
                if (targetUnit.UnitEntity == null) continue;
                if (targetUnit.UnitEntity == GameContext.ActiveUnit.UnitEntity) continue;
                if (!isIndependent && targetUnit.Team == GameContext.ActiveUnit.Team) continue;
                
                unitTargetingContext.GenerateTargetingGrid(targetUnit.UnitEntity.MapCoordinates, creep.AtkRange, Layer.Preview);
                foreach (MapElement previewTile in MapContainer.GetMapElementsFromLayer(Layer.Preview))
                {
                    MapSlice currentPreviewSlice = MapContainer.GetMapSliceAtCoordinates(previewTile.MapCoordinates);

                    if (TargetAndMoveTilesOverlap(currentPreviewSlice))
                    {
                        attackPositionsInRange.Add(new KeyValuePair<GameUnit, Vector2>(targetUnit, previewTile.MapCoordinates));
                    }
                }
                MapContainer.ClearPreviewGrid();
            }

            return attackPositionsInRange;

            //TODO In the future, consider whether the target unit is a ranged/melee unit to make an optimal move
        }

        private static bool TargetAndMoveTilesOverlap(MapSlice currentPreviewSlice)
        {
            return currentPreviewSlice.DynamicEntity != null && currentPreviewSlice.PreviewEntity != null;
        }

        [Obsolete]
        private static List<GameUnit> UnitsWithinThreatRange(GameUnit creep, bool isIndependent)
        {
            //Use the threat range to determine if units are in range
            UnitTargetingContext rangeMovingContext =
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Dark));

            rangeMovingContext.GenerateThreatGrid(creep.UnitEntity.MapCoordinates, creep, creep.Team);

            //If target is on a tile that has a preview or dynamic tile, then add it to the list

            List<GameUnit> unitsWithinDistance = new List<GameUnit>();
            foreach (GameUnit unit in GameContext.Units)
            {
                if (unit.UnitEntity == null) continue;
                if (unit.UnitEntity == GameContext.ActiveUnit.UnitEntity) continue;
                if (!isIndependent && unit.Team == GameContext.ActiveUnit.Team) continue;

                MapSlice unitSlice = MapContainer.GetMapSliceAtCoordinates(unit.UnitEntity.MapCoordinates);

                if (unitSlice.PreviewEntity != null || unitSlice.DynamicEntity != null)
                {
                    unitsWithinDistance.Add(unit);
                }
            }

            MapContainer.ClearDynamicAndPreviewGrids();

            return unitsWithinDistance;
        }
    }
}