using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class TakeItemAction : UnitAction
    {
        public TakeItemAction() : base(
            icon: MiscIconProvider.GetMiscIcon(MiscIcon.Spoils, GameDriver.CellSizeVector),
            name: "Take Item",
            description: "Take this item from an ally in range.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: true
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                if (targetUnit.Inventory.Count > 0)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();
                    GameContext.GameMapContext.OpenTakeItemMenu(targetUnit, true);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                        "Target has no items in inventory!", 50
                    );
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static void TakeItemFromInventory(GameUnit taker, GameUnit takenFrom, IItem itemToTake)
        {
            if (!takenFrom.Inventory.Contains(itemToTake)) return;

            taker.AddItemToInventory(itemToTake);
            takenFrom.RemoveItemFromInventory(itemToTake);
            AssetManager.CombatBlockSFX.Play();
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor($"Took {itemToTake.Name}!", 50);
        }
    }
}