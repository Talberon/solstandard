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
    public class OpenChestAction : UnitAction
    {
        private readonly Vector2 targetCoordinates;
        private readonly Chest chest;

        public OpenChestAction(Chest chest, Vector2 targetCoordinates) : base(
            icon: chest.RenderSprite,
            name: "Open Chest",
            description: "Opens a chest if able.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null
        )
        {
            this.chest = chest;
            this.targetCoordinates = targetCoordinates;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) targetCoordinates.X, (int) targetCoordinates.Y] =
                new MapDistanceTile(TileSprite, targetCoordinates);

            GameContext.GameMapContext.MapContainer.MapCursor.SnapCursorToCoordinates(targetCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsUnopenedChest(targetSlice))
            {
                if (!chest.IsLocked)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new ToggleOpenEvent(chest));
                    eventQueue.Enqueue(new WaitFramesEvent(5));

                    if (chest.Items.Count > 0)
                    {
                        foreach (IItem item in chest.Items)
                        {
                            eventQueue.Enqueue(new AddItemToUnitInventoryEvent(GameContext.ActiveUnit, item));
                            eventQueue.Enqueue(new WaitFramesEvent(30));
                        }
                    }

                    if (chest.Gold > 0)
                    {
                        eventQueue.Enqueue(new IncreaseUnitGoldEvent(chest.Gold));
                        eventQueue.Enqueue(new WaitFramesEvent(20));
                    }


                    eventQueue.Enqueue(new EndTurnEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Chest is locked!", 50);
                    AssetManager.LockedSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Cannot open chest here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool TargetIsUnopenedChest(MapSlice targetSlice)
        {
            return chest == targetSlice.TerrainEntity
                   && !chest.IsOpen
                   && targetSlice.DynamicEntity != null
                   && targetSlice.UnitEntity == null;
        }
    }
}