using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Skills;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Health;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
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
        private readonly HealthBar resultsHealthBar;
        private readonly List<HealthBar> healthbars;

        private static readonly Color DeadPortraitColor = new Color(10, 10, 10, 180);

        private readonly UnitStatistics stats;
        public bool Enabled { get; private set; }

        public List<UnitSkill> Skills { get; private set; }
        private UnitSkill armedUnitSkill;

        public List<StatusEffect> StatusEffects { get; private set; }

        public GameUnit(string id, Team team, Role role, UnitEntity mapEntity, UnitStatistics stats,
            ITexture2D largePortrait, ITexture2D mediumPortrait, ITexture2D smallPortrait, List<UnitSkill> skills) :
            base(id, mapEntity)
        {
            this.team = team;
            this.role = role;
            this.stats = stats;
            Skills = skills;
            this.largePortrait =
                new SpriteAtlas(largePortrait, new Vector2(largePortrait.Width, largePortrait.Height), 1);
            this.mediumPortrait =
                new SpriteAtlas(mediumPortrait, new Vector2(mediumPortrait.Width, mediumPortrait.Height), 1);
            this.smallPortrait =
                new SpriteAtlas(smallPortrait, new Vector2(smallPortrait.Width, smallPortrait.Height), 1);
            initiativeHealthBar = new HealthBar(this.stats.MaxHp, this.stats.Hp, Vector2.One);
            combatHealthBar = new HealthBar(this.stats.MaxHp, this.stats.Hp, Vector2.One);
            hoverWindowHealthBar = new HealthBar(this.stats.MaxHp, this.stats.Hp, Vector2.One);
            resultsHealthBar = new HealthBar(this.stats.MaxHp, this.stats.Hp, Vector2.One);

            healthbars = new List<HealthBar>
            {
                initiativeHealthBar,
                combatHealthBar,
                hoverWindowHealthBar,
                resultsHealthBar
            };

            armedUnitSkill = skills.Find(skill => skill.GetType() == typeof(BasicAttack));

            StatusEffects = new List<StatusEffect>();
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

        public IRenderable GetResultsHealthBar(Vector2 barSize)
        {
            resultsHealthBar.SetSize(barSize);
            return resultsHealthBar;
        }

        public IRenderable DetailPane
        {
            get
            {
                return new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new RenderText(AssetManager.HeaderFont, Id),
                            new RenderBlank(),
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Hp),
                            new RenderText(AssetManager.WindowFont, "HP: "),
                            new RenderText(
                                AssetManager.WindowFont,
                                Stats.Hp.ToString(),
                                UnitStatistics.DetermineStatColor(Stats.Hp, Stats.MaxHp)
                            )
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Atk),
                            new RenderText(AssetManager.WindowFont, "ATK: "),
                            new RenderText(
                                AssetManager.WindowFont,
                                Stats.Atk.ToString(),
                                UnitStatistics.DetermineStatColor(Stats.Atk, Stats.BaseAtk)
                            )
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Def),
                            new RenderText(AssetManager.WindowFont, "DEF: "),
                            new RenderText(
                                AssetManager.WindowFont,
                                Stats.Def.ToString(),
                                UnitStatistics.DetermineStatColor(Stats.Def, Stats.BaseDef)
                            )
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Sp),
                            new RenderText(AssetManager.WindowFont, "SP: "),
                            new RenderText(
                                AssetManager.WindowFont,
                                Stats.Sp.ToString(),
                                UnitStatistics.DetermineStatColor(Stats.Sp, Stats.MaxSp)
                            )
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, "MV: "),
                            new RenderText(
                                AssetManager.WindowFont,
                                Stats.Mv.ToString(),
                                UnitStatistics.DetermineStatColor(Stats.Mv, Stats.BaseMv)
                            )
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.AtkRange),
                            new RenderText(AssetManager.WindowFont, "RNG:"),
                            new RenderText(
                                AssetManager.WindowFont,
                                string.Format("[{0}]", string.Join(",", Stats.AtkRange)),
                                UnitStatistics.DetermineStatColor(Stats.AtkRange.Max(), Stats.BaseAtkRange.Max())
                            )
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

        public void ArmUnitSkill(UnitSkill skill)
        {
            armedUnitSkill = skill;
        }

        public void ExecuteArmedSkill(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            armedUnitSkill.ExecuteAction(targetSlice, mapContext, battleContext);
        }

        public void CancelArmedSkill(MapContext mapContext)
        {
            armedUnitSkill.CancelAction(mapContext);
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
            SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
        }

        public void DisableExhaustedUnit()
        {
            if (UnitEntity == null) return;

            Enabled = false;
            UnitEntity.SetState(UnitEntity.UnitEntityState.Inactive);
            SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
            UpdateStatusEffects();
        }

        public void SetUnitAnimation(UnitSprite.UnitAnimationState state)
        {
            if (UnitEntity != null)
            {
                UnitEntity.UnitSprite.SetAnimation(state);
            }
        }

        public void AddStatusEffect(StatusEffect statusEffect)
        {
            StatusEffects.Add(statusEffect);
            statusEffect.ApplyEffect(this);
        }

        private void UpdateStatusEffects()
        {
            foreach (StatusEffect effect in StatusEffects)
            {
                effect.UpdateEffect(this);
            }

            StatusEffects.RemoveAll(effect => effect.TurnDuration < 1);
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
                AssetManager.CombatDeathSFX.Play();
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