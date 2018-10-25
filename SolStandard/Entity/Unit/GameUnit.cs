using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Health;
using SolStandard.Map;
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

        public List<UnitAction> Skills { get; private set; }
        private UnitAction armedUnitAction;

        public List<StatusEffect> StatusEffects { get; private set; }

        public List<IItem> Inventory { get; private set; }
        public int CurrentGold { get; set; }

        public GameUnit(string id, Team team, Role role, UnitEntity mapEntity, UnitStatistics stats,
            ITexture2D largePortrait, ITexture2D mediumPortrait, ITexture2D smallPortrait, List<UnitAction> skills) :
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

            armedUnitAction = skills.Find(skill => skill.GetType() == typeof(BasicAttack));

            StatusEffects = new List<StatusEffect>();
            Inventory = new List<IItem>();
            CurrentGold = 0;
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

        public IRenderable UnitPortraitPane
        {
            get
            {
                const int hoverWindowHealthBarHeight = 15;
                IRenderable[,] selectedUnitPortrait =
                {
                    {
                        MediumPortrait
                    },
                    {
                        GetHoverWindowHealthBar(new Vector2(MediumPortrait.Width, hoverWindowHealthBarHeight))
                    }
                };

                return new WindowContentGrid(selectedUnitPortrait, 2);
            }
        }

        public IRenderable InventoryPane
        {
            get
            {
                if (Inventory.Count > 0)
                {
                    const int offset = 1;
                    IRenderable[,] content = new IRenderable[Inventory.Count + offset, 2];

                    content[0, 0] = new RenderBlank();
                    content[0, 1] = new RenderText(AssetManager.HeaderFont, "Inventory");

                    for (int i = 0; i < Inventory.Count; i++)
                    {
                        content[i + offset, 0] = Inventory[i].Icon;
                        content[i + offset, 1] = new RenderText(AssetManager.WindowFont, Inventory[i].Name);
                    }

                    return new WindowContentGrid(content, 2);
                }

                return null;
            }
        }

        public IRenderable DetailPane
        {
            get
            {
                Color statPanelColor = new Color(10, 10, 10, 100);
                Vector2 panelSizeOverride = new Vector2(180, 35);

                return new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new Window(
                                new RenderText(AssetManager.HeaderFont, Id),
                                statPanelColor, panelSizeOverride),

                            new Window(
                                new RenderText(AssetManager.HeaderFont, Role.ToString()),
                                statPanelColor, panelSizeOverride),
                        },
                        {
                            new Window(
                                new WindowContentGrid(
                                    new IRenderable[,]
                                    {
                                        {
                                            UnitStatistics.GetSpriteAtlas(StatIcons.Hp),
                                            new RenderText(AssetManager.WindowFont, "HP: "),
                                            new RenderText(AssetManager.WindowFont,
                                                Stats.Hp.ToString() + "/" + Stats.MaxHp.ToString())
                                        }
                                    },
                                    1
                                ),
                                statPanelColor, panelSizeOverride
                            ),

                            new Window(
                                new WindowContentGrid(
                                    new IRenderable[,]
                                    {
                                        {
                                            new SpriteAtlas(AssetManager.GoldIcon, new Vector2(GameDriver.CellSize), 1),
                                            new RenderText(AssetManager.WindowFont,
                                                "Gold: " + CurrentGold + Currency.CurrencyAbbreviation)
                                        }
                                    },
                                    1
                                ),
                                statPanelColor, panelSizeOverride
                            )
                        },
                        {
                            new Window(
                                new WindowContentGrid(
                                    new IRenderable[,]
                                    {
                                        {
                                            UnitStatistics.GetSpriteAtlas(StatIcons.Atk),
                                            new RenderText(AssetManager.WindowFont, "ATK: "),
                                            new RenderText(
                                                AssetManager.WindowFont,
                                                Stats.Atk.ToString(),
                                                UnitStatistics.DetermineStatColor(Stats.Atk, Stats.BaseAtk)
                                            )
                                        }
                                    },
                                    1
                                ),
                                statPanelColor, panelSizeOverride
                            ),
                            new Window(
                                new WindowContentGrid(
                                    new IRenderable[,]
                                    {
                                        {
                                            UnitStatistics.GetSpriteAtlas(StatIcons.Def),
                                            new RenderText(AssetManager.WindowFont, "DEF: "),
                                            new RenderText(
                                                AssetManager.WindowFont,
                                                Stats.Def.ToString(),
                                                UnitStatistics.DetermineStatColor(Stats.Def, Stats.BaseDef)
                                            )
                                        }
                                    },
                                    1
                                ),
                                statPanelColor, panelSizeOverride
                            )
                        },
                        {
                            new Window(
                                new WindowContentGrid(
                                    new IRenderable[,]
                                    {
                                        {
                                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                                            new RenderText(AssetManager.WindowFont, "MV: "),
                                            new RenderText(
                                                AssetManager.WindowFont,
                                                Stats.Mv.ToString(),
                                                UnitStatistics.DetermineStatColor(Stats.Mv, Stats.BaseMv)
                                            )
                                        }
                                    },
                                    1
                                ),
                                statPanelColor, panelSizeOverride
                            ),
                            new Window(
                                new WindowContentGrid(
                                    new IRenderable[,]
                                    {
                                        {
                                            UnitStatistics.GetSpriteAtlas(StatIcons.AtkRange),
                                            new RenderText(AssetManager.WindowFont, "RNG:"),
                                            new RenderText(
                                                AssetManager.WindowFont,
                                                string.Format("[{0}]", string.Join(",", Stats.AtkRange)),
                                                UnitStatistics.DetermineStatColor(Stats.AtkRange.Max(),
                                                    Stats.BaseAtkRange.Max())
                                            )
                                        }
                                    },
                                    1
                                ),
                                statPanelColor, panelSizeOverride
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

        public void ArmUnitSkill(UnitAction action)
        {
            armedUnitAction = action;
        }

        public void ExecuteArmedSkill(MapSlice targetSlice, GameMapContext gameMapContext, BattleContext battleContext)
        {
            armedUnitAction.ExecuteAction(targetSlice, gameMapContext, battleContext);
        }

        public void CancelArmedSkill(GameMapContext gameMapContext)
        {
            armedUnitAction.CancelAction(gameMapContext);
        }

        public void MoveUnitInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    MoveUnitToCoordinates(new Vector2(MapEntity.MapCoordinates.X, MapEntity.MapCoordinates.Y + 1));
                    break;
                case Direction.Right:
                    MoveUnitToCoordinates(new Vector2(MapEntity.MapCoordinates.X + 1, MapEntity.MapCoordinates.Y));
                    break;
                case Direction.Up:
                    MoveUnitToCoordinates(new Vector2(MapEntity.MapCoordinates.X, MapEntity.MapCoordinates.Y - 1));
                    break;
                case Direction.Left:
                    MoveUnitToCoordinates(new Vector2(MapEntity.MapCoordinates.X - 1, MapEntity.MapCoordinates.Y));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        public void MoveUnitToCoordinates(Vector2 newCoordinates)
        {
            MapEntity.MapCoordinates = newCoordinates;
            PreventUnitLeavingMapBounds(MapContainer.MapGridSize);
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

        public void AddItemToInventory(IItem item)
        {
            Inventory.Add(item);
            Skills.Insert(1, item.DropAction());
            Skills.Insert(1, item.UseAction());
        }

        public bool RemoveItemFromInventory(IItem item)
        {
            if (Inventory.Contains(item))
            {
                Skills.Remove(Skills.Find(skill => skill.Name == item.UseAction().Name));
                Skills.Remove(Skills.Find(skill => skill.Name == item.DropAction().Name));
                Inventory.Remove(item);
                return true;
            }

            MapContainer.AddNewToastAtMapCursor("Cannot drop item here!", 100);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private void KillIfDead()
        {
            if (stats.Hp <= 0 && MapEntity != null)
            {
                DropSpoils();
                largePortrait.RenderColor = DeadPortraitColor;
                mediumPortrait.RenderColor = DeadPortraitColor;
                smallPortrait.RenderColor = DeadPortraitColor;
                Trace.WriteLine("Unit " + Id + " is dead!");
                AssetManager.CombatDeathSFX.Play();
                MapEntity = null;
            }
        }

        private void DropSpoils()
        {
            TerrainEntity entityAtUnitPosition =
                MapContainer.GameGrid[(int) Layer.Items][(int) MapEntity.MapCoordinates.X,
                    (int) MapEntity.MapCoordinates.Y] as TerrainEntity;

            //Check if an item already exists here and add it to the spoils so that they aren't lost 
            if (entityAtUnitPosition != null)
            {
                if (entityAtUnitPosition is IItem)
                {
                    AddItemToInventory(entityAtUnitPosition as IItem);
                }
                else if (entityAtUnitPosition is Currency)
                {
                    Currency gold = entityAtUnitPosition as Currency;
                    CurrentGold += gold.Value;
                }
            }

            MapContainer.GameGrid[(int) Layer.Items][(int) MapEntity.MapCoordinates.X, (int) MapEntity.MapCoordinates.Y]
                = new Spoils(
                    Id + " Spoils",
                    "Spoils",
                    new SpriteAtlas(AssetManager.SpoilsIcon, new Vector2(GameDriver.CellSize), 1),
                    MapEntity.MapCoordinates,
                    new Dictionary<string, string>(),
                    CurrentGold,
                    Inventory
                );
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