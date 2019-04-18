using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class VendorPurchase : UnitAction
    {
        private readonly IItem item;
        private readonly int price;
        private readonly Vendor vendor;

        public VendorPurchase(IItem item, int price, Vendor vendor) : base(
            icon: item.Icon,
            name: "Purchase " + item.Name + ": " + price + Currency.CurrencyAbbreviation,
            description: new WindowContentGrid(new[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont, "Effect:"),
                    },
                    {
                        item.UseAction().Description
                    }
                },
                1
            ),
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1},
            freeAction: true
        )
        {
            this.item = item;
            this.price = price;
            this.vendor = vendor;
        }


        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Vector2 vendorCoordinates = vendor.MapCoordinates;

            MapContainer.GameGrid[(int) mapLayer][(int) vendorCoordinates.X, (int) vendorCoordinates.Y] =
                new MapDistanceTile(TileSprite, vendorCoordinates);

            GameContext.GameMapContext.MapContainer.MapCursor.SnapCursorToCoordinates(vendorCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsVendor(targetSlice))
            {
                if (ActiveUnitCanAffordItem())
                {
                    vendor.RemoveBuyActionForItem(item);

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new DecreaseUnitGoldEvent(price));
                    eventQueue.Enqueue(new WaitFramesEvent(25));
                    eventQueue.Enqueue(new AddItemToUnitInventoryEvent(GameContext.ActiveUnit, item));
                    eventQueue.Enqueue(new WaitFramesEvent(50));
                    eventQueue.Enqueue(new AdditionalActionEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can not afford item!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public IItem Item
        {
            get { return item; }
        }

        private bool ActiveUnitCanAffordItem()
        {
            return GameContext.ActiveUnit.CurrentGold >= price;
        }

        private bool TargetIsVendor(MapSlice targetSlice)
        {
            return targetSlice.TerrainEntity == vendor;
        }
    }
}