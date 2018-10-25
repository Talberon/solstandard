using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
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

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) targetCoordinates.X, (int) targetCoordinates.Y] =
                new MapDistanceTile(TileSprite, targetCoordinates, 0, false);
        }

        public override void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext, BattleContext battleContext)
        {
            if (
                door == targetSlice.TerrainEntity
                && targetSlice.DynamicEntity != null
                && targetSlice.UnitEntity == null
            )
            {
                if (!door.IsLocked)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new ToggleOpenEvent(door));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new EndTurnEvent(ref gameMapContext));
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    MapContainer.AddNewToastAtMapCursor("Door is locked!", 50);
                    AssetManager.LockedSFX.Play();
                }
            }
            else
            {
                MapContainer.AddNewToastAtMapCursor("Not a door!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}