using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
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
            if (GameContext.ActiveUnit.RemoveItemFromInventory(itemTile as IItem))
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
                    },
                    1
                );
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(toastContent, 50);
                AssetManager.DropItemSFX.Play();
            }

            Complete = true;
        }

        private void DropItemAtCoordinates()
        {
            itemTile.SnapToCoordinates(dropCoordinates);
            MapContainer.GameGrid[(int) Layer.Items][(int) dropCoordinates.X, (int) dropCoordinates.Y] = itemTile;
        }
    }
}