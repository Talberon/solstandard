using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
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
        private const SkillIcon RoutineIcon = SkillIcon.Wander;

        public WanderRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, GameDriver.CellSizeVector),
                name: "Wander Routine",
                description: "Wander in random directions.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {0},
                freeAction: false
            )
        {
        }

        public IRenderable MapIcon => SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3));


        public bool CanBeReadied(CreepUnit unit)
        {
            return true;
        }

        public bool CanExecute => true;

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit roamer = GlobalContext.ActiveUnit;
            GlobalEventQueue.QueueSingleEvent(new ToastAtCursorEvent("Wandering...", 50));
            Roam(roamer);
            GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(30));
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }


        public static void Roam(GameUnit roamer)
        {
            var roamEventQueue = new Queue<IEvent>();
            //Move randomly up to max movement
            for (int i = 0; i < roamer.Stats.Mv; i++)
            {
                var randomDirection =
                    (Direction) GameDriver.Random.Next(1, Enum.GetValues(typeof(Direction)).Length);

                roamEventQueue.Enqueue(new CreepMoveEvent(roamer, randomDirection));
                roamEventQueue.Enqueue(new SkippableWaitFramesEvent(15));
            }

            roamEventQueue.Enqueue(new CreepMoveEvent(roamer, Direction.None));
            GlobalEventQueue.QueueEvents(roamEventQueue);
        }
    }
}