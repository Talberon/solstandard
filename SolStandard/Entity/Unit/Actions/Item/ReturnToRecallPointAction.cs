using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class ReturnToRecallPointAction : UnitAction
    {
        private readonly RecallCharm recallSource;

        public ReturnToRecallPointAction(RecallCharm recallSource) : base(
            icon: recallSource.Icon.Clone(),
            name: "Recall",
            description: "Single Use. " + Environment.NewLine +
                         "Teleport to the deployed recall point. " + Environment.NewLine +
                         "Removes the recall point and the source." + Environment.NewLine +
                         "Recall ID: " + recallSource.RecallId,
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement),
            range: null,
            freeAction: false
        )
        {
            this.recallSource = recallSource;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            List<RecallPoint> recallPoints = MapContainer.GetMapElementsFromLayer(Layer.Entities)
                .Where(tile => tile is RecallPoint).Cast<RecallPoint>().ToList();

            foreach (RecallPoint recall in recallPoints.Where(point => point.BelongsToSource(recallSource.RecallId)))
            {
                MapContainer.GameGrid[(int) mapLayer][(int) recall.MapCoordinates.X, (int) recall.MapCoordinates.Y] =
                    new MapDistanceTile(TileSprite, recall.MapCoordinates);

                GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(recall.MapCoordinates);
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanMoveToTargetTile(targetSlice))
            {
                UnitEntity targetEntity = GlobalContext.ActiveUnit.UnitEntity;
                GlobalContext.ActiveUnit.RemoveItemFromInventory(recallSource);

                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new HideUnitEvent(targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(
                    new BlinkCoordinatesEvent(GlobalContext.ActiveUnit.UnitEntity, targetSlice.MapCoordinates)
                );
                eventQueue.Enqueue(new RemoveEntityFromMapEvent(Layer.Entities, targetSlice.MapCoordinates));
                eventQueue.Enqueue(new UnhideUnitEvent(targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't transport here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}