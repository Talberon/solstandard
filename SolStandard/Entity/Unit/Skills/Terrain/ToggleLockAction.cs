using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills.Terrain
{
    public class ToggleLockAction : UnitAction
    {
        private readonly Key key;

        public ToggleLockAction(Key key) : base(
            icon: key.Icon,
            name: "Use: " + key.Name,
            description: "Locks or unlocks the target if you have the appropriate key.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
            this.key = key;
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            ILockable targetUnlockable = targetSlice.TerrainEntity as ILockable;

            if (KeyWorksOnLock(targetSlice, targetUnlockable))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new ToggleLockEvent(targetUnlockable));
                eventQueue.Enqueue(new WaitFramesEvent(5));
                eventQueue.Enqueue(new ToggleOpenEvent(targetSlice.TerrainEntity as IOpenable));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        private bool KeyWorksOnLock(MapSlice targetSlice, ILockable targetUnlockable)
        {
            return targetUnlockable != null
                   && targetSlice.DynamicEntity != null
                   && targetSlice.UnitEntity == null
                   && key.UsedWith == targetSlice.TerrainEntity.Name;
        }
    }
}