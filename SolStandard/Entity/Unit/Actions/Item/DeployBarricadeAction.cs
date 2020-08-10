using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class DeployBarricadeAction : UnitAction
    {
        private readonly Barricade barricade;

        public DeployBarricadeAction(Barricade barricade) : base(
            icon: barricade.RenderSprite.Clone(),
            name: "Place Obstacle",
            description: "Place a breakable obstacle on an unoccupied tile.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.barricade = barricade;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanPlaceObstacleAtTarget(targetSlice))
            {
                barricade.SnapToCoordinates(targetSlice.MapCoordinates);
                GlobalContext.ActiveUnit.RemoveItemFromInventory(barricade);

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new PlaceEntityOnMapEvent(barricade, Layer.Entities, AssetManager.CombatBlockSFX));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Cannot place obstacle here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CanPlaceObstacleAtTarget(MapSlice targetSlice)
        {
            return UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates) &&
                   targetSlice.TerrainEntity == null && targetSlice.DynamicEntity != null;
        }
    }
}