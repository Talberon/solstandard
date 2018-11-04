﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class RailgunAction : UnitAction
    {
        private readonly int range;

        public RailgunAction(IRenderable tileIcon, int range) : base(
            icon: tileIcon,
            name: "Railgun",
            description: "Attack a target at an extended linear range based on the range of this weapon." +
                         "\nAttack using your own attack statistic.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
            this.range = range;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            GenerateRealLinearTargetingGrid(origin, range, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            new BasicAttack().ExecuteAction(targetSlice);
        }

        private void GenerateRealLinearTargetingGrid(Vector2 origin, int maxRange, Layer mapLayer)
        {
            List<MapDistanceTile> attackTiles = new List<MapDistanceTile>();

            for (int i = 1; i <= maxRange; i++)
            {
                Vector2 northTile = new Vector2(origin.X, origin.Y - i);
                Vector2 southTile = new Vector2(origin.X, origin.Y + i);
                Vector2 eastTile = new Vector2(origin.X + i, origin.Y);
                Vector2 westTile = new Vector2(origin.X - i, origin.Y);

                AddTileWithinMapBounds(attackTiles, northTile, i);
                AddTileWithinMapBounds(attackTiles, southTile, i);
                AddTileWithinMapBounds(attackTiles, eastTile, i);
                AddTileWithinMapBounds(attackTiles, westTile, i);
            }

            AddVisitedTilesToGameGrid(attackTiles, mapLayer);
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