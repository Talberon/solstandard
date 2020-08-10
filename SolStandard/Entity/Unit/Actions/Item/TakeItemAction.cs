using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Map;
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

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            base.GenerateActionGrid(origin, mapLayer);

            List<MapElement> elements = MapContainer.GetMapElementsFromLayer(mapLayer);

            RemoveUnselectableOptionsFromGrid(mapLayer, elements);

            MapElement firstTile = MapContainer.GetMapElementsFromLayer(mapLayer).FirstOrDefault();

            if (firstTile != null)
            {
                GlobalContext.WorldContext.MapContainer.MapCursor
                    .SnapCameraAndCursorToCoordinates(firstTile.MapCoordinates);
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                if (targetUnit.Inventory.Count > 0)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();
                    GlobalContext.WorldContext.OpenTakeItemMenu(targetUnit, true);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                        "Target has no items in inventory!", 50
                    );
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static void TakeItemFromInventory(GameUnit taker, GameUnit takenFrom, IItem itemToTake)
        {
            if (!takenFrom.Inventory.Contains(itemToTake)) return;

            taker.AddItemToInventory(itemToTake);
            takenFrom.RemoveItemFromInventory(itemToTake);
            AssetManager.CombatBlockSFX.Play();
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor($"Took {itemToTake.Name}!", 50);
        }

        private static void RemoveUnselectableOptionsFromGrid(Layer mapLayer, IEnumerable<MapElement> elements)
        {
            foreach (MapElement element in elements)
            {
                MapSlice elementSlice = MapContainer.GetMapSliceAtCoordinates(element.MapCoordinates);
                GameUnit targetUnit = UnitSelector.SelectUnit(elementSlice.UnitEntity);

                if (targetUnit == null || targetUnit.Team != GlobalContext.ActiveUnit.Team)
                {
                    MapContainer.GameGrid[(int) mapLayer][(int) elementSlice.MapCoordinates.X,
                        (int) elementSlice.MapCoordinates.Y] = null;
                }
            }
        }
    }
}