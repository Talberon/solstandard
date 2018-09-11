using SolStandard.Entity.General;
using SolStandard.Entity.Unit;

namespace SolStandard.Map.Elements.Cursor
{
    public class MapSlice
    {
        private readonly UnitEntity unitEntity;
        private readonly MapElement dynamicEntity;
        private readonly TerrainEntity terrainEntity;
        private readonly MapTile collideTile;
        private readonly MapTile terrainDecorationTile;
        private readonly MapTile terrainTile;

        public MapSlice(UnitEntity unitEntity, MapElement dynamicEntity, TerrainEntity terrainEntity,
            MapTile collideTile, MapTile terrainDecorationTile, MapTile terrainTile)
        {
            this.unitEntity = unitEntity;
            this.dynamicEntity = dynamicEntity;
            this.terrainEntity = terrainEntity;
            this.collideTile = collideTile;
            this.terrainDecorationTile = terrainDecorationTile;
            this.terrainTile = terrainTile;
        }

        public UnitEntity UnitEntity
        {
            get { return unitEntity; }
        }

        public MapElement DynamicEntity
        {
            get { return dynamicEntity; }
        }

        public TerrainEntity TerrainEntity
        {
            get { return terrainEntity; }
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
    }
}