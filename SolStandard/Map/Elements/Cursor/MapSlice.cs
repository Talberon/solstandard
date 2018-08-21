namespace SolStandard.Map.Elements.Cursor
{
    public class MapSlice
    {
        private readonly MapEntity unitEntity;
        private readonly MapEntity generalEntity;
        private readonly MapTile collideTile;
        private readonly MapTile terrainTile;

        public MapSlice(MapEntity unitEntity, MapEntity generalEntity, MapTile collideTile, MapTile terrainTile)
        {
            this.unitEntity = unitEntity;
            this.generalEntity = generalEntity;
            this.collideTile = collideTile;
            this.terrainTile = terrainTile;
        }

        public MapEntity UnitEntity
        {
            get { return unitEntity; }
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