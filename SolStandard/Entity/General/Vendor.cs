using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Scenario;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Entity.General
{
    public class Vendor : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; }
        private readonly Dictionary<UnitAction, int> purchaseActions;
        private IRenderable itemList;

        public Vendor(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool canMove,
            int[] interactRange, IReadOnlyList<IItem> items, IReadOnlyList<int> prices, int[] quantities) :
            base(name, type, sprite, mapCoordinates)
        {
            if (items.Count != prices.Count || items.Count != quantities.Length)
            {
                throw new VendorMisconfiguredException("The vendor <" + name + "> entity " +
                                                       "should have the same number of items, prices and quantities.",
                    items.Count, prices.Count, quantities.Length);
            }

            CanMove = canMove;
            InteractRange = interactRange;

            purchaseActions = new Dictionary<UnitAction, int>();
            for (int i = 0; i < items.Count; i++)
            {
                purchaseActions.Add(new VendorPurchase(items[i], prices[i], this), quantities[i]);
            }

            itemList = GenerateItemList();
        }

        public void RemoveBuyActionForItem(IItem item)
        {
            UnitAction actionToRemove = null;

            foreach (KeyValuePair<UnitAction, int> purchaseActionKeyPair in purchaseActions)
            {
                if (!(purchaseActionKeyPair.Key is VendorPurchase buyAction) ||
                    buyAction.Item.Name != item.Name) continue;

                purchaseActions[buyAction]--;

                if (purchaseActions[buyAction] < 1)
                {
                    actionToRemove = buyAction;
                }

                break;
            }

            if (actionToRemove != null)
            {
                purchaseActions.Remove(actionToRemove);
            }

            itemList = GenerateItemList();
        }

        public IEnumerable<IItem> Items
        {
            get { return purchaseActions.Keys.Cast<VendorPurchase>().Select(action => action.Item).ToList(); }
        }

        public List<UnitAction> TileActions()
        {
            return purchaseActions.Keys.ToList();
        }

        private IRenderable GenerateItemList()
        {
            var itemDetailList = new IRenderable[purchaseActions.Count, 5];

            List<VendorPurchase> purchaseActionsList = purchaseActions.Keys.Cast<VendorPurchase>().ToList();

            for (int i = 0; i < purchaseActions.Count; i++)
            {
                itemDetailList[i, 0] = purchaseActionsList[i].Icon.Clone();
                itemDetailList[i, 1] = new RenderText(AssetManager.WindowFont, purchaseActionsList[i].Item.Name);

                itemDetailList[i, 2] =
                    ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Taxes, GameDriver.CellSizeVector);

                //Price
                itemDetailList[i, 3] = new RenderText(AssetManager.WindowFont,
                    $"{purchaseActionsList[i].Price}{Currency.CurrencyAbbreviation}");

                //Quantity
                itemDetailList[i, 4] = new RenderText(AssetManager.WindowFont,
                    $"[{purchaseActions[purchaseActionsList[i]]}]");
            }

            return new WindowContentGrid(itemDetailList, 1, HorizontalAlignment.Right);
        }

        protected override IRenderable EntityInfo => itemList;
    }
}