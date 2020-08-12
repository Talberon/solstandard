using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class Transport : UnitAction
    {
        private readonly string targetLabel;

        public Transport(MapElement portal, string targetLabel) : base(
            icon: portal.RenderSprite.Clone(),
            name: "Transport",
            description: "Moves unit to another space.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement),
            range: null,
            freeAction: true
        )
        {
            this.targetLabel = targetLabel;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            foreach (MapElement mapElement in MapContainer.GameGrid[(int) Layer.Entities])
            {
                var entity = (MapEntity) mapElement;
                if (entity != null && entity.Name == targetLabel)
                {
                    MapContainer
                            .GameGrid[(int) mapLayer][(int) entity.MapCoordinates.X, (int) entity.MapCoordinates.Y] =
                        new MapDistanceTile(TileSprite, entity.MapCoordinates);
                    GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(entity.MapCoordinates);
                }
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanMoveToTargetTile(targetSlice))
            {
                UnitEntity targetEntity = GlobalContext.ActiveUnit.UnitEntity;

                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new HideUnitEvent(targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new BlinkCoordinatesEvent(
                    GlobalContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates
                ));
                eventQueue.Enqueue(new UnhideUnitEvent(targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new AdditionalActionEvent());
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