using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class PickUpCurrencyAction : UnitAction
    {
        private readonly Currency currency;

        public PickUpCurrencyAction(Currency currency) : base(
            icon: currency.RenderSprite.Clone(),
            name: "Pick Up",
            description: "Add [" + currency.Value + "] to your unit's money count.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: currency.InteractRange,
            freeAction: true
        )
        {
            this.currency = currency;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (SelectingItemAtUnitLocation(targetSlice))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new PickUpCurrencyEvent(currency));
                eventQueue.Enqueue(new WaitFramesEvent(50));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Cannot pick up money here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool SelectingItemAtUnitLocation(MapSlice targetSlice)
        {
            return currency.MapCoordinates == GlobalContext.ActiveUnit.UnitEntity.MapCoordinates &&
                   targetSlice.DynamicEntity != null;
        }
    }
}