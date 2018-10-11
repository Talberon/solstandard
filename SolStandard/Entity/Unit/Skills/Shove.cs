﻿using System.Collections.Generic;
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
    public class Shove : UnitAction
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
                if (CanShove(targetSlice, targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new ShoveEvent(targetUnit));
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

        public static bool CanShove(MapSlice targetSlice, GameUnit targetUnit)
        {
            Vector2 actorCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = targetUnit.UnitEntity.MapCoordinates;
            Vector2 oppositeCoordinates = DetermineShovePosition(actorCoordinates, targetCoordinates);

            if (TargetIsUnitInRange(targetSlice, targetUnit) &&
                UnitMovingContext.CanMoveAtCoordinates(oppositeCoordinates))
            {
                return true;
            }

            return false;
        }

        public static Vector2 DetermineShovePosition(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            Vector2 oppositeCoordinates = targetCoordinates;

            if (SourceNorthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move South
                oppositeCoordinates.Y++;
            }

            if (SourceSouthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move North
                oppositeCoordinates.Y--;
            }

            if (SourceEastOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move West
                oppositeCoordinates.X--;
            }

            if (SourceWestOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move East
                oppositeCoordinates.X++;
            }

            return oppositeCoordinates;
        }


    }
}