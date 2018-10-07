﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills.Terrain
{
    public class UseDoorAction : UnitAction
    {
        private readonly Vector2 targetCoordinates;
        private readonly Door door;

        public UseDoorAction(Door door, Vector2 targetCoordinates) : base(
            icon: door.RenderSprite,
            name: "Use Door",
            description: "Opens or closes the door.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null
        )
        {
            this.door = door;
            this.targetCoordinates = targetCoordinates;
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            MapContainer.GameGrid[(int) Layer.Dynamic][(int) targetCoordinates.X, (int) targetCoordinates.Y] =
                new MapDistanceTile(TileSprite, targetCoordinates, 0, false);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            if (
                door == targetSlice.TerrainEntity
                && targetSlice.DynamicEntity != null
                && targetSlice.UnitEntity == null
                && !door.IsLocked
            )
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    //TODO Add door SFX
                    new ToggleOpenEvent(door, AssetManager.MenuConfirmSFX, AssetManager.MenuConfirmSFX)
                );
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }
    }
}