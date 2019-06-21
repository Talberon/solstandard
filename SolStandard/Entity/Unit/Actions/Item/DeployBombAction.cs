using System;
using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class DeployBombAction : UnitAction
    {
        private readonly Bomb bombToDeploy;

        public DeployBombAction(Bomb bombToDeploy, int fuseTurns) : base(
            icon: bombToDeploy.RenderSprite.Clone(),
            name: "Set Bomb",
            description: "Place a bomb on an unoccupied tile. Will detonate after [" + fuseTurns + "] rounds." +
                         Environment.NewLine +
                         "Will detonate in a [" + string.Join(",", bombToDeploy.Range) + "] tile range." +
                         Environment.NewLine +
                         "Cannot be picked up once placed!",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.bombToDeploy = bombToDeploy;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanPlaceBombAtTarget(targetSlice))
            {
                bombToDeploy.SnapToCoordinates(targetSlice.MapCoordinates);
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
                    "Must place item on unoccupied space!",
                    50
                );
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CanPlaceBombAtTarget(MapSlice targetSlice)
        {
            return UnitMovingContext.CanEndMoveAtCoordinates(targetSlice.MapCoordinates) &&
                   targetSlice.DynamicEntity != null;
        }
    }
}