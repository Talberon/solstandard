using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Containers.Components.World.SubContext.Targeting;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Entity.Unit.Statuses.Bard;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Command;
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

        //Units
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
        Cavalier,
        Rogue,

        //Creeps
        Slime,
        Troll,
        Orc,
        Necromancer,
        Skeleton,
        Goblin,
        Rat,
        Bat,
        Spider,
        BloodOrc,
        Kobold,
        Dragon,

        //Pets
        Boar,
    }

    public enum Team
    {
        Red,
        Blue,
        Creep
    }

    public class GameUnit : GameEntity, IThreatRange
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly SpriteAtlas largePortrait;
        private readonly SpriteAtlas mediumPortrait;
        private readonly SpriteAtlas smallPortrait;

        private HealthBar hoverWindowResourceBar;
        private HealthBar combatResourceBar;
        private MiniHealthBar initiativeHealthBar;
        private MiniHealthBar resultsHealthBar;
        private List<IHealthBar> healthbars;
        private readonly MiniCommandPointBar miniCommandPointBar;

        public static readonly Color DeadPortraitColor = new Color(10, 10, 10, 180);
        public static readonly Color ExhaustedPortraitColor = new Color(150, 150, 150);
        public static readonly Color ActivePortraitColor = Color.White;

        public bool IsExhausted { get; private set; }

        public List<UnitAction> Actions { get; set; }
        public List<UnitAction> ContextualActions { get; }
        private UnitAction armedUnitAction;
        private readonly UnitSpriteSheet entitySpriteSheet;

        public List<StatusEffect> StatusEffects { get; }

        public bool IsCommander { get; set; }
        public bool IsMovable { get; set; }

        public List<IItem> Inventory { get; }
        public int CurrentBounty { get; set; }

        private TriggeredAnimation deathAnimation;

        public UnitEntity UnitEntity => (UnitEntity) MapEntity;
        public UnitStatistics Stats { get; }
        public Team Team { get; }
        public Role Role { get; }
        public bool IsActive => GlobalContext.InitiativePhase.CurrentActiveTeam == Team && !IsExhausted;
        public IRenderable LargePortrait => largePortrait;
        public IRenderable MediumPortrait => mediumPortrait;
        public IRenderable SmallPortrait => smallPortrait;

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

            entitySpriteSheet = unitEntity.UnitSpriteSheet;

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
            CurrentBounty = 0;

            deathAnimation = null;

            ResetHealthBars();
            miniCommandPointBar = new MiniCommandPointBar(Stats.MaxCmd, Vector2.One);
        }

        public int[] AtkRange => Stats.CurrentAtkRange;
        public int MvRange => Stats.Mv;

        public IRenderable GetInitiativeCommandPointBar(Vector2 barSize)
        {
            miniCommandPointBar.BarSize = barSize;
            return miniCommandPointBar;
        }

        public IRenderable GetInitiativeHealthBar(Vector2 barSize)
        {
            initiativeHealthBar.BarSize = barSize;
            return initiativeHealthBar;
        }

        public IRenderable GetHoverWindowHealthBar(Vector2 barSize)
        {
            hoverWindowResourceBar.BarSize = barSize;
            return hoverWindowResourceBar;
        }

        public IRenderable GetCombatHealthBar(Vector2 barSize)
        {
            combatResourceBar.BarSize = barSize;
            return combatResourceBar;
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
                var panelColor = new Color(10, 10, 10, 100);
                const int hoverWindowHealthBarHeight = 32;
                int windowBordersSize = AssetManager.WindowTexture.Width * 2 / 3;
                IRenderable[,] selectedUnitPortrait =
                {
                    {
                        new Window(
                            new WindowContentGrid(
                                new[,]
                                {
                                    {
                                        (IsCommander)
                                            ? new CommandPointBar(
                                                Stats.MaxCmd,
                                                Stats.CurrentCmd,
                                                new Vector2(MediumPortrait.Width - windowBordersSize,
                                                    (float) hoverWindowHealthBarHeight / 2)
                                            )
                                            : RenderBlank.Blank
                                    },
                                    {
                                        GetHoverWindowHealthBar(
                                            new Vector2(MediumPortrait.Width - windowBordersSize,
                                                hoverWindowHealthBarHeight)
                                        )
                                    }
                                },
                                0,
                                HorizontalAlignment.Centered
                            ),
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
                if (Inventory.Count <= 0) return null;

                var content = new IRenderable[1, Inventory.Count + 1];

                content[0, 0] = new Window(
                    new RenderText(AssetManager.StatFont, " ITEMS"),
                    WorldHUD.BlankTerrainWindowColor
                );

                for (int i = 0; i < Inventory.Count; i++)
                {
                    content[0, i + 1] = Inventory[i].Icon.Clone();
                }

                return new WindowContentGrid(content, 2);
            }
        }

        public IRenderable DetailPane
        {
            get
            {
                ISpriteFont statfont = AssetManager.StatFont;
                var statPanelColor = new Color(10, 10, 10, 100);
                const float panelWidth = 410;
                const float panelHeight = 33;
                var twoColumnPanel = new Vector2(panelWidth / 2, panelHeight);
                var threeColumnPanel = new Vector2(panelWidth / 3 - 1, panelHeight);

                const int crownIconSize = 24;
                return new WindowContentGrid(
                    new[,]
                    {
                        {
                            new Window(
                                new[,]
                                {
                                    {
                                        IsCommander
                                            ? MiscIconProvider.GetMiscIcon(MiscIcon.Crown,
                                                new Vector2(crownIconSize))
                                            : RenderBlank.Blank,
                                        new RenderText(AssetManager.HeaderFont, Id)
                                    }
                                },
                                statPanelColor,
                                twoColumnPanel,
                                HorizontalAlignment.Centered
                            ),

                            new Window(
                                new RenderText(AssetManager.HeaderFont, Role.ToString()),
                                statPanelColor,
                                twoColumnPanel
                            ),
                            RenderBlank.Blank
                        },
                        {
                            new Window(
                                new IRenderable[,]
                                {
                                    {
                                        UnitStatistics.GetSpriteAtlas(Unit.Stats.Armor),
                                        new RenderText(statfont,
                                            UnitStatistics.Abbreviation[Unit.Stats.Armor] + ": "),
                                        new RenderText(statfont, Stats.CurrentArmor + "/" + Stats.MaxArmor)
                                    }
                                },
                                statPanelColor,
                                threeColumnPanel,
                                HorizontalAlignment.Centered
                            ),
                            new Window(
                                new IRenderable[,]
                                {
                                    {
                                        UnitStatistics.GetSpriteAtlas(Unit.Stats.Hp),
                                        new RenderText(statfont,
                                            UnitStatistics.Abbreviation[Unit.Stats.Hp] + ": "),
                                        new RenderText(statfont, Stats.CurrentHP + "/" + Stats.MaxHP)
                                    }
                                },
                                statPanelColor,
                                threeColumnPanel,
                                HorizontalAlignment.Centered
                            ),
                            new Window(
                                new IRenderable[,]
                                {
                                    {
                                        MiscIconProvider.GetMiscIcon(MiscIcon.Gold, GameDriver.CellSizeVector),
                                        new RenderText(statfont,
                                            "Bounty: " + CurrentBounty + Currency.CurrencyAbbreviation)
                                    }
                                },
                                statPanelColor,
                                threeColumnPanel,
                                HorizontalAlignment.Centered
                            )
                        },
                        {
                            new Window(
                                new IRenderable[,]
                                {
                                    {
                                        UnitStatistics.GetSpriteAtlas(Unit.Stats.Atk),
                                        new RenderText(statfont,
                                            UnitStatistics.Abbreviation[Unit.Stats.Atk] + ": "),
                                        new RenderText(
                                            statfont,
                                            Stats.Atk.ToString(),
                                            UnitStatistics.DetermineStatColor(Stats.Atk, Stats.BaseAtk)
                                        )
                                    }
                                },
                                statPanelColor,
                                threeColumnPanel,
                                HorizontalAlignment.Centered
                            ),
                            new Window(
                                new IRenderable[,]
                                {
                                    {
                                        UnitStatistics.GetSpriteAtlas(Unit.Stats.Retribution),
                                        new RenderText(statfont,
                                            UnitStatistics.Abbreviation[Unit.Stats.Retribution] + ": "),
                                        new RenderText(
                                            statfont,
                                            Stats.Ret.ToString(),
                                            UnitStatistics.DetermineStatColor(Stats.Ret, Stats.BaseRet)
                                        )
                                    }
                                },
                                statPanelColor,
                                threeColumnPanel,
                                HorizontalAlignment.Centered
                            ),
                            new Window(
                                new IRenderable[,]
                                {
                                    {
                                        UnitStatistics.GetSpriteAtlas(Unit.Stats.Block),
                                        new RenderText(statfont,
                                            UnitStatistics.Abbreviation[Unit.Stats.Block] + ": "),
                                        new RenderText(
                                            statfont,
                                            Stats.Blk.ToString(),
                                            UnitStatistics.DetermineStatColor(Stats.Blk, Stats.BaseBlk)
                                        )
                                    }
                                },
                                statPanelColor,
                                threeColumnPanel,
                                HorizontalAlignment.Centered
                            )
                        },
                        {
                            new Window(
                                new IRenderable[,]
                                {
                                    {
                                        UnitStatistics.GetSpriteAtlas(Unit.Stats.Luck),
                                        new RenderText(statfont,
                                            UnitStatistics.Abbreviation[Unit.Stats.Luck] + ": "),
                                        new RenderText(
                                            statfont,
                                            Stats.Luck.ToString(),
                                            UnitStatistics.DetermineStatColor(Stats.Luck, Stats.BaseLuck)
                                        )
                                    }
                                },
                                statPanelColor,
                                threeColumnPanel,
                                HorizontalAlignment.Centered
                            ),
                            new Window(
                                new IRenderable[,]
                                {
                                    {
                                        UnitStatistics.GetSpriteAtlas(Unit.Stats.Mv),
                                        new RenderText(statfont,
                                            UnitStatistics.Abbreviation[Unit.Stats.Mv] + ": "),
                                        new RenderText(
                                            statfont,
                                            Stats.Mv.ToString(),
                                            UnitStatistics.DetermineStatColor(Stats.Mv, Stats.BaseMv)
                                        )
                                    }
                                },
                                statPanelColor,
                                threeColumnPanel,
                                HorizontalAlignment.Centered
                            ),
                            new Window(
                                new IRenderable[,]
                                {
                                    {
                                        UnitStatistics.GetSpriteAtlas(Unit.Stats.AtkRange),
                                        new RenderText(statfont,
                                            UnitStatistics.Abbreviation[Unit.Stats.AtkRange] + ": "),
                                        new RenderText(
                                            statfont,
                                            $"[{string.Join(",", Stats.CurrentAtkRange)}]",
                                            UnitStatistics.DetermineStatColor(Stats.CurrentAtkRange.Max(),
                                                Stats.BaseAtkRange.Max())
                                        )
                                    }
                                },
                                statPanelColor,
                                threeColumnPanel,
                                HorizontalAlignment.Centered
                            )
                        }
                    },
                    2
                );
            }
        }

        public IRenderable GetMapSprite(Vector2 size, Color color, UnitAnimationState animation, int frameDelay,
            bool isFlipped)
        {
            UnitSpriteSheet clonedSpriteSheet = entitySpriteSheet.Clone();
            if (isFlipped) clonedSpriteSheet.Flip();
            clonedSpriteSheet.SetFrameDelay(frameDelay);
            clonedSpriteSheet.SetAnimation(animation);
            clonedSpriteSheet.DefaultColor = color;

            if (IsAlive) return clonedSpriteSheet.Resize(size);
            if (Stats.CurrentHP != 0) return deathAnimation ?? RenderBlank.Blank;
            deathAnimation = AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.Death, size);
            deathAnimation.PlayOnce();
            return deathAnimation;
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

            if (UnitMovingPhase.CanEndMoveAtCoordinates(destination) || ignoreCollision)
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
            combatResourceBar = new HealthBar(Stats.MaxArmor, Stats.MaxHP, Vector2.One);
            hoverWindowResourceBar = new HealthBar(Stats.MaxArmor, Stats.MaxHP, Vector2.One);
            initiativeHealthBar = new MiniHealthBar(Stats.MaxArmor, Stats.MaxHP, Vector2.One);
            resultsHealthBar = new MiniHealthBar(Stats.MaxArmor, Stats.MaxHP, Vector2.One);

            healthbars = new List<IHealthBar>
            {
                initiativeHealthBar,
                combatResourceBar,
                hoverWindowResourceBar,
                resultsHealthBar
            };
        }

        public void KillUnit()
        {
            Stats.CurrentArmor = 0;
            Stats.CurrentHP = 0;
            healthbars.ForEach(healthbar => healthbar.SetArmorAndHp(Stats.CurrentArmor, Stats.CurrentHP));
            KillIfDead(false);
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

            if (UnitEntity == null) return;
            GlobalContext.WorldContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.Damage, GameDriver.CellSizeVector),
                UnitEntity.MapCoordinates
            );
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

            if (UnitEntity == null) return;
            GlobalContext.WorldContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.RecoverArmor, GameDriver.CellSizeVector),
                UnitEntity.MapCoordinates
            );
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

            if (UnitEntity == null) return;
            GlobalContext.WorldContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.RecoverHealth, GameDriver.CellSizeVector),
                UnitEntity.MapCoordinates
            );
        }

        public void AddCommandPoints(int amountToAdd = 1)
        {
            if (Stats.CurrentCmd < Stats.MaxCmd) Stats.CurrentCmd += amountToAdd;
            miniCommandPointBar.UpdateCommandPoints(Stats.CurrentCmd);
        }

        public void RemoveCommandPoints(int pointsToRemove)
        {
            Stats.CurrentCmd -= pointsToRemove;
            if (Stats.CurrentCmd < 0) Stats.CurrentCmd = 0;
            miniCommandPointBar.UpdateCommandPoints(Stats.CurrentCmd);
        }

        public void ActivateUnit()
        {
            if (UnitEntity == null) return;
            IsExhausted = false;
            UnitEntity.SetState(UnitEntity.UnitEntityState.Active);
            SetUnitAnimation(UnitAnimationState.Active);
            UpdateStatusEffects();
            largePortrait.DefaultColor = ActivePortraitColor;
            mediumPortrait.DefaultColor = ActivePortraitColor;
            smallPortrait.DefaultColor = ActivePortraitColor;
            if (IsCommander) AddCommandPoints();
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
            SetUnitAnimation(UnitAnimationState.Active);
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
            var statusEffectEvents = new Queue<IEvent>();

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

            if (item.IsBroken) item.Icon.DefaultColor = DeadPortraitColor;

            if (UnitEntity != null)
            {
                UnitEntity.HasItemsInInventory = Inventory.Count != 0;
            }
        }

        public bool RemoveItemFromInventory(IItem item)
        {
            if (Inventory.Contains(item))
            {
                Inventory.Remove(item);
                
                if (UnitEntity != null)
                {
                    UnitEntity.HasItemsInInventory = Inventory.Count != 0;
                }
                
                return true;
            }
            
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Cannot drop item here!", 100);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private void KillIfDead(bool playSfx = true)
        {
            if (Stats.CurrentHP > 0 || MapEntity == null) return;

            IsExhausted = true;
            DropSpoils();
            largePortrait.DefaultColor = DeadPortraitColor;
            mediumPortrait.DefaultColor = DeadPortraitColor;
            smallPortrait.DefaultColor = DeadPortraitColor;
            Logger.Debug("Unit " + Id + " is dead!");
            if (playSfx) AssetManager.CombatDeathSFX.Play();
            GlobalContext.WorldContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.Death, GameDriver.CellSizeVector),
                MapEntity.MapCoordinates
            );
            MapEntity = null;
            WorldContext.WorldHUD.GenerateObjectiveWindow();
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
            if (CurrentBounty == 0 && Inventory.Count == 0) return;

            MapElement itemAtUnitPosition = MapContainer.GameGrid[(int) Layer.Items][(int) MapEntity.MapCoordinates.X,
                (int) MapEntity.MapCoordinates.Y];

            //If on top of other Spoils, pick those up before dropping on top of them
            if (itemAtUnitPosition is Spoils spoilsAtUnitPosition)
            {
                CurrentBounty += spoilsAtUnitPosition.Gold;
                Inventory.AddRange(spoilsAtUnitPosition.Items);
            }

            //Check if an item already exists here and add it to the spoils so that they aren't lost 
            if (itemAtUnitPosition is TerrainEntity entityAtUnitPosition)
            {
                switch (entityAtUnitPosition)
                {
                    case IItem item:
                        AddItemToInventory(item);
                        break;
                    case Currency currency:
                    {
                        Currency gold = currency;
                        CurrentBounty += gold.Value;
                        break;
                    }
                }
            }

            MapContainer.GameGrid[(int) Layer.Items][(int) MapEntity.MapCoordinates.X, (int) MapEntity.MapCoordinates.Y]
                = new Spoils(
                    Id + " Spoils",
                    "Spoils",
                    MiscIconProvider.GetMiscIcon(MiscIcon.Spoils, GameDriver.CellSizeVector),
                    MapEntity.MapCoordinates,
                    CurrentBounty,
                    new List<IItem>(Inventory)
                );

            GlobalContext.InitiativePhase.DeductGoldFromTeam(CurrentBounty, Team);
            CurrentBounty = 0;
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

        public override string ToString()
        {
            return "GameUnit: " + Id + ", " + Team + ", " + Role;
        }

        public bool IsDifferentFrom(int hashCode)
        {
            return GetHashCode() != hashCode;
        }

        // ReSharper disable NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            const int serialId = 123014;
            int hashCode = Stats.GetHashCode();
            hashCode += Id.Length;
            hashCode += Inventory.Count;
            hashCode += StatusEffects.Count;
            hashCode += (int) Team;
            hashCode += (int) Role;
            hashCode += CurrentBounty;
            hashCode *= serialId;
            return hashCode;
        }

        public void DrawAuras(SpriteBatch spriteBatch)
        {
            if (!IsAlive) return;

            List<SongStatus> songs = StatusEffects.Where(status => status is SongStatus).Cast<SongStatus>().ToList();

            foreach (SongStatus song in songs)
            {
                var targetingContext = new UnitTargetingPhase(song.SongSprite);
                List<MapDistanceTile> auraTiles =
                    targetingContext.GetTargetingTiles(UnitEntity.MapCoordinates, song.AuraRange);

                foreach (MapDistanceTile tile in auraTiles)
                {
                    tile.Draw(spriteBatch);
                }
            }
        }
    }
}