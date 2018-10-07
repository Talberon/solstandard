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

        public ToggleSwitchAction(MapEntity switchTile, List<ILockable> targetLockables) : base(
            icon: switchTile.RenderSprite,
            name: "Use: " + switchTile.Name,
            description: "Opens or closes the target lockable.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
            this.targetLockables = targetLockables;
        }


        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            if (TargetingSwitch(targetSlice))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();

                foreach (ILockable lockable in targetLockables)
                {
                    eventQueue.Enqueue(new ToggleLockEvent(lockable));
                }

                eventQueue.Enqueue(new WaitFramesEvent(5));

                foreach (ILockable lockable in targetLockables)
                {
                    eventQueue.Enqueue(
                        new ToggleOpenEvent(
                            lockable as IOpenable, AssetManager.MenuConfirmSFX, AssetManager.MenuConfirmSFX
                        )
                    );
                }

                eventQueue.Enqueue(new WaitFramesEvent(5));
                //Relock the tile after opening/closing it
                foreach (ILockable lockable in targetLockables)
                {
                    eventQueue.Enqueue(new ToggleLockEvent(lockable));
                }

                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        private bool TargetingSwitch(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null &&
                   targetSlice.TerrainEntity is Switch;
        }
    }
}