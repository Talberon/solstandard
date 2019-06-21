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

    public class GameUnit : GameEntity, IThreatRange
    {
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

        public bool IsExhausted { get; private set; }

        public List<UnitAction> Actions { get; }
        public List<UnitAction> ContextualActions { get; }
        private UnitAction armedUnitAction;

        public List<StatusEffect> StatusEffects { get; }
        public bool IsCommander { get; set; }
        public bool IsMovable { get; set; }

        public List<IItem> Inventory { get; }
        public int CurrentGold { get; set; }

        private readonly UnitSpriteSheet unitSpriteSheet;

        public GameUnit(string id, Team team, Role role, UnitEntity unitEntity, UnitStatistics stats,
            ITexture2D portrait, List<UnitAction> actions, bool isCommander) :
            base(id, unitEntity)
        {
            Team = team;
            Role = role;
            Stats = stats;
            Actions = actions;
            IsCommander = isCommander;
            IsMovable = true;

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

        public UnitEntity UnitEntity => (UnitEntity) MapEntity;

        public UnitStatistics Stats { get; private set; }

        public Team Team { get; }

        public Role Role { get; }


        public bool IsActive => GameContext.InitiativeContext.CurrentActiveTeam == Team && !IsExhausted;

        public IRenderable LargePortrait => largePortrait;

        public IRenderable MediumPortrait => mediumPortrait;

        public IRenderable SmallPortrait => smallPortrait;

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
                const float panelWidth = 410;
                const float panelHeight = 33;
                Vector2 twoColumnPanel = new Vector2(panelWidth / 2, panelHeight);
                Vector2 threeColumnPanel = new Vector2(panelWidth / 3 - 1, panelHeight);

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
                                statPanelColor,
                                twoColumnPanel
                            ),

                            new Window(
                                new RenderText(AssetManager.HeaderFont, Role.ToString()),
                                statPanelColor,
                                twoColumnPanel
                            ),
                            new RenderBlank()
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
                                statPanelColor,
                                threeColumnPanel
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
                                statPanelColor,
                                threeColumnPanel
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
                                threeColumnPanel
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
                                statPanelColor,
                                threeColumnPanel
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
                                threeColumnPanel
                            ),
                            new Window(
                                new WindowContentGrid(
                                    new IRenderable[,]
                                    {
                                        {
                                            UnitStatistics.GetSpriteAtlas(Unit.Stats.Block),
                                            new RenderText(AssetManager.WindowFont,
                                                UnitStatistics.Abbreviation[Unit.Stats.Block] + ": "),
                                            new RenderText(
                                                AssetManager.WindowFont,
                                                Stats.Blk.ToString(),
                                                UnitStatistics.DetermineStatColor(Stats.Blk, Stats.BaseBlk)
                                            )
                                        }
                                    },
                                    1
                                ),
                                statPanelColor,
                                threeColumnPanel
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
                                statPanelColor,
                                threeColumnPanel
                            ),
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
                                statPanelColor,
                                threeColumnPanel
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
                                                $"[{string.Join(",", Stats.CurrentAtkRange)}]",
                                                UnitStatistics.DetermineStatColor(Stats.CurrentAtkRange.Max(),
                                                    Stats.BaseAtkRange.Max())
                                            )
                                        }
                                    },
                                    1
                                ),
                                statPanelColor,
                                threeColumnPanel
                            )
                        }
                    },
                    2
                );
            }
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

        public void MoveUnitInDirection(Direction direction, bool ignoreCollision)
        {
            Vector2 destination = UnitEntity.MapCoordinates;
            switch (direction)
            {
                case Direction.None:
                    SetUnitAnimation(UnitAnimationState.Idle);
                    break;
                case Direction.Down:
                    SetUnitAnimation(UnitAnimationState.WalkDown);
                    destination.Y += 1;
                    break;
                case Direction.Right:
                    SetUnitAnimation(UnitAnimationState.WalkRight);
                    destination.X += 1;
                    break;
                case Direction.Up:
                    SetUnitAnimation(UnitAnimationState.WalkUp);
                    destination.Y -= 1;
                    break;
                case Direction.Left:
                    SetUnitAnimation(UnitAnimationState.WalkLeft);
                    destination.X -= 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            if (UnitMovingContext.CanEndMoveAtCoordinates(destination) || ignoreCollision)
            {
                MoveUnitToCoordinates(destination);
            }
        }

        public void MoveUnitToCoordinates(Vector2 newCoordinates)
        {
            MapEntity.SlideToCoordinates(newCoordinates);
            PreventUnitLeavingMapBounds(MapContainer.MapGridSize);
        }

        private void ResetHealthBars()
        {
            combatHealthBar = new HealthBar(Stats.MaxArmor, Stats.MaxHP, Vector2.One);
            hoverWindowHealthBar = new HealthBar(Stats.MaxArmor, Stats.MaxHP, Vector2.One);
            initiativeHealthBar = new MiniHealthBar(Stats.MaxArmor, Stats.MaxHP, Vector2.One);
            resultsHealthBar = new MiniHealthBar(Stats.MaxArmor, Stats.MaxHP, Vector2.One);

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

        public void ExhaustAndDisableUnit()
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
            UnitEntity?.UnitSpriteSheet.SetAnimation(state);
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

            Stats = Stats.ApplyCommanderBonuses();
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
                if (effect.HasNotification)
                {
                    statusEffectEvents.Enqueue(new CameraCursorPositionEvent(UnitEntity.MapCoordinates));
                }

                statusEffectEvents.Enqueue(new UpdateStatusEffectEvent(effect, this));

                if (effect.HasNotification) statusEffectEvents.Enqueue(new WaitFramesEvent(100));
            }

            GlobalEventQueue.QueueEvents(statusEffectEvents);
        }

        public void AddItemToInventory(IItem item)
        {
            Inventory.Add(item);

            if (item.IsBroken)
            {
                item.Icon.DefaultColor = DeadPortraitColor;
            }
        }

        public bool RemoveItemFromInventory(IItem item)
        {
            if (Inventory.Contains(item))
            {
                Inventory.Remove(item);
                return true;
            }

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Cannot drop item here!", 100);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private void KillIfDead()
        {
            if (Stats.CurrentHP <= 0 && MapEntity != null)
            {
                IsExhausted = true;
                DropSpoils();
                largePortrait.DefaultColor = DeadPortraitColor;
                mediumPortrait.DefaultColor = DeadPortraitColor;
                smallPortrait.DefaultColor = DeadPortraitColor;
                Trace.WriteLine("Unit " + Id + " is dead!");
                AssetManager.CombatDeathSFX.Play();
                MapEntity = null;
                GameMapContext.GameMapView.GenerateObjectiveWindow();
            }
        }

        public bool IsAlive => Stats.CurrentHP > 0 && MapEntity != null;

        public void Escape()
        {
            IsExhausted = true;
            DropSpoils();
            largePortrait.DefaultColor = DeadPortraitColor;
            mediumPortrait.DefaultColor = DeadPortraitColor;
            smallPortrait.DefaultColor = DeadPortraitColor;
            MapEntity = null;
        }

        private void DropSpoils()
        {
            //Don't drop spoils if inventory is empty
            if (CurrentGold == 0 && Inventory.Count == 0) return;

            //If on top of other Spoils, pick those up before dropping on top of them

            if (MapContainer.GameGrid[(int) Layer.Items][(int) MapEntity.MapCoordinates.X,
                (int) MapEntity.MapCoordinates.Y] is Spoils spoilsAtUnitPosition)
            {
                CurrentGold += spoilsAtUnitPosition.Gold;
                Inventory.AddRange(spoilsAtUnitPosition.Items);
            }


            //Check if an item already exists here and add it to the spoils so that they aren't lost 
            if (MapContainer.GameGrid[(int) Layer.Items][(int) MapEntity.MapCoordinates.X,
                (int) MapEntity.MapCoordinates.Y] is TerrainEntity itemAtUnitPosition)
            {
                switch (itemAtUnitPosition)
                {
                    case IItem item:
                        AddItemToInventory(item);
                        break;
                    case Currency currency:
                    {
                        Currency gold = currency;
                        CurrentGold += gold.Value;
                        break;
                    }
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
                MapEntity.SnapToCoordinates(new Vector2(0, MapEntity.MapCoordinates.Y));
            }

            if (MapEntity.MapCoordinates.X >= mapSize.X)
            {
                MapEntity.SnapToCoordinates(new Vector2(mapSize.X - 1, MapEntity.MapCoordinates.Y));
            }

            if (MapEntity.MapCoordinates.Y < 0)
            {
                MapEntity.SnapToCoordinates(new Vector2(MapEntity.MapCoordinates.X, 0));
            }

            if (MapEntity.MapCoordinates.Y >= mapSize.Y)
            {
                MapEntity.SnapToCoordinates(new Vector2(MapEntity.MapCoordinates.X, mapSize.Y - 1));
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

        public static SpriteAtlas GetCommanderCrown(Vector2 size)
        {
            return new SpriteAtlas(AssetManager.CommanderIcon, new Vector2(AssetManager.CommanderIcon.Height), size);
        }

        public int[] AtkRange => Stats.CurrentAtkRange;

        public int MvRange => Stats.Mv;
    }
}