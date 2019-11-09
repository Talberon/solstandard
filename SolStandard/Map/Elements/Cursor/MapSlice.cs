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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            if (obj is MapSlice slice)
            {
                return Equals(slice);
            }

            return false;
        }

        private bool Equals(MapSlice other)
        {
            return Equals(UnitEntity, other.UnitEntity) &&
                   Equals(TerrainEntity, other.TerrainEntity) &&
                   Equals(ItemEntity, other.ItemEntity) &&
                   Equals(CollideTile, other.CollideTile) &&
                   Equals(TerrainDecorationTile, other.TerrainDecorationTile) &&
                   Equals(TerrainTile, other.TerrainTile) &&
                   MapCoordinates.Equals(other.MapCoordinates);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (UnitEntity != null ? UnitEntity.GetHashCode() : 0);
                hashCode += (hashCode * 397) ^ (TerrainEntity != null ? TerrainEntity.GetHashCode() : 0);
                hashCode += (hashCode * 397) ^ (ItemEntity != null ? ItemEntity.GetHashCode() : 0);
                hashCode += (hashCode * 397) ^ (CollideTile != null ? CollideTile.GetHashCode() : 0);
                hashCode += (hashCode * 397) ^
                            (TerrainDecorationTile != null ? TerrainDecorationTile.GetHashCode() : 0);
                hashCode += (hashCode * 397) ^ (TerrainTile != null ? TerrainTile.GetHashCode() : 0);
                hashCode += (hashCode * 397) ^ MapCoordinates.GetHashCode();
                return hashCode;
            }
        }
    }
}