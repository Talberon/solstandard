using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class VendorPurchase : UnitAction
    {
        private readonly Vendor vendor;

        public VendorPurchase(IItem item, int price, Vendor vendor) : base(
            icon: item.Icon.Clone(),
            name: "Buy " + item.Name + ": " + price + Currency.CurrencyAbbreviation,
            description: new WindowContentGrid(new[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont, "Effect:")
                    },
                    {
                        item.UseAction().Description
                    }
                }
            ),
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1},
            freeAction: true
        )
        {
            Item = item;
            Price = price;
            this.vendor = vendor;
        }


        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Vector2 vendorCoordinates = vendor.MapCoordinates;

            MapContainer.GameGrid[(int) mapLayer][(int) vendorCoordinates.X, (int) vendorCoordinates.Y] =
                new MapDistanceTile(TileSprite, vendorCoordinates);

            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(vendorCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsVendor(targetSlice))
            {
                if (ActiveTeamCanAffordItem())
                {
                    vendor.RemoveBuyActionForItem(Item);

                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(
                        new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                    );
                    eventQueue.Enqueue(new DecreaseTeamGoldEvent(Price));
                    eventQueue.Enqueue(new WaitFramesEvent(25));
                    eventQueue.Enqueue(new AddItemToUnitInventoryEvent(GlobalContext.ActiveUnit, Item.Duplicate()));
                    eventQueue.Enqueue(new WaitFramesEvent(50));
                    eventQueue.Enqueue(new AdditionalActionEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can not afford item!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public IItem Item { get; }

        public int Price { get; }

        private bool ActiveTeamCanAffordItem()
        {
            return GlobalContext.InitiativePhase.GetGoldForTeam(GlobalContext.ActiveTeam) >= Price;
        }

        private bool TargetIsVendor(MapSlice targetSlice)
        {
            return targetSlice.TerrainEntity == vendor;
        }
    }
}