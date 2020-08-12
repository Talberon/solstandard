using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class RailgunAction : UnitAction
    {
        private readonly WeaponStatistics weaponStatistics;
        private readonly int range;

        public RailgunAction(IRenderable tileIcon, int range, WeaponStatistics weaponStatistics) : base(
            icon: tileIcon,
            name: "Railgun",
            description: "Attack a target at an extended linear range based on the range of this weapon.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null,
            freeAction: false
        )
        {
            this.range = range;
            this.weaponStatistics = weaponStatistics;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            GenerateRealLinearTargetingGrid(origin, range, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GlobalEventQueue.QueueSingleEvent(
                    new StartCombatEvent(
                        targetUnit,
                        false,
                        GlobalContext.ActiveUnit.Stats.ApplyWeaponStatistics(weaponStatistics)
                    )
                );
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a valid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private void GenerateRealLinearTargetingGrid(Vector2 origin, int maxRange, Layer mapLayer)
        {
            var attackTiles = new List<MapDistanceTile>();

            for (int i = 1; i <= maxRange; i++)
            {
                var northTile = new Vector2(origin.X, origin.Y - i);
                var southTile = new Vector2(origin.X, origin.Y + i);
                var eastTile = new Vector2(origin.X + i, origin.Y);
                var westTile = new Vector2(origin.X - i, origin.Y);

                AddTileWithinMapBounds(attackTiles, northTile, i);
                AddTileWithinMapBounds(attackTiles, southTile, i);
                AddTileWithinMapBounds(attackTiles, eastTile, i);
                AddTileWithinMapBounds(attackTiles, westTile, i);
            }

            AddVisitedTilesToGameGrid(attackTiles, mapLayer);
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