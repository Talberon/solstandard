using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Containers.Contexts
{
    public class UnitTargetingContext
    {
        private readonly MapLayer mapLayer;
        private readonly TextureCell textureCell;

        public UnitTargetingContext(MapLayer mapLayer, TextureCell textureCell)
        {
            this.mapLayer = mapLayer;
            this.textureCell = textureCell;
        }

        public void GenerateTargetingGrid(Vector2 origin, int[] range)
        {
            List<MapDistanceTile> targetingTiles = new List<MapDistanceTile>();

            foreach (int distance in range)
            {
                targetingTiles.Add(new MapDistanceTile(textureCell, new Vector2(origin.X + distance, origin.Y), distance));
                targetingTiles.Add(new MapDistanceTile(textureCell, new Vector2(origin.X - distance, origin.Y), distance));
                targetingTiles.Add(new MapDistanceTile(textureCell, new Vector2(origin.X, origin.Y + distance), distance));
                targetingTiles.Add(new MapDistanceTile(textureCell, new Vector2(origin.X, origin.Y - distance), distance));

                if (distance > 1)
                {
                    int subdistance = distance - 1; 
                    
                    targetingTiles.Add(new MapDistanceTile(textureCell,
                        new Vector2(origin.X + subdistance, origin.Y + subdistance), distance));
                    targetingTiles.Add(new MapDistanceTile(textureCell,
                        new Vector2(origin.X + subdistance, origin.Y - subdistance), distance));
                    targetingTiles.Add(new MapDistanceTile(textureCell,
                        new Vector2(origin.X - subdistance, origin.Y - subdistance), distance));
                    targetingTiles.Add(new MapDistanceTile(textureCell,
                        new Vector2(origin.X - subdistance, origin.Y + subdistance), distance));
                }
            }
            
            AddTargetingTilesToGameGrid(targetingTiles);
        }
        
        private void AddTargetingTilesToGameGrid(IEnumerable<MapDistanceTile> targetingTiles)
        {
            foreach (MapDistanceTile tile in targetingTiles)
            {
                mapLayer.GameGrid[(int) Layer.Dynamic][(int) tile.Coordinates.X, (int) tile.Coordinates.Y] = tile;
            }
        }
    }
}