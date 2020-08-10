using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class Wait : UnitAction
    {
        public Wait() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Wait, GameDriver.CellSizeVector),
            name: "Wait",
            description: "Take no action and end your turn.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: false
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (targetSlice.DynamicEntity != null)
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitActionEvent());
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't wait here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}