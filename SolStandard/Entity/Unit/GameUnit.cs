using System;
using System.Collections.Generic;
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

        private readonly HealthBar hoverWindowHealthBar;
        private readonly HealthBar combatHealthBar;
        private readonly HealthBar initiativeHealthBar;
        private readonly List<HealthBar> healthbars;

        private readonly UnitStatistics stats;

        public GameUnit(string id, Team unitTeam, UnitClass unitJobClass, UnitEntity mapEntity, UnitStatistics stats,
            ITexture2D largePortrait, ITexture2D mediumPortrait, ITexture2D smallPortrait) : base(id, mapEntity)
        {
            this.unitTeam = unitTeam;
            this.unitJobClass = unitJobClass;
            this.stats = stats;
            this.largePortrait = largePortrait;
            this.mediumPortrait = mediumPortrait;
            this.smallPortrait = smallPortrait;
            initiativeHealthBar = new HealthBar(this.stats.MaxHp, this.stats.Hp, Vector2.One);
            combatHealthBar = new HealthBar(this.stats.MaxHp, this.stats.Hp, Vector2.One);
            hoverWindowHealthBar = new HealthBar(this.stats.MaxHp, this.stats.Hp, Vector2.One);

            healthbars = new List<HealthBar>
            {
                initiativeHealthBar,
                combatHealthBar,
                hoverWindowHealthBar
            };
        }

        public UnitEntity UnitEntity
        {
            get { return (UnitEntity) MapEntity; }
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

        public IRenderable GetInitiativeHealthBar(Vector2 barSize)
        {
            initiativeHealthBar.SetSize(barSize);
            return initiativeHealthBar;
        }

        public IRenderable GetHoverWindowHealthBar(Vector2 barSize)
        {
            hoverWindowHealthBar.SetSize(barSize);
            return hoverWindowHealthBar;
        }

        public IRenderable GetCombatHealthBar(Vector2 barSize)
        {
            combatHealthBar.SetSize(barSize);
            return combatHealthBar;
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

            PreventUnitLeavingMapBounds(mapSize);
        }

        public void DamageUnit(int damage)
        {
            stats.Hp -= damage;
            healthbars.ForEach(healthbar => healthbar.DealDamage(damage));
            KillIfDead();
        }

        private void KillIfDead()
        {
            if (stats.Hp <= 0)
            {
                MapEntity = null;
            }
        }

        public void SetUnitAnimation(UnitSprite.UnitAnimationState state)
        {
            if (UnitEntity != null)
            {
                UnitEntity.UnitSprite.SetAnimation(state);
            }
        }

        private void PreventUnitLeavingMapBounds(Vector2 mapSize)
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

        public override string ToString()
        {
            return "GameUnit: " + Id + ", " + UnitTeam + ", " + UnitJobClass;
        }
    }
}