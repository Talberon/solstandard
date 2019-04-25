using System;
using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class DeployBombAction : UnitAction
    {
        private readonly Bomb bombToDeploy;

        public DeployBombAction(Bomb bombToDeploy) : base(
            icon: bombToDeploy.RenderSprite,
            name: "Set Bomb",
            description: "Place a bomb on an unoccupied tile. Will detonate at the beginning of the next round." +
                         Environment.NewLine +
                         "Will detonate in a [" + bombToDeploy.Range + "] tile radius." + Environment.NewLine +
                         "Cannot be picked up once placed!",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1},
            freeAction: false
        )
        {
            this.bombToDeploy = bombToDeploy;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanPlaceBombAtTarget(targetSlice))
            {
                bombToDeploy.MapCoordinates = targetSlice.MapCoordinates;
                GameContext.ActiveUnit.RemoveItemFromInventory(bombToDeploy);
                
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new PlaceEntityOnMapEvent(bombToDeploy.Duplicate() as Bomb, Layer.Entities,
                    AssetManager.CombatBlockSFX));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    "Must place item on immovable empty space!",
                    50
                );
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CanPlaceBombAtTarget(MapSlice targetSlice)
        {
            return targetSlice.TerrainEntity == null && targetSlice.DynamicEntity != null;
        }
    }
}