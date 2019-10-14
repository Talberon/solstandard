﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts.Combat;
using SolStandard.Containers.View;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Entity.Unit.Statuses.Bard;
using SolStandard.HUD.Window.Animation;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Combat;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;

namespace SolStandard.Containers.Contexts
{
    public class BattleContext
    {
        public enum BattleState
        {
            Start,
            RollDice,
            ResolveCombat
        }

        private const int BountyPerTriumph = 10;

        private readonly BattleView battleView;

        private CombatDamage attackerDamage;
        private CombatDamage defenderDamage;

        private int frameCounter;
        private bool currentlyRolling;
        private int rollingCounter;
        private bool currentlyResolvingBlocks;
        private bool currentlyResolvingDamage;

        public BattleState CurrentState { get; private set; }

        private GameUnit attacker;
        private GameUnit defender;
        private Vector2 attackerCoordinates;
        private Vector2 defenderCoordinates;
        private UnitStatistics attackerStats;
        private UnitStatistics defenderStats;

        private List<ICombatProc> attackerProcs;
        private List<ICombatProc> defenderProcs;

        private int attackerDamageCounter;
        private int defenderDamageCounter;
        private bool attackerInRange;
        private bool defenderInRange;

        private bool freeAction;

        //Network-Related
        public bool PeerCanContinue;
        private bool SelfCanContinue { get; set; }

        private const int AttackPointSize = 40;

        public BattleContext(BattleView battleView)
        {
            this.battleView = battleView;
            frameCounter = 0;
            currentlyRolling = false;
            rollingCounter = 0;
            currentlyResolvingBlocks = false;
            currentlyResolvingDamage = false;
            CurrentState = BattleState.Start;
            attackerDamageCounter = 0;
            defenderDamageCounter = 0;
            freeAction = false;
        }


        public void StartNewCombat(GameUnit newAttacker, GameUnit newDefender, UnitStatistics newAttackerStats,
            UnitStatistics newDefenderStats, bool isFreeAction = false)
        {
            attacker = newAttacker;
            defender = newDefender;
            attackerStats = newAttackerStats;
            defenderStats = newDefenderStats;
            freeAction = isFreeAction;

            attacker.SetUnitAnimation(UnitAnimationState.Active);
            defender.SetUnitAnimation(UnitAnimationState.Active);

            //Treat the unit as off-screen if null
            attackerCoordinates = attacker.UnitEntity?.MapCoordinates ?? new Vector2(-1);
            defenderCoordinates = defender.UnitEntity?.MapCoordinates ?? new Vector2(-1);

            attackerInRange = true;
            defenderInRange = RangeComparison.TargetIsWithinRangeOfOrigin(
                defenderCoordinates,
                defenderStats.CurrentAtkRange,
                attackerCoordinates
            );

            SetupHelpWindow();
            SetupAttackerWindows();
            SetupDefenderWindows();

            attackerProcs = new List<ICombatProc>();
            foreach (StatusEffect effect in attacker.StatusEffects)
            {
                if (effect is ICombatProc procEffect) attackerProcs.Add(procEffect);
            }

            defenderProcs = new List<ICombatProc>();
            foreach (StatusEffect effect in defender.StatusEffects)
            {
                if (effect is ICombatProc procEffect) defenderProcs.Add(procEffect);
            }

            attackerProcs.ForEach(proc => proc.OnCombatStart(attacker, defender));
            defenderProcs.ForEach(proc => proc.OnCombatStart(attacker, defender));

            SetPromptWindowText("Start Combat!");

            GameContext.MapCamera.SetZoomLevel(MapCamera.ZoomLevel.Combat);
            SelfCanContinue = true;
            GlobalEventQueue.QueueSingleEvent(new CombatNotifyStateCompleteEvent(CurrentState));
        }

        public void ContinueCombat()
        {
            switch (CurrentState)
            {
                case BattleState.Start:
                    //Skip the dice roll if neither unit has dice
                    if (attackerDamage.CountBlanks() > 0 || defenderDamage.CountBlanks() > 0)
                    {
                        if (TryProceedToState(BattleState.RollDice))
                        {
                            AssetManager.MapUnitSelectSFX.Play();
                            StartRollingDice();
                        }
                    }
                    else
                    {
                        if (TryProceedToState(BattleState.ResolveCombat))
                        {
                            StartResolvingBlocks();
                        }
                    }

                    break;
                case BattleState.RollDice:
                    if (TryProceedToState(BattleState.ResolveCombat))
                    {
                        AssetManager.MapUnitSelectSFX.Play();
                        StartResolvingBlocks();
                    }

                    break;
                case BattleState.ResolveCombat:
                    if (TryProceedToState(BattleState.Start))
                    {
                        attackerProcs.ForEach(proc => proc.OnCombatEnd(attacker, defender));
                        defenderProcs.ForEach(proc => proc.OnCombatEnd(attacker, defender));

                        AssetManager.MapUnitSelectSFX.Play();
                        GameContext.MapCamera.RevertToPreviousZoomLevel();
                        GameContext.GameMapContext.CurrentTurnState = GameMapContext.TurnState.FinishingCombat;

                        //Events
                        if (!attacker.IsAlive)
                        {
                            TakeAttackerSpoilsAndIncreaseBounty();
                        }
                        else if (!defender.IsAlive)
                        {
                            TakeDefenderSpoilsAndIncreaseBounty();
                        }

                        if (freeAction)
                        {
                            GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
                        }
                        else
                        {
                            GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TakeDefenderSpoilsAndIncreaseBounty()
        {
            MapSlice defenderSlice = MapContainer.GetMapSliceAtCoordinates(defenderCoordinates);
            if (!(defenderSlice.ItemEntity is Spoils defenderSpoils)) return;
            GlobalEventQueue.QueueSingleEvent(new TakeSpoilsEvent(defenderSpoils, attacker));
            GlobalEventQueue.QueueSingleEvent(new CameraCursorPositionEvent(attackerCoordinates));

            string toastMessage = $"Obtained {defenderSpoils.Gold} {Currency.CurrencyAbbreviation}!";

            foreach (IItem item in defenderSpoils.Items)
            {
                toastMessage += Environment.NewLine;
                toastMessage += $"Obtained {item.Name}!";
            }

            attacker.CurrentBounty += BountyPerTriumph;
            toastMessage += Environment.NewLine + Environment.NewLine +
                            $"{attacker.Id} bounty increased by {BountyPerTriumph}!";

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(attackerCoordinates, toastMessage, AssetManager.CoinSFX, 80)
            );

            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(60));
        }

        private void TakeAttackerSpoilsAndIncreaseBounty()
        {
            MapSlice attackerSlice = MapContainer.GetMapSliceAtCoordinates(attackerCoordinates);
            if (!(attackerSlice.ItemEntity is Spoils attackerSpoils)) return;
            GlobalEventQueue.QueueSingleEvent(new TakeSpoilsEvent(attackerSpoils, defender));
            GlobalEventQueue.QueueSingleEvent(new CameraCursorPositionEvent(defenderCoordinates));

            string toastMessage = $"Obtained {attackerSpoils.Gold} {Currency.CurrencyAbbreviation}!";

            foreach (IItem item in attackerSpoils.Items)
            {
                toastMessage += Environment.NewLine;
                toastMessage += $"Obtained {item.Name}!";
            }

            defender.CurrentBounty += BountyPerTriumph;
            toastMessage += Environment.NewLine + Environment.NewLine +
                            $"{defender.Id} bounty increased by {BountyPerTriumph}!";

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(defenderCoordinates, toastMessage, AssetManager.CoinSFX, 80)
            );
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(60));
        }

        private void SetPromptWindowText(string promptText)
        {
            IRenderable[,] promptTextContent =
            {
                {
                    new RenderText(AssetManager.PromptFont, promptText),
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank
                },
                {
                    new RenderText(AssetManager.PromptFont, "["),
                    new RenderText(AssetManager.PromptFont, "Press "),
                    InputIconProvider.GetInputIcon(Input.Confirm,
                        new Vector2(AssetManager.PromptFont.MeasureString("A").Y)),
                    new RenderText(AssetManager.PromptFont, "]")
                }
            };
            WindowContentGrid promptWindowContentGrid = new WindowContentGrid(promptTextContent, 2);

            Vector2 threeBarsHighOrTaller = new Vector2(0, battleView.AttackerBonusWindow.Height * 3);
            if (threeBarsHighOrTaller.Y < promptWindowContentGrid.Height)
            {
                threeBarsHighOrTaller.Y = 0;
            }

            battleView.GenerateUserPromptWindow(promptWindowContentGrid, threeBarsHighOrTaller);
        }

        private void SetupHelpWindow()
        {
            const string helpText =
                "INFO: Swords deal 1 damage. Shields block swords. Blanks are ignored. Swords are ignored if not in range.";
            IRenderable[,] textToRender =
            {
                {
                    new RenderText(AssetManager.WindowFont, helpText, Color.White),
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank
                },
                {
                    new RenderText(AssetManager.WindowFont, "Dice Legend: ", Color.White),
                    new RenderText(AssetManager.WindowFont, "[Unresolved] ", Color.White),
                    new RenderText(AssetManager.WindowFont, "[Bonus] ", new Color(100, 250, 100)),
                    new RenderText(AssetManager.WindowFont, "[Damage] ", new Color(250, 100, 100)),
                    new RenderText(AssetManager.WindowFont, "[Blocked] ", new Color(100, 100, 250)),
                    new RenderText(AssetManager.WindowFont, "[Ignored]", Color.Gray)
                }
            };
            WindowContentGrid helpTextWindowContentGrid = new WindowContentGrid(textToRender, 2);
            battleView.GenerateHelpTextWindow(helpTextWindowContentGrid);
        }

        private void SetupAttackerWindows()
        {
            BonusStatistics attackerBonus =
                DetermineTerrainBonusForUnit(attacker) + DetermineAuraBonusForUnit(attacker);

            attackerDamage = new CombatDamage(
                attackerStats.Atk,
                attackerStats.Blk,
                attackerStats.Luck,
                attackerBonus.AtkBonus,
                attackerBonus.BlockBonus,
                attackerBonus.LuckBonus,
                AttackPointSize
            );
            Color attackerWindowColor = TeamUtility.DetermineTeamColor(attacker.Team);

            battleView.GenerateAttackerUnitCard(attackerWindowColor, attacker);
            battleView.GenerateAttackerHpWindow(attackerWindowColor, attacker);
            battleView.GenerateAttackerAtkWindow(attackerWindowColor, attackerStats, Stats.Atk);
            battleView.GenerateAttackerInRangeWindow(attackerWindowColor, attackerInRange);
            battleView.GenerateAttackerBonusWindow(attackerBonus, attackerWindowColor);
            battleView.GenerateAttackerDamageWindow(attackerWindowColor, attackerDamage);
            battleView.GenerateAttackerSpriteWindow(attacker, Color.White, UnitAnimationState.Active);
        }

        private void SetupDefenderWindows()
        {
            BonusStatistics defenderBonus =
                DetermineTerrainBonusForUnit(defender) + DetermineAuraBonusForUnit(defender);
            defenderDamage = new CombatDamage(
                defenderStats.Ret,
                defenderStats.Blk,
                defenderStats.Luck,
                defenderBonus.RetBonus,
                defenderBonus.BlockBonus,
                defenderBonus.LuckBonus,
                AttackPointSize
            );
            Color defenderWindowColor = TeamUtility.DetermineTeamColor(defender.Team);

            battleView.GenerateDefenderUnitCard(defenderWindowColor, defender);
            battleView.GenerateDefenderHpWindow(defenderWindowColor, defender);
            battleView.GenerateDefenderRetWindow(defenderWindowColor, defenderStats, Stats.Retribution);
            battleView.GenerateDefenderRangeWindow(defenderWindowColor, defenderInRange);
            battleView.GenerateDefenderBonusWindow(defenderBonus, defenderWindowColor);
            battleView.GenerateDefenderDamageWindow(defenderWindowColor, defenderDamage);
            battleView.GenerateDefenderSpriteWindow(defender, Color.White, UnitAnimationState.Active);
        }

        private bool TryProceedToState(BattleState state)
        {
            if (currentlyResolvingBlocks || currentlyResolvingDamage || currentlyRolling) return false;

            PeerCanContinue = false;
            SelfCanContinue = false;

            CurrentState = state;
            Trace.WriteLine("Changing combat state: " + CurrentState);
            return true;
        }

        private static BonusStatistics DetermineTerrainBonusForUnit(GameUnit unit)
        {
            MapSlice unitSlice = MapContainer.GetMapSliceAtCoordinates(unit.UnitEntity.MapCoordinates);

            return !(unitSlice.TerrainEntity is BuffTile buffTile) ? new BonusStatistics() : buffTile.BonusStatistics;
        }

        private static BonusStatistics DetermineAuraBonusForUnit(GameUnit auraAffectedUnit)
        {
            BonusStatistics totalBonus = new BonusStatistics(0, 0, 0, 0);

            List<SongStatus> songsInPlay = new List<SongStatus>();
            foreach (GameUnit livingUnit in GameContext.Units.Where(unit => unit.IsAlive))
            {
                songsInPlay.AddRange(livingUnit.StatusEffects.Where(status => status is SongStatus).Cast<SongStatus>()
                    .ToList());
            }

            foreach (SongStatus song in songsInPlay)
            {
                if (song.UnitIsAffectedBySong(auraAffectedUnit))
                {
                    totalBonus += song.ActiveBonus;
                }
            }

            return totalBonus;
        }

        private void StartRollingDice()
        {
            if (currentlyRolling) return;
            currentlyRolling = true;
            battleView.HidePromptWindow();
        }

        private void RollDice()
        {
            rollingCounter++;
            const int rollingFrames = 60;
            if (rollingCounter >= rollingFrames)
            {
                rollingCounter = 0;
                currentlyRolling = false;

                SetPromptWindowText("Resolve dice.");
                SelfCanContinue = true;
                GlobalEventQueue.QueueSingleEvent(new CombatNotifyStateCompleteEvent(CurrentState));
            }

            const int renderDelay = 3;
            if (frameCounter % renderDelay == 0)
            {
                attackerDamage.RollDice();
                defenderDamage.RollDice();
                AssetManager.DiceRollSFX.Play();
            }
        }

        private void StartResolvingBlocks()
        {
            if (currentlyResolvingBlocks) return;
            currentlyResolvingBlocks = true;
            battleView.HidePromptWindow();
        }

        private void ResolveBlocks()
        {
            int attackerSwords = attackerDamage.CountDamage();
            int defenderSwords = defenderDamage.CountDamage();
            int attackerShields = attackerDamage.CountShields();
            int defenderShields = defenderDamage.CountShields();

            //Animate grey-out of each pair of swords+shields, one after another
            const int renderDelay = 20;

            if (frameCounter % renderDelay != 0) return;

            if (attackerInRange && attackerSwords > 0 && defenderShields > 0)
            {
                attackerDamage.BlockAttackPoint();
                defenderDamage.ResolveBlockPoint();
                attackerProcs.ForEach(proc => proc.OnBlock(attacker, defender));
                AssetManager.CombatBlockSFX.Play();
            }
            else if (defenderInRange && defenderSwords > 0 && attackerShields > 0)
            {
                defenderDamage.BlockAttackPoint();
                attackerDamage.ResolveBlockPoint();
                defenderProcs.ForEach(proc => proc.OnBlock(defender, attacker));
                AssetManager.CombatBlockSFX.Play();
            }
            else
            {
                //Don't count defender's attack dice if out of range
                if (!defenderInRange)
                {
                    defenderDamage.DisableAllAttackPoints();
                    defenderDamage.DisableAllDiceWithValue(Die.FaceValue.Sword);
                }

                currentlyResolvingBlocks = false;

                StartResolvingDamage();
            }
        }

        private void StartResolvingDamage()
        {
            if (currentlyResolvingDamage) return;
            currentlyResolvingDamage = true;
            battleView.HidePromptWindow();
        }

        private void ResolveDamage()
        {
            int attackerSwords = attackerDamage.CountDamage();
            int defenderSwords = defenderDamage.CountDamage();

            //Animate HP bar taking one damage at a time
            const int renderDelay = 12;
            const float maxDamageShakeOffset = 10f;
            const int damageShakeDurationInFrames = 5;

            if (frameCounter % renderDelay != 0) return;

            if (NonSwordPointsRemain())
            {
                //Disable blank dice after all other dice resolved
                attackerDamage.DisableAllDiceWithValue(Die.FaceValue.Blank);
                defenderDamage.DisableAllDiceWithValue(Die.FaceValue.Blank);
                attackerDamage.DisableAllDiceWithValue(Die.FaceValue.Shield);
                defenderDamage.DisableAllDiceWithValue(Die.FaceValue.Shield);
                attackerDamage.DisableRemainingShields();
                defenderDamage.DisableRemainingShields();

                AssetManager.DisableDiceSFX.Play();
            }
            else if (attackerSwords > 0 && attackerInRange)
            {
                attackerDamage.ResolveDamagePoint();
                defender.DamageUnit();

                attackerProcs.ForEach(proc => proc.OnDamage(attacker, defender));
                battleView.GenerateAttackerSpriteWindow(attacker, Color.White, UnitAnimationState.Attack);
                battleView.GenerateDefenderSpriteWindow(defender, Color.White, UnitAnimationState.Hit);
                battleView.GenerateDefenderHpWindow(
                    TeamUtility.DetermineTeamColor(defender.Team),
                    defender,
                    new RenderableShake(maxDamageShakeOffset, damageShakeDurationInFrames)
                );

                attackerDamageCounter++;
                AssetManager.CombatDamageSFX.Play();
            }
            else if (defenderSwords > 0 && defenderInRange && defenderStats.CurrentHP > 0)
            {
                defenderDamage.ResolveDamagePoint();
                attacker.DamageUnit();

                defenderProcs.ForEach(proc => proc.OnCombatStart(defender, attacker));
                battleView.GenerateAttackerSpriteWindow(attacker, Color.White, UnitAnimationState.Hit);
                battleView.GenerateDefenderSpriteWindow(defender, Color.White, UnitAnimationState.Attack);
                battleView.GenerateAttackerHpWindow(
                    TeamUtility.DetermineTeamColor(attacker.Team),
                    attacker,
                    new RenderableShake(maxDamageShakeOffset, damageShakeDurationInFrames)
                );

                defenderDamageCounter++;
                AssetManager.CombatDamageSFX.Play();
            }
            else
            {
                currentlyResolvingDamage = false;

                SetPromptWindowDamageReport();
                attacker.SetUnitAnimation(UnitAnimationState.Idle);
                defender.SetUnitAnimation(UnitAnimationState.Idle);
                battleView.GenerateAttackerSpriteWindow(attacker, Color.White, UnitAnimationState.Idle);
                battleView.GenerateDefenderSpriteWindow(defender, Color.White, UnitAnimationState.Idle);
                ResetDamageCounters();

                GlobalEventQueue.QueueSingleEvent(new CombatNotifyStateCompleteEvent(CurrentState));
            }

            if (attackerStats.CurrentHP <= 0)
            {
                battleView.GenerateAttackerSpriteWindow(attacker, Color.White, UnitAnimationState.Idle);
                attackerDamage.DisableAllAttackPoints();
            }

            if (defenderStats.CurrentHP <= 0)
            {
                battleView.GenerateDefenderSpriteWindow(defender, Color.White, UnitAnimationState.Idle);
                defenderDamage.DisableAllAttackPoints();
            }
        }

        private void SetPromptWindowDamageReport()
        {
            string damageReport = "Attacker " + attacker.Id + " deals " + attackerDamageCounter + " damage!\n";
            damageReport += "Defender " + defender.Id + " deals " + defenderDamageCounter + " damage!\n";
            if (defenderStats.CurrentHP <= 0) damageReport += defender.Id + " is defeated!\n";
            if (attackerStats.CurrentHP <= 0) damageReport += attacker.Id + " is defeated!\n";
            damageReport += "End Combat.";
            SelfCanContinue = true;
            SetPromptWindowText(damageReport);
        }

        private void ResetDamageCounters()
        {
            attackerDamageCounter = 0;
            defenderDamageCounter = 0;
        }

        private bool NonSwordPointsRemain()
        {
            bool blanksLeft = (attackerDamage.CountBlanks() > 0 || defenderDamage.CountBlanks() > 0);
            bool shieldsLeft = (attackerDamage.CountShields() > 0 || defenderDamage.CountShields() > 0);

            return blanksLeft || shieldsLeft;
        }

        private void UpdateDice()
        {
            if (currentlyRolling)
            {
                RollDice();
            }
            else if (currentlyResolvingBlocks)
            {
                ResolveBlocks();
            }
            else if (currentlyResolvingDamage)
            {
                ResolveDamage();
                battleView.GenerateAttackerUnitCard(TeamUtility.DetermineTeamColor(attacker.Team), attacker, false);
                battleView.GenerateDefenderUnitCard(TeamUtility.DetermineTeamColor(defender.Team), defender, false);
            }
        }

        public bool CombatCanContinue
        {
            get
            {
                if (GameDriver.ConnectedAsClient || GameDriver.ConnectedAsServer)
                {
                    return PeerCanContinue && SelfCanContinue;
                }

                return SelfCanContinue;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            frameCounter++;
            UpdateDice();
            battleView.Draw(spriteBatch);
        }
    }
}