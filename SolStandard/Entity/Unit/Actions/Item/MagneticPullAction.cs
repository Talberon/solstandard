using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Actions.Champion;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class MagneticPullAction : UnitAction
    {
        private readonly Magnet magnet;

        public MagneticPullAction(Magnet magnet, IRenderable skillIcon, int[] skillRange) :
            base(
                icon: skillIcon.Clone(),
                name: "Pull Unit",
                description: "Pull target unit towards you.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: skillRange,
                freeAction: true
            )
        {
            this.magnet = magnet;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            var attackTiles = new List<MapDistanceTile>();

            foreach (int skillRange in Range)
            {
                var northTile = new Vector2(origin.X, origin.Y - skillRange);
                var southTile = new Vector2(origin.X, origin.Y + skillRange);
                var eastTile = new Vector2(origin.X + skillRange, origin.Y);
                var westTile = new Vector2(origin.X - skillRange, origin.Y);

                AddTileWithinMapBounds(attackTiles, northTile, skillRange);
                AddTileWithinMapBounds(attackTiles, southTile, skillRange);
                AddTileWithinMapBounds(attackTiles, eastTile, skillRange);
                AddTileWithinMapBounds(attackTiles, westTile, skillRange);
            }

            AddVisitedTilesToGameGrid(attackTiles, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (!magnet.IsBroken)
            {
                if (TargetIsUnitInRange(targetSlice, targetUnit))
                {
                    if (Challenge.CanPull(targetSlice, targetUnit))
                    {
                        magnet.DecrementRemainingUses();

                        MapContainer.ClearDynamicAndPreviewGrids();

                        var eventQueue = new Queue<IEvent>();
                        eventQueue.Enqueue(new PullEvent(targetUnit));
                        eventQueue.Enqueue(new WaitFramesEvent(10));
                        eventQueue.Enqueue(new AdditionalActionEvent());
                        GlobalEventQueue.QueueEvents(eventQueue);
                    }
                    else
                    {
                        GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Obstructed/Immovable!", 50);
                        AssetManager.WarningSFX.Play();
                    }
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a unit in range!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Item is broken!", 50);
                AssetManager.WarningSFX.Play();
            }
        }


        private void AddTileWithinMapBounds(ICollection<MapDistanceTile> tiles, Vector2 tileCoordinates, int distance)
        {
            if (WorldContext.CoordinatesWithinMapBounds(tileCoordinates))
            {
                tiles.Add(new MapDistanceTile(TileSprite, tileCoordinates, distance));
            }
        }

        private static void AddVisitedTilesToGameGrid(IEnumerable<MapDistanceTile> visitedTiles, Layer layer)
        {
            foreach (MapDistanceTile tile in visitedTiles)
            {
                MapContainer.GameGrid[(int) layer][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y] = tile;
            }
        }
    }
}