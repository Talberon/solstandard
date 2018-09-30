using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills
{
    public class Shove : UnitSkill
    {
        public Shove() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Shove, new Vector2(32)),
            name: "Shove",
            description: "Push a unit away one space if there is an unoccupied space behind them.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsUnitInRange(targetSlice, targetUnit))
            {
                if (CanShove(targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new ShoveEvent(ref targetUnit));
                    eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        public static bool CanShove(GameUnit target)
        {
            Vector2 actorCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = target.UnitEntity.MapCoordinates;
            Vector2 oppositeCoordinates = DetermineShovePosition(actorCoordinates, targetCoordinates);

            if (TargetUnitIsInRange(target) && UnitMovingContext.CanMoveAtCoordinates(oppositeCoordinates))
            {
                return true;
            }

            return false;
        }

        public static Vector2 DetermineShovePosition(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            Vector2 oppositeCoordinates = targetCoordinates;

            if (ActorNorthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move South
                oppositeCoordinates.Y++;
            }

            if (ActorSouthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move North
                oppositeCoordinates.Y--;
            }

            if (ActorEastOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move West
                oppositeCoordinates.X--;
            }

            if (ActorWestOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move East
                oppositeCoordinates.X++;
            }

            return oppositeCoordinates;
        }

        private static bool ActorSouthOfTarget(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            return actorCoordinates.Y > targetCoordinates.Y;
        }

        private static bool ActorWestOfTarget(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            return actorCoordinates.X < targetCoordinates.X;
        }

        private static bool ActorEastOfTarget(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            return actorCoordinates.X > targetCoordinates.X;
        }

        private static bool ActorNorthOfTarget(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            return actorCoordinates.Y < targetCoordinates.Y;
        }


        private static bool TargetUnitIsInRange(GameUnit targetUnit)
        {
            return
                targetUnit != null
                && GameContext.ActiveUnit != targetUnit
                && (MapContainer.GetMapSliceAtCoordinates(targetUnit.UnitEntity.MapCoordinates).DynamicEntity != null);
        }
    }
}