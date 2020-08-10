using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class OpenChestAction : UnitAction
    {
        private readonly Vector2 targetCoordinates;
        private readonly Chest chest;

        public OpenChestAction(Chest chest, Vector2 targetCoordinates, bool freeAction = true) : base(
            icon: chest.RenderSprite,
            name: "Open Chest",
            description: "Opens a chest if able.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: freeAction
        )
        {
            this.chest = chest;
            this.targetCoordinates = targetCoordinates;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) targetCoordinates.X, (int) targetCoordinates.Y] =
                new MapDistanceTile(TileSprite, targetCoordinates);

            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(targetCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsUnopenedChest(targetSlice))
            {
                if (!chest.IsLocked)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    GlobalEventQueue.QueueSingleEvent(
                        new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                    );
                    GlobalEventQueue.QueueSingleEvent(new ToggleOpenEvent(chest));
                    GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(5));

                    chest.TakeContents();

                    if (FreeAction)
                    {
                        GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
                    }
                    else
                    {
                        GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
                    }
                }
                else
                {
                    Key matchingKey = ActiveUnitMatchingKey(targetSlice);

                    if (matchingKey != null)
                    {
                        new ToggleLockAction(matchingKey).ExecuteAction(targetSlice);
                    }
                    else
                    {
                        GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Chest is locked!", 50);
                        AssetManager.LockedSFX.Play();
                    }
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Cannot open chest here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static Key ActiveUnitMatchingKey(MapSlice targetSlice)
        {
            return GlobalContext.ActiveUnit.Inventory.Where(item => item is Key).Cast<Key>()
                .FirstOrDefault(key => key.UsedWith == targetSlice.TerrainEntity.Name);
        }

        private bool TargetIsUnopenedChest(MapSlice targetSlice)
        {
            return chest == targetSlice.TerrainEntity
                   && !chest.IsOpen
                   && targetSlice.DynamicEntity != null
                   && targetSlice.UnitEntity == null;
        }
    }
}