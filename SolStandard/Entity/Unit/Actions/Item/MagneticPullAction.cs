using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
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
                icon: skillIcon,
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
            List<MapDistanceTile> attackTiles = new List<MapDistanceTile>();

            foreach (int skillRange in Range)
            {
                Vector2 northTile = new Vector2(origin.X, origin.Y - skillRange);
                Vector2 southTile = new Vector2(origin.X, origin.Y + skillRange);
                Vector2 eastTile = new Vector2(origin.X + skillRange, origin.Y);
                Vector2 westTile = new Vector2(origin.X - skillRange, origin.Y);

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

                        Queue<IEvent> eventQueue = new Queue<IEvent>();
                        eventQueue.Enqueue(new PullEvent(targetUnit));
                        eventQueue.Enqueue(new WaitFramesEvent(10));
                        eventQueue.Enqueue(new AdditionalActionEvent());
                        GlobalEventQueue.QueueEvents(eventQueue);
                    }
                    else
                    {
                        GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Obstructed/Immovable!", 50);
                        AssetManager.WarningSFX.Play();
                    }
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a unit in range!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Item is broken!", 50);
                AssetManager.WarningSFX.Play();
            }
        }


        private void AddTileWithinMapBounds(ICollection<MapDistanceTile> tiles, Vector2 tileCoordinates, int distance)
        {
            if (GameMapContext.CoordinatesWithinMapBounds(tileCoordinates))
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