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
    public class Transport : UnitAction
    {
        private readonly Vector2 targetCoordinates;

        public Transport(Portal portal, Vector2 targetCoordinates) : base(
            icon: portal.RenderSprite,
            name: "Transport",
            description: "Moves unit to another space.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement),
            range: null
        )
        {
            this.targetCoordinates = targetCoordinates;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) targetCoordinates.X, (int) targetCoordinates.Y] =
                new MapDistanceTile(TileSprite, targetCoordinates, 0, false);
            MapContainer.MapCursor.SnapCursorToCoordinates(targetCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext, BattleContext battleContext)
        {
            if (CanMoveToTargetTile(targetSlice))
            {
                UnitEntity targetEntity = GameContext.ActiveUnit.UnitEntity;

                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new HideUnitEvent(ref targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new BlinkCoordinatesEvent(
                    GameContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates
                ));
                eventQueue.Enqueue(new UnhideUnitEvent(ref targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref gameMapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                MapContainer.AddNewToastAtMapCursor("Can't transport here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}