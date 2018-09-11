using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Health;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public enum Role
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
        private readonly Team team;
        private readonly Role role;

        private readonly SpriteAtlas largePortrait;
        private readonly SpriteAtlas mediumPortrait;
        private readonly SpriteAtlas smallPortrait;

        private readonly HealthBar hoverWindowHealthBar;
        private readonly HealthBar combatHealthBar;
        private readonly HealthBar initiativeHealthBar;
        private readonly List<HealthBar> healthbars;

        private static readonly Color DeadPortraitColor = new Color(10, 10, 10, 180);

        private readonly UnitStatistics stats;
        public bool Enabled { get; private set; }

        public GameUnit(string id, Team team, Role role, UnitEntity mapEntity, UnitStatistics stats,
            ITexture2D largePortrait, ITexture2D mediumPortrait, ITexture2D smallPortrait) : base(id, mapEntity)
        {
            this.team = team;
            this.role = role;
            this.stats = stats;
            this.largePortrait =
                new SpriteAtlas(largePortrait, new Vector2(largePortrait.Width, largePortrait.Height), 1);
            this.mediumPortrait =
                new SpriteAtlas(mediumPortrait, new Vector2(mediumPortrait.Width, mediumPortrait.Height), 1);
            this.smallPortrait =
                new SpriteAtlas(smallPortrait, new Vector2(smallPortrait.Width, smallPortrait.Height), 1);
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

        public Team Team
        {
            get { return team; }
        }

        public Role Role
        {
            get { return role; }
        }

        public IRenderable LargePortrait
        {
            get { return largePortrait; }
        }

        public IRenderable MediumPortrait
        {
            get { return mediumPortrait; }
        }

        public IRenderable SmallPortrait
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
        
        public IRenderable DetailPane
        {
            get
            {
                return new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new RenderText(GameDriver.HeaderFont, Id),
                            new RenderBlank()
                        },
                        {
                            new SpriteAtlas(GameDriver.StatIcons, new Vector2(GameDriver.CellSize), 1),
                            new RenderText(GameDriver.WindowFont, "HP: " + Stats.Hp)
                        },
                        {
                            new SpriteAtlas(GameDriver.StatIcons, new Vector2(GameDriver.CellSize), 2),
                            new RenderText(GameDriver.WindowFont, "ATK: " + Stats.Atk)
                        },
                        {
                            new SpriteAtlas(GameDriver.StatIcons, new Vector2(GameDriver.CellSize), 3),
                            new RenderText(GameDriver.WindowFont, "DEF: " + Stats.Def)
                        },
                        {
                            new SpriteAtlas(GameDriver.StatIcons, new Vector2(GameDriver.CellSize), 4),
                            new RenderText(GameDriver.WindowFont, "SP: " + Stats.Sp)
                        },
                        {
                            new SpriteAtlas(GameDriver.StatIcons, new Vector2(GameDriver.CellSize), 5),
                            new RenderText(GameDriver.WindowFont, "MV: " + Stats.Mv)
                        },
                        {
                            new SpriteAtlas(GameDriver.StatIcons, new Vector2(GameDriver.CellSize), 6),
                            new RenderText(GameDriver.WindowFont,
                                string.Format("RNG: [{0}]", string.Join(",", Stats.AtkRange)))
                        }
                    },
                    2
                );
            }
        }

        public IRenderable GetMapSprite(Vector2 size)
        {
            if (UnitEntity == null)
            {
                return new RenderBlank();
            }

            AnimatedSprite mapSprite = UnitEntity.UnitSprite.Clone();
            mapSprite.Resize(size);
            return mapSprite;
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

        public void ActivateUnit()
        {
            if (UnitEntity == null) return;

            Enabled = true;
            UnitEntity.SetState(UnitEntity.UnitEntityState.Active);
        }

        public void DisableExhaustedUnit()
        {
            if (UnitEntity == null) return;

            Enabled = false;
            UnitEntity.SetState(UnitEntity.UnitEntityState.Inactive);
        }

        public void SetUnitAnimation(UnitSprite.UnitAnimationState state)
        {
            if (UnitEntity != null)
            {
                UnitEntity.UnitSprite.SetAnimation(state);
            }
        }

        private void KillIfDead()
        {
            if (stats.Hp <= 0)
            {
                MapEntity = null;
                largePortrait.RenderColor = DeadPortraitColor;
                mediumPortrait.RenderColor = DeadPortraitColor;
                smallPortrait.RenderColor = DeadPortraitColor;
                Trace.WriteLine("Unit " + Id + " is dead!");
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
            return "GameUnit: " + Id + ", " + Team + ", " + Role;
        }
    }
}