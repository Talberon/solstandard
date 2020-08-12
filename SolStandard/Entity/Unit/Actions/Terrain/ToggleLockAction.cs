using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class ToggleLockAction : UnitAction
    {
        private readonly Key key;

        public ToggleLockAction(Key key) : base(
            icon: key.Icon,
            name: "Use",
            description: $"Locks or unlocks a target ${key.UsedWith}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: true
        )
        {
            this.key = key;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (targetSlice.TerrainEntity is ILockable targetUnlockable &&
                KeyWorksOnLock(targetSlice, targetUnlockable))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                targetUnlockable.ToggleLock();

                GlobalEventQueue.QueueSingleEvent(new DeleteItemEvent(key));

                switch (targetUnlockable)
                {
                    case Chest targetChest:
                        new OpenChestAction(targetChest, targetSlice.MapCoordinates).ExecuteAction(targetSlice);
                        break;
                    case Door door:
                        new UseDoorAction(door, targetSlice.MapCoordinates).ExecuteAction(targetSlice);
                        break;
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Key doesn't work here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool KeyWorksOnLock(MapSlice targetSlice, ILockable targetUnlockable)
        {
            return targetUnlockable != null
                   && targetSlice.DynamicEntity != null
                   && targetSlice.UnitEntity == null
                   && (key.IsMasterKey || key.UsedWith == targetSlice.TerrainEntity.Name)
                   && (!(targetUnlockable is Chest) || LockedChestIsNotOpen(targetUnlockable));
        }

        private static bool LockedChestIsNotOpen(ILockable targetUnlockable)
        {
            return targetUnlockable is Chest targetChest && !targetChest.IsOpen && targetChest.IsLocked;
        }
    }
}