using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills.Terrain
{
    public class ToggleSwitchAction : UnitAction
    {
        private readonly ILockable targetLockable;

        public ToggleSwitchAction(MapEntity switchTile, ILockable targetLockable) : base(
            icon: switchTile.RenderSprite,
            name: "Use: " + switchTile.Name,
            description: "Opens or closes the target lockable.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
            this.targetLockable = targetLockable;
        }


        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            if (SwitchWorksOnLock(targetSlice))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new ToggleLockEvent(targetLockable));
                eventQueue.Enqueue(new WaitFramesEvent(5));
                eventQueue.Enqueue(
                    new ToggleOpenEvent(
                        targetLockable as IOpenable, AssetManager.MenuConfirmSFX, AssetManager.MenuConfirmSFX
                    )
                );
                eventQueue.Enqueue(new WaitFramesEvent(5));
                //Relock the tile after opening/closing it
                eventQueue.Enqueue(new ToggleLockEvent(targetLockable));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        private bool SwitchWorksOnLock(MapSlice targetSlice)
        {
            return targetLockable != null && targetSlice.DynamicEntity != null;
        }
    }
}