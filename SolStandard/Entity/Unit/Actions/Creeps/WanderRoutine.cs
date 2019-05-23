using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class WanderRoutine : UnitAction, IRoutine
    {
        //TODO Add unique icon
        private const SkillIcon RoutineIcon = SkillIcon.DoubleTime;

        public WanderRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2(GameDriver.CellSize)),
                name: "Wander Routine",
                description: "Wander in random directions.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {0},
                freeAction: false
            )
        {
        }

        public IRenderable MapIcon
        {
            get { return SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3)); }
        }

        public bool CanExecute
        {
            get { return true; }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit roamer = GameContext.ActiveUnit;
            Roam(roamer);
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }


        public static void Roam(GameUnit roamer)
        {
            Queue<IEvent> roamEventQueue = new Queue<IEvent>();
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(
                    roamer.UnitEntity.MapCoordinates,
                    "Wandering...",
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
            GlobalEventQueue.QueueEvents(roamEventQueue);
        }
    }
}