using System;
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
    public class SeizeAction : UnitAction
    {
        private readonly Vector2 tileCoordinates;
        private readonly SeizeEntity seizeEntity;

        public SeizeAction(SeizeEntity seizeEntity, Vector2 tileCoordinates) : base(
            icon: seizeEntity.RenderSprite,
            name: "Seize",
            description: "Seize the objective and win the battle.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null
        )
        {
            this.tileCoordinates = tileCoordinates;
            this.seizeEntity = seizeEntity;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) tileCoordinates.X, (int) tileCoordinates.Y] =
                new MapDistanceTile(TileSprite, tileCoordinates, 0, false);
            MapContainer.MapCursor.SnapCursorToCoordinates(tileCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext,
            BattleContext battleContext)
        {
            if (SelectingTileAtUnitLocation(targetSlice))
            {
                if (UnitIsAllowedToSeize)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new SeizeObjectiveEvent(GameContext.ActiveUnit.Team));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new EndTurnEvent(ref gameMapContext));
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    MapContainer.AddNewToastAtMapCursor("Cannot be seized by this team!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                MapContainer.AddNewToastAtMapCursor("Invalid selection!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool SelectingTileAtUnitLocation(MapSlice targetSlice)
        {
            return tileCoordinates == GameContext.ActiveUnit.UnitEntity.MapCoordinates &&
                   targetSlice.DynamicEntity != null;
        }

        private bool UnitIsAllowedToSeize
        {
            get
            {
                switch (GameContext.ActiveUnit.Team)
                {
                    case Team.Red:
                        return seizeEntity.CapturableByRed;
                    case Team.Blue:
                        return seizeEntity.CapturableByBlue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}