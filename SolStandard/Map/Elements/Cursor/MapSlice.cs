using Microsoft.Xna.Framework;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;

namespace SolStandard.Map.Elements.Cursor
{
    public class MapSlice
    {
        private readonly Vector2 mapCoordinates;
        private readonly UnitEntity unitEntity;
        private readonly MapElement previewEntity;
        private readonly MapElement dynamicEntity;
        private readonly TerrainEntity terrainEntity;
        private readonly TerrainEntity itemEntity;
        private readonly MapTile collideTile;
        private readonly MapTile terrainDecorationTile;
        private readonly MapTile terrainTile;

        public MapSlice(Vector2 mapCoordinates, UnitEntity unitEntity, MapElement previewEntity,
            MapElement dynamicEntity, TerrainEntity terrainEntity, TerrainEntity itemEntity, MapTile collideTile,
            MapTile terrainDecorationTile, MapTile terrainTile)
        {
            this.unitEntity = unitEntity;
            this.previewEntity = previewEntity;
            this.dynamicEntity = dynamicEntity;
            this.terrainEntity = terrainEntity;
            this.itemEntity = itemEntity;
            this.collideTile = collideTile;
            this.terrainDecorationTile = terrainDecorationTile;
            this.terrainTile = terrainTile;
            this.mapCoordinates = mapCoordinates;
        }

        public UnitEntity UnitEntity
        {
            get { return unitEntity; }
        }

        public MapElement PreviewEntity
        {
            get { return previewEntity; }
        }

        public MapElement DynamicEntity
        {
            get { return dynamicEntity; }
        }

        public TerrainEntity TerrainEntity
        {
            get { return terrainEntity; }
        }

        public TerrainEntity ItemEntity
        {
            get { return itemEntity; }
        }

        public MapTile CollideTile
        {
            get { return collideTile; }
        }

        public MapTile TerrainDecorationTile
        {
            get { return terrainDecorationTile; }
        }

        public MapTile TerrainTile
        {
            get { return terrainTile; }
        }

        public Vector2 MapCoordinates
        {
            get { return mapCoordinates; }
        }
    }
}