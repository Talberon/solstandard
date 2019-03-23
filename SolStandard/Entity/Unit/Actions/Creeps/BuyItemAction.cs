using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class BuyItemAction : UnitAction
    {
        private readonly IItem item;
        private readonly int price;

        public BuyItemAction(IItem item, int price) : base(
            icon: item.Icon,
            name: "Purchase " + item.Name + ": " + price + Currency.CurrencyAbbreviation,
            description: "Effect:" + Environment.NewLine + item.UseAction().Description,
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1}
        )
        {
            this.item = item;
            this.price = price;
        }

        private GameUnit Merchant
        {
            get { return GameContext.Units.Find(unit => unit.ContextualActions.Contains(this)); }
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Vector2 merchantCoordinates = Merchant.UnitEntity.MapCoordinates;

            MapContainer.GameGrid[(int) mapLayer][(int) merchantCoordinates.X, (int) merchantCoordinates.Y] =
                new MapDistanceTile(TileSprite, merchantCoordinates);

            GameContext.GameMapContext.MapContainer.MapCursor.SnapCursorToCoordinates(merchantCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit actingUnit = GameContext.ActiveUnit;
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsUnitInRange(targetSlice, targetUnit))
            {
                if (ActiveUnitCanAffordItem())
                {
                    Merchant.ContextualActions.Remove(this);

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new TransferUnitGoldEvent(actingUnit, targetUnit, price,
                        Currency.GoldIcon(new Vector2(16))));
                    eventQueue.Enqueue(new WaitFramesEvent(25));
                    eventQueue.Enqueue(new TransferUnitItemEvent(targetUnit, actingUnit, item));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
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

        private bool ActiveUnitCanAffordItem()
        {
            return GameContext.ActiveUnit.CurrentGold >= price;
        }
    }
}