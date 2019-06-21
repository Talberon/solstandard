using Microsoft.Xna.Framework;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;

namespace SolStandard.Map.Elements.Cursor
{
    public class MapSlice
    {
        public MapSlice(Vector2 mapCoordinates, UnitEntity unitEntity, MapElement previewEntity,
            MapElement dynamicEntity, TerrainEntity terrainEntity, TerrainEntity itemEntity, MapTile collideTile,
            MapTile terrainDecorationTile, MapTile terrainTile)
        {
            UnitEntity = unitEntity;
            PreviewEntity = previewEntity;
            DynamicEntity = dynamicEntity;
            TerrainEntity = terrainEntity;
            ItemEntity = itemEntity;
            CollideTile = collideTile;
            TerrainDecorationTile = terrainDecorationTile;
            TerrainTile = terrainTile;
            MapCoordinates = mapCoordinates;
        }

        public UnitEntity UnitEntity { get; }

        public MapElement PreviewEntity { get; }

        public MapElement DynamicEntity { get; }

        public TerrainEntity TerrainEntity { get; }

        public TerrainEntity ItemEntity { get; }

        public MapTile CollideTile { get; }

        public MapTile TerrainDecorationTile { get; }

        public MapTile TerrainTile { get; }

        public Vector2 MapCoordinates { get; }
    }
}