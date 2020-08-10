using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class DropItemEvent : IEvent
    {
        private readonly TerrainEntity itemTile;
        private readonly Vector2 dropCoordinates;

        public DropItemEvent(TerrainEntity itemTile, Vector2 dropCoordinates)
        {
            this.dropCoordinates = dropCoordinates;
            this.itemTile = itemTile;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            if (GlobalContext.ActiveUnit.RemoveItemFromInventory(itemTile as IItem))
            {
                DropItemAtCoordinates();

                IRenderable toastContent = new WindowContentGrid(
                    new[,]
                    {
                        {
                            SpriteResizer.TryResizeRenderable(itemTile.RenderSprite,
                                new Vector2(MapContainer.MapToastIconSize)),
                            new RenderText(AssetManager.MapFont, "Dropped " + itemTile.Name + "!")
                        }
                    }
                );
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(toastContent, 50);
                AssetManager.DropItemSFX.Play();
            }

            WorldContext.WorldHUD.GenerateObjectiveWindow();

            Complete = true;
        }

        private void DropItemAtCoordinates()
        {
            itemTile.SnapToCoordinates(dropCoordinates);

            MapElement targetItemElement =
                MapContainer.GameGrid[(int) Layer.Items][(int) dropCoordinates.X, (int) dropCoordinates.Y];


            var newItemToDrop = itemTile as IItem;
            List<IItem> itemsToDrop;
            int goldToDrop;

            switch (targetItemElement)
            {
                case Spoils targetSpoils:
                    itemsToDrop = targetSpoils.Items;
                    itemsToDrop.Add(newItemToDrop);
                    goldToDrop = targetSpoils.Gold;
                    break;
                case Currency currency:
                    itemsToDrop = new List<IItem> {newItemToDrop};
                    goldToDrop = currency.Value;
                    break;
                case IItem existingItem:
                    itemsToDrop = new List<IItem>
                    {
                        existingItem,
                        newItemToDrop
                    };
                    goldToDrop = 0;
                    break;
                default:
                    itemsToDrop = new List<IItem> {newItemToDrop};
                    goldToDrop = 0;
                    break;
            }

            var spoilsToDrop = new Spoils(
                "Item Bag",
                "Spoils",
                MiscIconProvider.GetMiscIcon(MiscIcon.Spoils, GameDriver.CellSizeVector),
                dropCoordinates,
                goldToDrop,
                itemsToDrop
            );

            MapContainer.GameGrid[(int) Layer.Items][(int) dropCoordinates.X, (int) dropCoordinates.Y] = spoilsToDrop;
        }
    }
}