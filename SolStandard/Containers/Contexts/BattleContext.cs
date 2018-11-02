using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Combat;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts
{
    public class BattleContext
    {
        public enum BattleState
        {
            Start,
            RollDice,
            ResolveCombat,
        }

        private readonly BattleView battleView;

        private CombatDamage attackerDamage;
        private CombatDamage defenderDamage;

        private int frameCounter;
        private bool currentlyRolling;
        private int rollingCounter;
        private bool currentlyResolvingBlocks;
        private bool currentlyResolvingDamage;

        private BattleState CurrentState { get; set; }

        private GameUnit attacker;
        private GameUnit defender;

        private int attackerDamageCounter;
        private int defenderDamageCounter;
        private bool attackerInRange;
        private bool defenderInRange;

        private const int AttackPointSize = 48;

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
        }

        public void StartNewCombat(GameUnit newAttacker, MapSlice attackerSlice, GameUnit newDefender,
            MapSlice defenderSlice)
        {
            attacker = newAttacker;
            defender = newDefender;

            attacker.SetUnitAnimation(UnitAnimationState.Attack);
            defender.SetUnitAnimation(UnitAnimationState.Attack);

            //Treat the unit as off-screen if null
            Vector2 attackerCoordinates =
                (attacker.UnitEntity != null) ? attacker.UnitEntity.MapCoordinates : new Vector2(-1);
            Vector2 defenderCoordinates =
                (defender.UnitEntity != null) ? defender.UnitEntity.MapCoordinates : new Vector2(-1);

            attackerInRange = true;
            defenderInRange = CoordinatesAreInRange(defenderCoordinates, attackerCoordinates, defender.Stats.AtkRange);

            SetupHelpWindow();
            SetupAttackerWindows(attackerSlice);
            SetupDefenderWindows(defenderSlice);
            SetPromptWindowText("Start Combat!");
        }

        public void ContinueCombat()
        {
            switch (CurrentState)
            {
                case BattleState.Start:
                    if (TryProceedToState(BattleState.RollDice))
                    {
                        AssetManager.MapUnitSelectSFX.Play();
                        StartRollingDice();
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
                        AssetManager.MapUnitSelectSFX.Play();
                        GameContext.GameMapContext.ProceedToNextState();
                    }

                    break;
                default:
                    GameContext.GameMapContext.ProceedToNextState();
                    return;
            }
        }

        private void SetPromptWindowText(string promptText)
        {
            IRenderable[,] promptTextContent =
            {
                {
                    new RenderText(AssetManager.PromptFont, promptText),
                    new RenderBlank(),
                    new RenderBlank(),
                    new RenderBlank()
                },
                {
                    new RenderText(AssetManager.PromptFont, "["),
                    new RenderText(AssetManager.PromptFont, "Press "),
                    ButtonIconProvider.GetButton(ButtonIcon.A,
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
                    new RenderBlank(),
                    new RenderBlank(),
                    new RenderBlank(),
                    new RenderBlank(),
                    new RenderBlank()
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

        private void SetupAttackerWindows(MapSlice attackerSlice)
        {
            Color attackerWindowColor = TeamUtility.DetermineTeamColor(attacker.Team);
            battleView.GenerateAttackerPortraitWindow(attackerWindowColor, attacker.MediumPortrait);

            //FIXME Remove the override
            Vector2 windowWidthOverride = new Vector2(0, 0);
            battleView.GenerateAttackerDetailWindow(attackerWindowColor, windowWidthOverride, attacker.DetailPane);
            battleView.GenerateAttackerHpWindow(attackerWindowColor, windowWidthOverride, attacker);
            battleView.GenerateAttackerAtkWindow(attackerWindowColor, windowWidthOverride, attacker.Stats);
            battleView.GenerateAttackerInRangeWindow(attackerWindowColor, windowWidthOverride, attackerInRange);

            int terrainAttackBonus = battleView.GenerateAttackerBonusWindow(attackerSlice, attackerWindowColor,
                windowWidthOverride);
            attackerDamage = new CombatDamage(attacker.Stats, terrainAttackBonus, AttackPointSize);
            battleView.GenerateAttackerDamageWindow(attackerWindowColor, attackerDamage);
            battleView.GenerateAttackerSpriteWindow(attacker, Color.White, UnitAnimationState.Attack);
        }

        private void SetupDefenderWindows(MapSlice defenderSlice)
        {
            Color defenderWindowColor = TeamUtility.DetermineTeamColor(defender.Team);
            battleView.GenerateDefenderPortraitWindow(defenderWindowColor, defender.MediumPortrait);

            //FIXME Remove the override
            Vector2 windowWidthOverride = new Vector2(0, 0);
            battleView.GenerateDefenderDetailWindow(defenderWindowColor, windowWidthOverride, defender.DetailPane);
            battleView.GenerateDefenderHpWindow(defenderWindowColor, windowWidthOverride, defender);
            battleView.GenerateDefenderDefWindow(defenderWindowColor, windowWidthOverride, defender.Stats);
            battleView.GenerateDefenderRangeWindow(defenderWindowColor, windowWidthOverride, defenderInRange);

            int terrainDefenseBonus =
                battleView.GenerateDefenderBonusWindow(defenderSlice, defenderWindowColor, windowWidthOverride);
            defenderDamage = new CombatDamage(defender.Stats, terrainDefenseBonus, AttackPointSize);
            battleView.GenerateDefenderDamageWindow(defenderWindowColor, defenderDamage);
            battleView.GenerateDefenderSpriteWindow(defender, Color.White, UnitAnimationState.Attack);
        }

        public bool TryProceedToState(BattleState state)
        {
            if (currentlyResolvingBlocks || currentlyResolvingDamage || currentlyRolling) return false;

            CurrentState = state;
            Trace.WriteLine("Changing combat state: " + CurrentState);
            return true;
        }

        public static bool CoordinatesAreInRange(Vector2 sourcePosition, Vector2 targetPosition,
            IEnumerable<int> sourceRange)
        {
            /*Since distance is measured in horizontal and vertical steps, the absolute value of the difference of
             absolute positions should add up to the appropriate range.*/
            int horizontalDistance = Math.Abs(Math.Abs((int) targetPosition.X) -
                                              Math.Abs((int) sourcePosition.X));

            int verticalDistance = Math.Abs(Math.Abs((int) targetPosition.Y) -
                                            Math.Abs((int) sourcePosition.Y));
            return sourceRange.Any(range => horizontalDistance + verticalDistance == range);
        }

        public void StartRollingDice()
        {
            if (!currentlyRolling)
            {
                currentlyRolling = true;
                battleView.HidePromptWindow();
            }
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
            }

            const int renderDelay = 3;
            if (frameCounter % renderDelay == 0)
            {
                attackerDamage.RollDice();
                defenderDamage.RollDice();
                AssetManager.DiceRollSFX.Play();
            }
        }

        public void StartResolvingBlocks()
        {
            if (!currentlyResolvingBlocks)
            {
                currentlyResolvingBlocks = true;
                battleView.HidePromptWindow();
            }
        }

        private void ResolveBlocks()
        {
            int attackerSwords = attackerDamage.CountDamage();
            int defenderSwords = defenderDamage.CountDamage();
            int attackerShields = attackerDamage.CountShields();
            int defenderShields = defenderDamage.CountShields();

            //Animate grey-out of each pair of swords+shields, one after another
            const int renderDelay = 20;
            if (frameCounter % renderDelay == 0)
            {
                if (attackerInRange && attackerSwords > 0 && defenderShields > 0)
                {
                    attackerDamage.BlockAttackPoint();
                    defenderDamage.ResolveBlockDie();
                    AssetManager.CombatBlockSFX.Play();
                }
                else if (defenderInRange && defenderSwords > 0 && attackerShields > 0)
                {
                    defenderDamage.BlockAttackPoint();
                    attackerDamage.ResolveBlockDie();
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
        }

        public void StartResolvingDamage()
        {
            if (!currentlyResolvingDamage)
            {
                currentlyResolvingDamage = true;
                battleView.HidePromptWindow();
            }
        }

        private void ResolveDamage()
        {
            int attackerSwords = attackerDamage.CountDamage();
            int defenderSwords = defenderDamage.CountDamage();

            //Animate HP bar taking one damage at a time
            const int renderDelay = 12;
            if (frameCounter % renderDelay == 0)
            {
                if (NonSwordDiceRemain())
                {
                    //Disable blank dice after all other dice resolved
                    attackerDamage.DisableAllDiceWithValue(Die.FaceValue.Blank);
                    defenderDamage.DisableAllDiceWithValue(Die.FaceValue.Blank);
                    attackerDamage.DisableAllDiceWithValue(Die.FaceValue.Shield);
                    defenderDamage.DisableAllDiceWithValue(Die.FaceValue.Shield);
                    AssetManager.DisableDiceSFX.Play();
                }
                else if (attackerSwords > 0 && attackerInRange)
                {
                    attackerDamage.ResolveDamagePoint();
                    defender.DamageUnit();
                    attackerDamageCounter++;
                    AssetManager.CombatDamageSFX.Play();
                }
                else if (defenderSwords > 0 && defenderInRange && defender.Stats.Hp > 0)
                {
                    defenderDamage.ResolveDamagePoint();
                    attacker.DamageUnit();
                    defenderDamageCounter++;
                    AssetManager.CombatDamageSFX.Play();
                }
                else
                {
                    currentlyResolvingDamage = false;

                    SetPromptWindowDamageReport();
                    attacker.SetUnitAnimation(UnitAnimationState.Idle);
                    defender.SetUnitAnimation(UnitAnimationState.Idle);
                    ResetDamageCounters();
                }

                if (attacker.Stats.Hp <= 0)
                {
                    battleView.GenerateAttackerSpriteWindow(attacker, GameUnit.DeadPortraitColor,
                        UnitAnimationState.Idle);
                    attackerDamage.DisableAllAttackPoints();
                }

                if (defender.Stats.Hp <= 0)
                {
                    battleView.GenerateDefenderSpriteWindow(defender, GameUnit.DeadPortraitColor,
                        UnitAnimationState.Idle);
                    defenderDamage.DisableAllAttackPoints();
                }
            }
        }

        private void SetPromptWindowDamageReport()
        {
            string damageReport = "Attacker " + attacker.Id + " deals " + attackerDamageCounter + " damage!\n";
            damageReport += "Defender " + defender.Id + " deals " + defenderDamageCounter + " damage!\n";
            if (defender.Stats.Hp <= 0) damageReport += defender.Id + " is defeated!\n";
            if (attacker.Stats.Hp <= 0) damageReport += attacker.Id + " is defeated!\n";
            damageReport += "End Combat.";
            SetPromptWindowText(damageReport);
        }

        private void ResetDamageCounters()
        {
            attackerDamageCounter = 0;
            defenderDamageCounter = 0;
        }

        private bool NonSwordDiceRemain()
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