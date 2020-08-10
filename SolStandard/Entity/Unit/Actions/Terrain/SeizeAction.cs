using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class SeizeAction : UnitAction
    {
        private readonly Vector2 tileCoordinates;
        private readonly SeizeEntity seizeEntity;

        public SeizeAction(SeizeEntity seizeEntity, Vector2 tileCoordinates) : base(
            icon: seizeEntity.RenderSprite.Clone(),
            name: "Seize",
            description: "Seize the objective and win the battle.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: false
        )
        {
            this.tileCoordinates = tileCoordinates;
            this.seizeEntity = seizeEntity;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) tileCoordinates.X, (int) tileCoordinates.Y] =
                new MapDistanceTile(TileSprite, tileCoordinates);
            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(tileCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (SelectingTileAtUnitLocation(targetSlice))
            {
                if (UnitIsAllowedToSeize)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new SeizeObjectiveEvent(GlobalContext.ActiveTeam));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new EndTurnEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Cannot be seized by this team!",
                        50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Invalid selection!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool SelectingTileAtUnitLocation(MapSlice targetSlice)
        {
            return tileCoordinates == GlobalContext.ActiveUnit.UnitEntity.MapCoordinates &&
                   targetSlice.DynamicEntity != null;
        }

        private bool UnitIsAllowedToSeize
        {
            get
            {
                return GlobalContext.ActiveTeam switch
                {
                    Team.Red => seizeEntity.CapturableByRed,
                    Team.Blue => seizeEntity.CapturableByBlue,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}