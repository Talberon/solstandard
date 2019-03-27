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

            List<GameUnit> targetsInRange = UnitsWithinThreatRange(roamer);

            Queue<IEvent> aiEventQueue = new Queue<IEvent>();
            aiEventQueue.Enqueue(new WaitFramesEvent(30));

            if (targetsInRange.Count > 0 && aggressive)
            {
                GameUnit targetUnit = targetsInRange[GameDriver.Random.Next(targetsInRange.Count)];

                GlobalEventQueue.QueueSingleEvent(
                    new ToastAtCoordinatesEvent(
                        roamer.UnitEntity.MapCoordinates,
                        "Targetting " + targetUnit.Id + "!",
                        50
                    )
                );

                List<Direction> directions = AStarAlgorithm.DirectionsToDestination(roamer.UnitEntity.MapCoordinates,
                    targetUnit.UnitEntity.MapCoordinates);

                foreach (Direction direction in directions)
                {
                    if (direction == Direction.None) continue;

                    aiEventQueue.Enqueue(new UnitMoveEvent(roamer, direction));
                    aiEventQueue.Enqueue(new WaitFramesEvent(15));
                }

                aiEventQueue.Enqueue(new UnitMoveEvent(roamer, Direction.None));
                aiEventQueue.Enqueue(new StartCombatEvent(targetUnit));
            }
            else
            {
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

                    aiEventQueue.Enqueue(new UnitMoveEvent(roamer, randomDirection));
                    aiEventQueue.Enqueue(new WaitFramesEvent(20));
                }

                aiEventQueue.Enqueue(new UnitMoveEvent(roamer, Direction.None));
                aiEventQueue.Enqueue(new WaitFramesEvent(30));
                aiEventQueue.Enqueue(new CreepEndTurnEvent());
            }

            GlobalEventQueue.QueueEvents(aiEventQueue);
        }

        private List<GameUnit> UnitsWithinThreatRange(GameUnit creep)
        {
            List<GameUnit> unitsWithinDistance = new List<GameUnit>();

            //Use the threat range to determine if units are in range
            UnitTargetingContext rangeMovingContext =
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Dark));

            rangeMovingContext.GenerateThreatGrid(creep.UnitEntity.MapCoordinates, creep);

            //If target is on a tile that has a non-null preview or dynamic tile, then add it to the list

            foreach (GameUnit unit in GameContext.Units)
            {
                if (unit.UnitEntity == null) continue;
                if (unit.UnitEntity == GameContext.ActiveUnit.UnitEntity) continue;
                if (!independent && unit.Team == GameContext.ActiveUnit.Team) continue;

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