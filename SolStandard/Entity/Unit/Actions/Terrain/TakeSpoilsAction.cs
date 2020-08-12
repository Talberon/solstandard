using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class TakeSpoilsAction : UnitAction
    {
        private readonly Spoils spoils;

        public TakeSpoilsAction(Spoils spoils) : base(
            icon: spoils.RenderSprite,
            name: "Claim Spoils",
            description: "Take all of the currency and items from the bag of spoils.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: true
        )
        {
            this.spoils = spoils;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) spoils.MapCoordinates.X, (int) spoils.MapCoordinates.Y] =
                new MapDistanceTile(TileSprite, spoils.MapCoordinates);

            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(spoils.MapCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (SelectingItemAtUnitLocation(targetSlice))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                );
                eventQueue.Enqueue(new TakeSpoilsEvent(spoils, GlobalContext.ActiveUnit));
                eventQueue.Enqueue(new WaitFramesEvent(30));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool SelectingItemAtUnitLocation(MapSlice targetSlice)
        {
            return spoils == targetSlice.ItemEntity &&
                   targetSlice.DynamicEntity != null;
        }
    }
}