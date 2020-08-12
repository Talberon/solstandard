using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Rogue
{
    public class PickDoorLock : UnitAction
    {
        private readonly int doorDamage;

        public PickDoorLock(int doorDamage) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.PickLock, GameDriver.CellSizeVector),
            name: "Pick Lock - Door",
            description: $"Deal [{doorDamage}] damage to target door.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.doorDamage = doorDamage;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsADoorInRange(targetSlice))
            {
                var targetDoor = targetSlice.TerrainEntity as Door;
                targetDoor?.DealDamage(doorDamage);
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor($"Door takes [{doorDamage}] damage!",
                    50);

                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
                GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target door!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool TargetIsADoorInRange(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null && targetSlice.TerrainEntity is Door;
        }
    }
}