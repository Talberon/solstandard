using System;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content.Health;
using SolStandard.Map.Elements;
using SolStandard.Utility;
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

        //TODO Decide how to handle this bar since any copies will not auto-update
        private readonly HealthBar defaultHealthBar;
        private const int InitiativeHpBarSize = 5;

        private readonly UnitStatistics stats;

        public GameUnit(string id, Team unitTeam, UnitClass unitJobClass, ref MapEntity mapEntity, UnitStatistics stats,
            ITexture2D largePortrait, ITexture2D mediumPortrait, ITexture2D smallPortrait) :
            base(id, ref mapEntity)
        {
            this.unitTeam = unitTeam;
            this.unitJobClass = unitJobClass;
            this.stats = stats;
            this.largePortrait = largePortrait;
            this.mediumPortrait = mediumPortrait;
            this.smallPortrait = smallPortrait;
            defaultHealthBar = new HealthBar(this.stats.MaxHp, this.stats.Hp, new Vector2(mediumPortrait.Width, InitiativeHpBarSize));
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

        public IRenderable DefaultHealthBar
        {
            get { return defaultHealthBar; }
        }

        public IRenderable GetCustomHealthBar(Vector2 barSize)
        {
            return new HealthBar(stats.MaxHp, stats.Hp, barSize);
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

        public void DamageUnit(int damage)
        {
            stats.Hp -= damage;
            defaultHealthBar.DealDamage(damage);
            KillIfDead();
        }

        private void KillIfDead()
        {
            if (stats.Hp <= 0)
            {
                MapEntity = null;
            }
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