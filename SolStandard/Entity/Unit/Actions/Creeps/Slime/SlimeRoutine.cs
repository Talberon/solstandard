﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps.Slime
{
    public class SlimeRoutine : UnitAction
    {
        public SlimeRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, new Vector2(GameDriver.CellSize)),
                name: "Slime Routine",
                description: "Execute Slime's default AI routine.",
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

            if (enemiesInRange.Count > 0)
            {
                GameUnit targetUnit = enemiesInRange[GameDriver.Random.Next(enemiesInRange.Count)];

                //TODO Create queue of movements next to that unit and then start combat with it.
            }
            else
            {
                //Move randomly up to max movement
                for (int i = 0; i < slime.Stats.Mv; i++)
                {
                    Direction randomDirection =
                        (Direction) GameDriver.Random.Next(Enum.GetValues(typeof(Direction)).Length);

                    //TODO Add movement and short wait to the queue
                    aiEventQueue.Enqueue(new CreepMoveEvent(slime, randomDirection));
                    aiEventQueue.Enqueue(new WaitFramesEvent(20));
                }
            }

            aiEventQueue.Enqueue(new EndTurnEvent());
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