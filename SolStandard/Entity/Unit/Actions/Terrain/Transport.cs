﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
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
                MapEntity entity = (MapEntity) mapElement;
                if (entity != null && entity.Name == targetLabel)
                {
                    MapContainer
                            .GameGrid[(int) mapLayer][(int) entity.MapCoordinates.X, (int) entity.MapCoordinates.Y] =
                        new MapDistanceTile(TileSprite, entity.MapCoordinates);
                    GameContext.GameMapContext.MapContainer.MapCursor.SnapCursorToCoordinates(entity.MapCoordinates);
                }
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanMoveToTargetTile(targetSlice))
            {
                UnitEntity targetEntity = GameContext.ActiveUnit.UnitEntity;

                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new HideUnitEvent(targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new BlinkCoordinatesEvent(
                    GameContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates
                ));
                eventQueue.Enqueue(new UnhideUnitEvent(targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't transport here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}