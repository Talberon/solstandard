using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
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
        public RoamingRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, new Vector2(GameDriver.CellSize)),
                name: "Roaming Routine",
                description: "Execute Roaming AI routine.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {0}
            )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit slime = GameContext.ActiveUnit;

            List<GameUnit> enemiesInRange = EnemiesWithinThreatRange(slime);

            Queue<IEvent> aiEventQueue = new Queue<IEvent>();
            aiEventQueue.Enqueue(new WaitFramesEvent(60));

            if (enemiesInRange.Count > 0)
            {
                GameUnit targetUnit = enemiesInRange[GameDriver.Random.Next(enemiesInRange.Count)];

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                    "Targetting " + targetUnit.Id + "!", slime.UnitEntity.MapCoordinates, 50
                );

                List<Direction> directions = AStarAlgorithm.DirectionsToDestination(slime.UnitEntity.MapCoordinates,
                    targetUnit.UnitEntity.MapCoordinates);

                foreach (Direction direction in directions)
                {
                    if (direction == Direction.None) continue;

                    aiEventQueue.Enqueue(new CreepMoveEvent(slime, direction));
                    aiEventQueue.Enqueue(new WaitFramesEvent(20));
                }

                aiEventQueue.Enqueue(new CreepMoveEvent(slime, Direction.None));
                aiEventQueue.Enqueue(new StartCombatEvent(targetUnit));
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                    "Roaming...", slime.UnitEntity.MapCoordinates, 50
                );

                //Move randomly up to max movement
                for (int i = 0; i < slime.Stats.Mv; i++)
                {
                    Direction randomDirection =
                        (Direction) GameDriver.Random.Next(1, Enum.GetValues(typeof(Direction)).Length);

                    aiEventQueue.Enqueue(new CreepMoveEvent(slime, randomDirection));
                    aiEventQueue.Enqueue(new WaitFramesEvent(20));
                }

                aiEventQueue.Enqueue(new EndTurnEvent());
            }

            GlobalEventQueue.QueueEvents(aiEventQueue);
        }

        private static List<GameUnit> EnemiesWithinThreatRange(GameUnit creep)
        {
            List<GameUnit> unitsWithinDistance = new List<GameUnit>();

            //Use the threat range to determine if units are in range
            UnitTargetingContext rangeMovingContext =
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Dark));

            rangeMovingContext.GenerateThreatGrid(creep.UnitEntity.MapCoordinates, creep);

            //If enemy is on a tile that has a non-null preview or dynamic tile, then add it to the list

            foreach (GameUnit unit in GameContext.Units)
            {
                if (unit.UnitEntity == null) continue;
                if (unit.Team == GameContext.ActiveUnit.Team) continue;

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