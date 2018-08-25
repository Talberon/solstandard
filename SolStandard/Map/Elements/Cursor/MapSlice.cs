namespace SolStandard.Map.Elements.Cursor
{
    public class MapSlice
    {
        private readonly MapEntity unitEntity;
        private readonly MapElement dynamicEntity;
        private readonly MapEntity generalEntity;
        private readonly MapTile collideTile;
        private readonly MapTile terrainTile;

        public MapSlice(MapEntity unitEntity, MapElement dynamicEntity, MapEntity generalEntity, MapTile collideTile,
            MapTile terrainTile)
        {
            this.unitEntity = unitEntity;
            this.dynamicEntity = dynamicEntity;
            this.generalEntity = generalEntity;
            this.collideTile = collideTile;
            this.terrainTile = terrainTile;
        }

        public MapEntity UnitEntity
        {
            get { return unitEntity; }
        }

        public MapElement DynamicEntity
        {
            get { return dynamicEntity; }
        }

        public MapEntity GeneralEntity
        {
            get { return generalEntity; }
        }

        public MapTile CollideTile
        {
            get { return collideTile; }
        }

        public MapTile TerrainTile
        {
            get { return terrainTile; }
        }
    }
}