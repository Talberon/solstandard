using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
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
using SolStandard.Utility.Events;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public enum Role
    {
        Silhouette,
        Champion,
        Marauder,
        Paladin,
        Bard,
        Cleric,
        Duelist,
        Lancer,
        Pugilist,
        Mage,
        Archer,
        Slime,
        Troll,
        Orc,
        Necromancer,
        Skeleton,
        Merchant,
        Goblin,
        Rat,
        Bat,
        Spider
    }

    public enum Team
    {
        Blue,
        Red,
        Creep
    }

    public class GameUnit : GameEntity
    {
        private readonly Team team;
        private readonly Role role;

        private readonly SpriteAtlas largePortrait;
        private readonly SpriteAtlas mediumPortrait;
        private readonly SpriteAtlas smallPortrait;

        private HealthBar hoverWindowHealthBar;
        private HealthBar combatHealthBar;
        private MiniHealthBar initiativeHealthBar;
        private MiniHealthBar resultsHealthBar;
        private List<IHealthBar> healthbars;

        public static readonly Color DeadPortraitColor = new Color(10, 10, 10, 180);
        public static readonly Color ExhaustedPortraitColor = new Color(150, 150, 150);
        public static readonly Color ActivePortraitColor = Color.White;

        private UnitStatistics stats;

        public bool IsExhausted { get; private set; }

        public List<UnitAction> Actions { get; private set; }
        public List<UnitAction> InventoryActions { get; private set; }
        public List<UnitAction> ContextualActions { get; private set; }
        private UnitAction armedUnitAction;

        public List<StatusEffect> StatusEffects { get; private set; }
        public bool IsCommander { get; set; }
        public bool IsMovable { get; set; }

        public List<IItem> Inventory { get; private set; }
        public int CurrentGold { get; set; }

        private readonly UnitSpriteSheet unitSpriteSheet;

        public GameUnit(string id, Team team, Role role, UnitEntity unitEntity, UnitStatistics stats,
            ITexture2D portrait, List<UnitAction> actions, bool isCommander) :
            base(id, unitEntity)
        {
            this.team = team;
            this.role = role;
            this.stats = stats;
            Actions = actions;
            IsCommander = isCommander;
            IsMovable = true;
            InventoryActions = new List<UnitAction>
            {
                new DropGiveGoldAction(),
                new Wait()
            };

            ContextualActions = new List<UnitAction>();
            largePortrait = new SpriteAtlas(portrait, new Vector2(portrait.Width, portrait.Height),
                new Vector2(256));
            mediumPortrait = new SpriteAtlas(portrait, new Vector2(portrait.Width, portrait.Height),
                new Vector2(128));
            smallPortrait = new SpriteAtlas(portrait, new Vector2(portrait.Width, portrait.Height),
                new Vector2(64));

            armedUnitAction = actions.Find(skill => skill.GetType() == typeof(BasicAttack));

            StatusEffects = new List<StatusEffect>();
            Inventory = new List<IItem>();
            CurrentGold = 0;

            unitSpriteSheet = GetSpriteSheetFromEntity(unitEntity);

            ApplyCommanderBonus();
            ResetHealthBars();
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


        public bool IsActive
        {
            get { return GameContext.InitiativeContext.CurrentActiveTeam == Team && !IsExhausted; }
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
            initiativeHealthBar.BarSize = barSize;
            return initiativeHealthBar;
        }

        public IRenderable GetHoverWindowHealthBar(Vector2 barSize)
        {
            hoverWindowHealthBar.BarSize = barSize;
            return hoverWindowHealthBar;
        }

        public IRenderable GetCombatHealthBar(Vector2 barSize)
        {
            combatHealthBar.BarSize = barSize;
            return combatHealthBar;
        }

        public IRenderable GetResultsHealthBar(Vector2 barSize)
        {
            resultsHealthBar.BarSize = barSize;
            return resultsHealthBar;
        }

        public IRenderable UnitPortraitPane
        {
            get
            {
                Color panelColor = new Color(10, 10, 10, 100);
                const int hoverWindowHealthBarHeight = 32;
                int windowBordersSize = AssetManager.WindowTexture.Width * 2 / 3;
                IRenderable[,] selectedUnitPortrait =
                {
                    {
                        new Window(
                            GetHoverWindowHealthBar(new Vector2(MediumPortrait.Width - windowBordersSize,
                                hoverWindowHealthBarHeight)),
                            panelColor
                        )
                    },
                    {
                        MediumPortrait
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
                Vector2 panelSizeOverride = new Vector2(180, 33);

                const int crownIconSize = 24;
                return new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new Window(
                                new WindowContentGrid(
                                    new[,]
                                    {
                                        {
                                            IsCommander
                                                ? GetCommanderCrown(new Vector2(crownIconSize))
                                                : new RenderBlank() as IRenderable,
                                            new RenderText(AssetManager.HeaderFont, Id)
                                        }
                                    },
                                    1
                                ),
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
                                            UnitStatistics.GetSpriteAtlas(Unit.Stats.Armor),
                                            new RenderText(AssetManager.WindowFont,
                                                UnitStatistics.Abbreviation[Unit.Stats.Armor] + ": "),
                                            new RenderText(
                                                AssetManager.WindowFont,
                                                Stats.CurrentArmor + "/" + Stats.MaxArmor
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
                                            UnitStatistics.GetSpriteAtlas(Unit.Stats.Hp),
                                            new RenderText(AssetManager.WindowFont,
                                                UnitStatistics.Abbreviation[Unit.Stats.Hp] + ": "),
                                            new RenderText(AssetManager.WindowFont, Stats.CurrentHP + "/" + Stats.MaxHP)
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
                                            UnitStatistics.GetSpriteAtlas(Unit.Stats.Atk),
                                            new RenderText(AssetManager.WindowFont,
                                                UnitStatistics.Abbreviation[Unit.Stats.Atk] + ": "),
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
                                            UnitStatistics.GetSpriteAtlas(Unit.Stats.Retribution),
                                            new RenderText(AssetManager.WindowFont,
                                                UnitStatistics.Abbreviation[Unit.Stats.Retribution] + ": "),
                                            new RenderText(
                                                AssetManager.WindowFont,
                                                Stats.Ret.ToString(),
                                                UnitStatistics.DetermineStatColor(Stats.Ret, Stats.BaseRet)
                                            )
                                        }
                                    },
                                    1
                                ),
                                statPanelColor,
                                panelSizeOverride
                            )
                        },
                        {
                            new Window(
                                new WindowContentGrid(
                                    new IRenderable[,]
                                    {
                                        {
                                            UnitStatistics.GetSpriteAtlas(Unit.Stats.Luck),
                                            new RenderText(AssetManager.WindowFont,
                                                UnitStatistics.Abbreviation[Unit.Stats.Luck] + ": "),
                                            new RenderText(
                                                AssetManager.WindowFont,
                                                Stats.Luck.ToString(),
                                                UnitStatistics.DetermineStatColor(Stats.Luck, Stats.BaseLuck)
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
                                            new SpriteAtlas(AssetManager.GoldIcon, new Vector2(GameDriver.CellSize)),
                                            new RenderText(AssetManager.WindowFont,
                                                "Gold: " + CurrentGold + Currency.CurrencyAbbreviation)
                                        }
                                    },
                                    1
                                ),
                                statPanelColor,
                                panelSizeOverride
                            )
                        },
                        {
                            new Window(
                                new WindowContentGrid(
                                    new IRenderable[,]
                                    {
                                        {
                                            UnitStatistics.GetSpriteAtlas(Unit.Stats.Mv),
                                            new RenderText(AssetManager.WindowFont,
                                                UnitStatistics.Abbreviation[Unit.Stats.Mv] + ": "),
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
                                            UnitStatistics.GetSpriteAtlas(Unit.Stats.AtkRange),
                                            new RenderText(AssetManager.WindowFont,
                                                UnitStatistics.Abbreviation[Unit.Stats.AtkRange] + ": "),
                                            new RenderText(
                                                AssetManager.WindowFont,
                                                string.Format("[{0}]", string.Join(",", Stats.CurrentAtkRange)),
                                                UnitStatistics.DetermineStatColor(Stats.CurrentAtkRange.Max(),
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


        public void AddContextualAction(UnitAction action)
        {
            ContextualActions.Add(action);
        }

        public IRenderable GetMapSprite(Vector2 size, UnitAnimationState animation = UnitAnimationState.Idle)
        {
            return GetMapSprite(size, Color.White, animation);
        }

        public IRenderable GetMapSprite(Vector2 size, Color color,
            UnitAnimationState animation = UnitAnimationState.Idle)
        {
            UnitSpriteSheet clonedSpriteSheet = unitSpriteSheet.Clone();
            clonedSpriteSheet.SetAnimation(animation);
            clonedSpriteSheet.DefaultColor = color;
            return clonedSpriteSheet.Resize(size);
        }

        public void ArmUnitSkill(UnitAction action)
        {
            armedUnitAction = action;
        }

        public void ExecuteArmedSkill(MapSlice targetSlice)
        {
            armedUnitAction.ExecuteAction(targetSlice);
        }

        public void CancelArmedSkill()
        {
            armedUnitAction.CancelAction();
        }

        public void MoveUnitInDirection(Direction direction)
        {
            Vector2 destination = UnitEntity.MapCoordinates;
            switch (direction)
            {
                case Direction.None:
                    SetUnitAnimation(UnitAnimationState.Idle);
                    break;
                case Direction.Down:
                    SetUnitAnimation(UnitAnimationState.WalkDown);
                    destination.Y = destination.Y + 1;
                    break;
                case Direction.Right:
                    SetUnitAnimation(UnitAnimationState.WalkRight);
                    destination.X = destination.X + 1;
                    break;
                case Direction.Up:
                    SetUnitAnimation(UnitAnimationState.WalkUp);
                    destination.Y = destination.Y - 1;
                    break;
                case Direction.Left:
                    SetUnitAnimation(UnitAnimationState.WalkLeft);
                    destination.X = destination.X - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            if (UnitMovingContext.CanEndMoveAtCoordinates(destination))
            {
                MoveUnitToCoordinates(destination);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        public void MoveUnitToCoordinates(Vector2 newCoordinates)
        {
            MapEntity.MapCoordinates = newCoordinates;
            PreventUnitLeavingMapBounds(MapContainer.MapGridSize);
        }

        private void ResetHealthBars()
        {
            combatHealthBar = new HealthBar(stats.MaxArmor, stats.MaxHP, Vector2.One);
            hoverWindowHealthBar = new HealthBar(stats.MaxArmor, stats.MaxHP, Vector2.One);
            initiativeHealthBar = new MiniHealthBar(stats.MaxArmor, stats.MaxHP, Vector2.One);
            resultsHealthBar = new MiniHealthBar(stats.MaxArmor, stats.MaxHP, Vector2.One);

            healthbars = new List<IHealthBar>
            {
                initiativeHealthBar,
                combatHealthBar,
                hoverWindowHealthBar,
                resultsHealthBar
            };
        }

        public void DamageUnit(bool ignoreArmor = false)
        {
            if (Stats.CurrentArmor > 0 && !ignoreArmor)
            {
                Stats.CurrentArmor--;
            }
            else
            {
                Stats.CurrentHP--;
            }

            healthbars.ForEach(healthbar => healthbar.SetArmorAndHp(Stats.CurrentArmor, Stats.CurrentHP));
            KillIfDead();
        }

        public void RecoverArmor(int amountToRecover)
        {
            if (amountToRecover + Stats.CurrentArmor > Stats.MaxArmor)
            {
                Stats.CurrentArmor = Stats.MaxArmor;
            }
            else
            {
                Stats.CurrentArmor += amountToRecover;
            }

            healthbars.ForEach(bar => bar.SetArmorAndHp(Stats.CurrentArmor, Stats.CurrentHP));
        }


        public void RecoverHP(int amountToRecover)
        {
            if (amountToRecover + Stats.CurrentHP > Stats.MaxHP)
            {
                Stats.CurrentHP = Stats.MaxHP;
            }
            else
            {
                Stats.CurrentHP += amountToRecover;
            }

            healthbars.ForEach(bar => bar.SetArmorAndHp(Stats.CurrentArmor, Stats.CurrentHP));
        }

        public void ActivateUnit()
        {
            if (UnitEntity == null) return;
            IsExhausted = false;
            UnitEntity.SetState(UnitEntity.UnitEntityState.Active);
            SetUnitAnimation(UnitAnimationState.Attack);
            UpdateStatusEffects();
            largePortrait.DefaultColor = ActivePortraitColor;
            mediumPortrait.DefaultColor = ActivePortraitColor;
            smallPortrait.DefaultColor = ActivePortraitColor;
        }

        public void DisableExhaustedUnit()
        {
            if (UnitEntity == null) return;
            IsExhausted = true;
            UnitEntity.SetState(UnitEntity.UnitEntityState.Exhausted);
            SetUnitAnimation(UnitAnimationState.Idle);
            largePortrait.DefaultColor = ExhaustedPortraitColor;
            mediumPortrait.DefaultColor = ExhaustedPortraitColor;
            smallPortrait.DefaultColor = ExhaustedPortraitColor;
        }

        public void EnableUnit()
        {
            if (UnitEntity == null || IsExhausted) return;

            UnitEntity.SetState(UnitEntity.UnitEntityState.Active);
            SetUnitAnimation(UnitAnimationState.Attack);
            largePortrait.DefaultColor = ActivePortraitColor;
            mediumPortrait.DefaultColor = ActivePortraitColor;
            smallPortrait.DefaultColor = ActivePortraitColor;
        }

        public void DisableInactiveUnit()
        {
            if (UnitEntity == null || IsExhausted) return;

            UnitEntity.SetState(UnitEntity.UnitEntityState.Inactive);
            SetUnitAnimation(UnitAnimationState.Idle);
        }

        public void SetUnitAnimation(UnitAnimationState state)
        {
            if (UnitEntity != null)
            {
                UnitEntity.UnitSpriteSheet.SetAnimation(state);
            }
        }

        public void AddStatusEffect(StatusEffect statusEffect)
        {
            //Do not allow stacking of same effect. Remove the existing one and reapply
            RemoveDuplicateEffects(statusEffect);

            StatusEffects.Add(statusEffect);
            statusEffect.ApplyEffect(this);
        }

        public void ApplyCommanderBonus()
        {
            if (!IsCommander) return;

            stats = stats.ApplyCommanderBonuses();
            ResetHealthBars();
        }

        private void RemoveDuplicateEffects(StatusEffect statusEffect)
        {
            foreach (StatusEffect effect in StatusEffects)
            {
                if (effect.GetType() == statusEffect.GetType())
                {
                    effect.RemoveEffect(this);
                }
            }

            StatusEffects.RemoveAll(status => status.GetType() == statusEffect.GetType());
        }

        private void UpdateStatusEffects()
        {
            Queue<IEvent> statusEffectEvents = new Queue<IEvent>();

            foreach (StatusEffect effect in StatusEffects)
            {
                statusEffectEvents.Enqueue(new CameraCursorPositionEvent(UnitEntity.MapCoordinates));
                statusEffectEvents.Enqueue(new UpdateStatusEffectEvent(effect, this));

                if (effect.HasNotification) statusEffectEvents.Enqueue(new WaitFramesEvent(100));
            }

            GlobalEventQueue.QueueEvents(statusEffectEvents);
        }

        public void AddItemToInventory(IItem item)
        {
            Inventory.Add(item);

            if (!item.IsBroken)
            {
                InventoryActions.Add(item.UseAction());
            }
            else
            {
                item.Icon.DefaultColor = DeadPortraitColor;
            }

            InventoryActions.Add(item.DropAction());
        }

        public bool RemoveItemFromInventory(IItem item)
        {
            if (Inventory.Contains(item))
            {
                InventoryActions.Remove(InventoryActions.Find(skill => skill.Name == item.UseAction().Name));
                InventoryActions.Remove(InventoryActions.Find(skill => skill.Name == item.DropAction().Name));
                Inventory.Remove(item);
                return true;
            }

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Cannot drop item here!", 100);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private void KillIfDead()
        {
            if (stats.CurrentHP <= 0 && MapEntity != null)
            {
                IsExhausted = true;
                DropSpoils();
                largePortrait.DefaultColor = DeadPortraitColor;
                mediumPortrait.DefaultColor = DeadPortraitColor;
                smallPortrait.DefaultColor = DeadPortraitColor;
                Trace.WriteLine("Unit " + Id + " is dead!");
                AssetManager.CombatDeathSFX.Play();
                MapEntity = null;
            }
        }

        public bool IsAlive
        {
            get { return stats.CurrentHP > 0; }
        }

        private void DropSpoils()
        {
            //Don't drop spoils if inventory is empty
            if (CurrentGold == 0 && Inventory.Count == 0) return;

            //If on top of other Spoils, pick those up before dropping on top of them
            Spoils spoilsAtUnitPosition =
                MapContainer.GameGrid[(int) Layer.Items][(int) MapEntity.MapCoordinates.X,
                    (int) MapEntity.MapCoordinates.Y] as Spoils;

            if (spoilsAtUnitPosition != null)
            {
                CurrentGold += spoilsAtUnitPosition.Gold;
                Inventory.AddRange(spoilsAtUnitPosition.Items);
            }


            TerrainEntity itemAtUnitPosition =
                MapContainer.GameGrid[(int) Layer.Items][(int) MapEntity.MapCoordinates.X,
                    (int) MapEntity.MapCoordinates.Y] as TerrainEntity;

            //Check if an item already exists here and add it to the spoils so that they aren't lost 
            if (itemAtUnitPosition != null)
            {
                if (itemAtUnitPosition is IItem)
                {
                    AddItemToInventory(itemAtUnitPosition as IItem);
                }
                else if (itemAtUnitPosition is Currency)
                {
                    Currency gold = itemAtUnitPosition as Currency;
                    CurrentGold += gold.Value;
                }
            }

            MapContainer.GameGrid[(int) Layer.Items][(int) MapEntity.MapCoordinates.X, (int) MapEntity.MapCoordinates.Y]
                = new Spoils(
                    Id + " Spoils",
                    "Spoils",
                    new SpriteAtlas(AssetManager.SpoilsIcon, new Vector2(GameDriver.CellSize)),
                    MapEntity.MapCoordinates,
                    CurrentGold,
                    new List<IItem>(Inventory)
                );

            CurrentGold = 0;
            Inventory.Clear();
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

        private UnitSpriteSheet GetSpriteSheetFromEntity(UnitEntity entity)
        {
            //TODO Find a cleaner way to test so that this isn't necessary

            if (entity != null) return entity.UnitSpriteSheet;

            Trace.TraceWarning("No unitEntity for unit " + this + "!");
            return new UnitSpriteSheet(new BlankTexture(), 1, Vector2.One, 100, false, Color.White);
        }

        public override string ToString()
        {
            return "GameUnit: " + Id + ", " + Team + ", " + Role;
        }

        public void ExecuteRoutines()
        {
            foreach (UnitAction action in Actions)
            {
                IRoutine routine = action as IRoutine;
                if (routine != null)
                {
                    routine.ExecuteAction(MapContainer.GetMapSliceAtCoordinates(UnitEntity.MapCoordinates));
                }
            }
        }

        public static SpriteAtlas GetCommanderCrown(Vector2 size)
        {
            return new SpriteAtlas(AssetManager.CommanderIcon, new Vector2(AssetManager.CommanderIcon.Height), size);
        }
    }
}