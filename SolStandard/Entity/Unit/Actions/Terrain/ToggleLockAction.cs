using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
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
            name: "Use: " + key.Name,
            description: "Locks or unlocks the target if you have the appropriate key.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: true
        )
        {
            this.key = key;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            ILockable targetUnlockable = targetSlice.TerrainEntity as ILockable;

            if (targetUnlockable != null && KeyWorksOnLock(targetSlice, targetUnlockable))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                targetUnlockable.ToggleLock();

                GlobalEventQueue.QueueSingleEvent(new DeleteItemEvent(key));

                if (targetUnlockable is Chest)
                {
                    Chest targetChest = targetUnlockable as Chest;
                    new OpenChestAction(targetChest, targetSlice.MapCoordinates).ExecuteAction(targetSlice);
                }
                else if (targetUnlockable is Door)
                {
                    Door targetDoor = targetUnlockable as Door;
                    new UseDoorAction(targetDoor, targetSlice.MapCoordinates).ExecuteAction(targetSlice);
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Key doesn't work here!", 50);
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
            Chest targetChest = targetUnlockable as Chest;
            return targetChest != null && !targetChest.IsOpen && targetChest.IsLocked;
        }
    }
}