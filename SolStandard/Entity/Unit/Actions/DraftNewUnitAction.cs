using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;

namespace SolStandard.Entity.Unit.Actions
{
    public class DraftNewUnitAction : UnitAction
    {
        private readonly IItem spawnItem;

        public DraftNewUnitAction(IItem spawnItem) : base(
            icon: spawnItem.Icon.Clone(),
            name: "Draft Unit",
            description: "Draft a new ally to aid you in battle!",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.spawnItem = spawnItem;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsUnoccupiedTileInRange(targetSlice))
            {
                GlobalContext.ActiveUnit.RemoveItemFromInventory(spawnItem);
                GlobalEventQueue.QueueSingleEvent(new AdhocDraftEvent());
            }
            else
            {
                GlobalContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool TargetIsUnoccupiedTileInRange(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null && targetSlice.UnitEntity == null &&
                   UnitMovingContext.CanEndMoveAtCoordinates(targetSlice.MapCoordinates);
        }
    }
}