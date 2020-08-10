using System;
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
    public class DeployLadderBridgeAction : UnitAction
    {
        private readonly LadderBridge ladderBridge;

        public DeployLadderBridgeAction(LadderBridge ladderBridge) : base(
            icon: ladderBridge.RenderSprite.Clone(),
            name: "Place Ladder/Bridge",
            description: "Place a ladder/bridge on an unoccupied immovable tile." + Environment.NewLine +
                         "Cannot be picked up once placed!",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.ladderBridge = ladderBridge;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanPlaceLadderBridgeAtTarget(targetSlice))
            {
                ladderBridge.SnapToCoordinates(targetSlice.MapCoordinates);
                GlobalContext.ActiveUnit.RemoveItemFromInventory(ladderBridge);

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new PlaceEntityOnMapEvent(ladderBridge.Duplicate() as LadderBridge, Layer.Entities,
                    AssetManager.CombatBlockSFX));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                    "Must place item on immovable empty space!",
                    50
                );
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CanPlaceLadderBridgeAtTarget(MapSlice targetSlice)
        {
            return !UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates) &&
                   targetSlice.TerrainEntity == null && targetSlice.DynamicEntity != null;
        }
    }
}