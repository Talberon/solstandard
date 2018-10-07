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

namespace SolStandard.Entity.Unit.Skills.Terrain
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

        public override void GenerateActionGrid(Vector2 origin)
        {
            MapContainer.GameGrid[(int) Layer.Dynamic][(int) targetCoordinates.X, (int) targetCoordinates.Y] =
                new MapDistanceTile(TileSprite, targetCoordinates, 0, false);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            if (
                chest == targetSlice.TerrainEntity
                && !chest.IsOpen
                && targetSlice.DynamicEntity != null
                && targetSlice.UnitEntity == null
            )
            {
                if (!chest.IsLocked)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new ToggleOpenEvent(chest));
                    eventQueue.Enqueue(new WaitFramesEvent(5));
                    eventQueue.Enqueue(new IncreaseUnitGoldEvent(chest.Gold));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    AssetManager.LockedSFX.Play();
                }
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }
    }
}