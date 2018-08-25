using System;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public enum UnitClass
    {
        Champion,
        Archer,
        Mage,
        Monarch
    }

    public enum Team
    {
        Red,
        Blue
    }

    public class GameUnit : GameEntity
    {
        private readonly Team unitTeam;
        private readonly UnitClass unitJobClass;

        private readonly ITexture2D largePortrait;
        private readonly ITexture2D mediumPortrait;
        private readonly ITexture2D smallPortrait;

        private readonly UnitStatistics stats;

        public GameUnit(string id, Team unitTeam, UnitClass unitJobClass, ref MapEntity mapEntity, UnitStatistics stats,
            ITexture2D largePortrait, ITexture2D mediumPortrait, ITexture2D smallPortrait) : base(id, ref mapEntity)
        {
            this.unitTeam = unitTeam;
            this.unitJobClass = unitJobClass;
            this.stats = stats;
            this.largePortrait = largePortrait;
            this.mediumPortrait = mediumPortrait;
            this.smallPortrait = smallPortrait;
        }

        public UnitStatistics Stats
        {
            get { return stats; }
        }

        public Team UnitTeam
        {
            get { return unitTeam; }
        }

        public UnitClass UnitJobClass
        {
            get { return unitJobClass; }
        }

        public ITexture2D LargePortrait
        {
            get { return largePortrait; }
        }

        public ITexture2D MediumPortrait
        {
            get { return mediumPortrait; }
        }

        public ITexture2D SmallPortrait
        {
            get { return smallPortrait; }
        }
        
        public void MoveUnitInDirection(Direction direction, Vector2 mapSize)
        {
            switch (direction)
            {
                case Direction.Down:
                    MapEntity.MapCoordinates = new Vector2(MapEntity.MapCoordinates.X, MapEntity.MapCoordinates.Y + 1);
                    break;
                case Direction.Right:
                    MapEntity.MapCoordinates = new Vector2(MapEntity.MapCoordinates.X + 1, MapEntity.MapCoordinates.Y);
                    break;
                case Direction.Up:
                    MapEntity.MapCoordinates = new Vector2(MapEntity.MapCoordinates.X, MapEntity.MapCoordinates.Y - 1);
                    break;
                case Direction.Left:
                    MapEntity.MapCoordinates = new Vector2(MapEntity.MapCoordinates.X - 1, MapEntity.MapCoordinates.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            PreventCursorLeavingMapBounds(mapSize);
        }

        private void PreventCursorLeavingMapBounds(Vector2 mapSize)
        {
            if (MapEntity.MapCoordinates.X < 0)
            {
                MapEntity.MapCoordinates = new Vector2(0, MapEntity.MapCoordinates.Y);
            }

            if (MapEntity.MapCoordinates.X >= mapSize.X)
            {
                MapEntity.MapCoordinates = new Vector2(mapSize.X - 1, MapEntity.MapCoordinates.Y);
            }

            if (MapEntity.MapCoordinates.Y < 0)
            {
                MapEntity.MapCoordinates = new Vector2(MapEntity.MapCoordinates.X, 0);
            }

            if (MapEntity.MapCoordinates.Y >= mapSize.Y)
            {
                MapEntity.MapCoordinates = new Vector2(MapEntity.MapCoordinates.X, mapSize.Y - 1);
            }
        }
    }
}