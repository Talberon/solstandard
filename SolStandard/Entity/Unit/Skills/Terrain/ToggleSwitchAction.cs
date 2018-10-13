using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills.Terrain
{
    public class ToggleSwitchAction : UnitAction
    {
        private readonly List<ILockable> targetLockables;
        private Switch switchTile;

        public ToggleSwitchAction(Switch switchTile, List<ILockable> targetLockables) : base(
            icon: switchTile.RenderSprite,
            name: "Use: " + switchTile.Name,
            description: "Opens or closes the target lockable.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
            this.switchTile = switchTile;
            this.targetLockables = targetLockables;
        }


        public override void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext, BattleContext battleContext)
        {
            if (TargetingSwitch(targetSlice) && NothingObstructingSwitchTarget())
            {
                switchTile.ToggleActive();

                MapContainer.ClearDynamicAndPreviewGrids();
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                foreach (ILockable lockable in targetLockables)
                {
                    eventQueue.Enqueue(new ToggleOpenEvent(lockable as IOpenable));
                }

                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref gameMapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                if (!TargetingSwitch(targetSlice))
                {
                    MapContainer.AddNewToastAtMapCursor("Not a target switch!", 50);
                }

                if (!NothingObstructingSwitchTarget())
                {
                    MapContainer.AddNewToastAtMapCursor("Switch target is obstructed!", 50);
                }

                AssetManager.WarningSFX.Play();
            }
        }

        private static bool TargetingSwitch(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null &&
                   targetSlice.TerrainEntity is Switch;
        }

        private bool NothingObstructingSwitchTarget()
        {
            //Don't allow the switch to be triggered if any target tiles have a unit on them
            foreach (ILockable lockable in targetLockables)
            {
                TerrainEntity entity = lockable as TerrainEntity;
                if (entity == null) continue;

                if (MapContainer.GetMapSliceAtCoordinates(entity.MapCoordinates).UnitEntity != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}