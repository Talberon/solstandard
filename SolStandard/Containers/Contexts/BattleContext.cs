using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.UI;
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
            CountDice,
            ResolveCombat,
        }

        private readonly BattleUI battleUI;
        private const int HpBarHeight = 25;

        private CombatDice attackerDice;
        private CombatDice defenderDice;

        private int frameCounter;
        private bool currentlyRolling;
        private int rollingCounter;
        private bool currentlyResolvingBlocks;
        private bool currentlyResolvingDamage;

        public BattleState CurrentState { get; private set; }

        private GameUnit attacker;
        private GameUnit defender;

        private int attackerDamageCounter;
        private int defenderDamageCounter;
        private bool attackerInRange;
        private bool defenderInRange;

        public BattleContext(BattleUI battleUI)
        {
            this.battleUI = battleUI;
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
            
            attacker.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
            defender.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);

            //Treat the unit as off-screen if null
            Vector2 attackerCoordinates =
                (attacker.UnitEntity != null) ? attacker.UnitEntity.MapCoordinates : new Vector2(-1);
            Vector2 defenderCoordinates =
                (defender.UnitEntity != null) ? defender.UnitEntity.MapCoordinates : new Vector2(-1);
            
            attackerInRange = CoordinatesAreInRange(attackerCoordinates, defenderCoordinates, attacker.Stats.AtkRange);
            defenderInRange = CoordinatesAreInRange(defenderCoordinates, attackerCoordinates, defender.Stats.AtkRange);
            
            SetupHelpWindow();
            SetupAttackerWindows(attackerSlice);
            SetupDefenderWindows(defenderSlice);
            SetPromptWindowText("Start Combat!");

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
                    ButtonIconProvider.GetButton(ButtonIcon.A, new Vector2(AssetManager.PromptFont.MeasureString("A").Y)),
                    new RenderText(AssetManager.PromptFont, "]")
                }
            };
            WindowContentGrid promptWindowContentGrid = new WindowContentGrid(promptTextContent, 2);

            Vector2 threeBarsHighOrTaller = new Vector2(0, battleUI.AttackerBonusWindow.Height * 3);
            if (threeBarsHighOrTaller.Y < promptWindowContentGrid.Height)
            {
                threeBarsHighOrTaller.Y = 0;
            }

            battleUI.GenerateUserPromptWindow(promptWindowContentGrid, threeBarsHighOrTaller);
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
            battleUI.GenerateHelpTextWindow(helpTextWindowContentGrid);
        }

        private void SetupAttackerWindows(MapSlice attackerSlice)
        {
            Color attackerWindowColor = TeamUtility.DetermineTeamColor(attacker.Team);
            battleUI.GenerateAttackerPortraitWindow(attackerWindowColor, attacker.LargePortrait);

            Vector2 portraitWidthOverride = new Vector2(battleUI.AttackerPortraitWindow.Width, 0);
            battleUI.GenerateAttackerLabelWindow(attackerWindowColor, portraitWidthOverride, attacker.Id);
            battleUI.GenerateAttackerClassWindow(attackerWindowColor, portraitWidthOverride,
                attacker.Role.ToString());
            battleUI.GenerateAttackerHpWindow(attackerWindowColor, portraitWidthOverride, attacker, HpBarHeight);
            battleUI.GenerateAttackerAtkWindow(attackerWindowColor, portraitWidthOverride, attacker.Stats.Atk);
            battleUI.GenerateAttackerInRangeWindow(attackerWindowColor, portraitWidthOverride, attackerInRange);
            battleUI.GenerateAttackerDiceLabelWindow(attackerWindowColor);

            int terrainAttackBonus = battleUI.GenerateAttackerBonusWindow(attackerSlice, attackerWindowColor,
                portraitWidthOverride);
            attackerDice = new CombatDice(attacker.Stats.Atk, terrainAttackBonus, 3);
            battleUI.GenerateAttackerDiceWindow(attackerWindowColor, ref attackerDice);
        }

        private void SetupDefenderWindows(MapSlice defenderSlice)
        {
            Color defenderWindowColor = TeamUtility.DetermineTeamColor(defender.Team);
            battleUI.GenerateDefenderPortraitWindow(defenderWindowColor, defender.LargePortrait);

            Vector2 portraitWidthOverride = new Vector2(battleUI.DefenderPortraitWindow.Width, 0);
            battleUI.GenerateDefenderLabelWindow(defenderWindowColor, portraitWidthOverride, defender.Id);
            battleUI.GenerateDefenderClassWindow(defenderWindowColor, portraitWidthOverride,
                defender.Role.ToString());
            battleUI.GenerateDefenderHpWindow(defenderWindowColor, portraitWidthOverride, defender, HpBarHeight);
            battleUI.GenerateDefenderDefWindow(defenderWindowColor, portraitWidthOverride, defender.Stats.Def);
            battleUI.GenerateDefenderRangeWindow(defenderWindowColor, portraitWidthOverride, defenderInRange);
            battleUI.GenerateDefenderDiceLabelWindow(defenderWindowColor);

            int terrainDefenseBonus =
                battleUI.GenerateDefenderBonusWindow(defenderSlice, defenderWindowColor, portraitWidthOverride);
            defenderDice = new CombatDice(defender.Stats.Def, terrainDefenseBonus, 3);
            battleUI.GenerateDefenderDiceWindow(defenderWindowColor, ref defenderDice);
        }

        public bool TryProceedToNextState()
        {
            if (currentlyResolvingBlocks || currentlyResolvingDamage || currentlyRolling) return false;

            if (CurrentState == BattleState.ResolveCombat)
            {
                CurrentState = 0;
                Trace.WriteLine("Resetting to initial combat state: " + CurrentState);
                return true;
            }
            else
            {
                CurrentState++;
                Trace.WriteLine("Changing combat state: " + CurrentState);
                return true;
            }
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
                battleUI.UserPromptWindow.Visible = false;
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

                SetPromptWindowText("Resolve Blocks.");
            }

            const int renderDelay = 3;
            if (frameCounter % renderDelay == 0)
            {
                attackerDice.RollDice();
                defenderDice.RollDice();
                AssetManager.DiceRollSFX.Play();
            }
            
        }


        public void StartResolvingBlocks()
        {
            if (!currentlyResolvingBlocks)
            {
                currentlyResolvingBlocks = true;
                battleUI.UserPromptWindow.Visible = false;
            }
        }

        private void ResolveBlocks()
        {
            int attackerSwords = attackerDice.CountFaceValue(Die.FaceValue.Sword, true);
            int defenderSwords = defenderDice.CountFaceValue(Die.FaceValue.Sword, true);
            int attackerShields = attackerDice.CountFaceValue(Die.FaceValue.Shield, true);
            int defenderShields = defenderDice.CountFaceValue(Die.FaceValue.Shield, true);

            //Animate grey-out of each pair of swords+shields, one after another
            const int renderDelay = 30;
            if (frameCounter % renderDelay == 0)
            {
                if (attackerInRange && attackerSwords > 0 && defenderShields > 0)
                {
                    attackerDice.BlockNextDieWithValue(Die.FaceValue.Sword);
                    defenderDice.BlockNextDieWithValue(Die.FaceValue.Shield);
                    AssetManager.CombatBlockSFX.Play();
                }
                else if (defenderInRange && defenderSwords > 0 && attackerShields > 0)
                {
                    defenderDice.BlockNextDieWithValue(Die.FaceValue.Sword);
                    attackerDice.BlockNextDieWithValue(Die.FaceValue.Shield);
                    AssetManager.CombatBlockSFX.Play();
                }
                else
                {
                    //Don't count defender's attack dice if out of range
                    if (!defenderInRange)
                    {
                        defenderDice.DisableAllDiceWithValue(Die.FaceValue.Sword);
                        AssetManager.DisableDiceSFX.Play();
                    }

                    currentlyResolvingBlocks = false;

                    SetPromptWindowText("Resolve Damage.");
                }
            }
        }

        public void StartResolvingDamage()
        {
            if (!currentlyResolvingDamage)
            {
                currentlyResolvingDamage = true;
                battleUI.UserPromptWindow.Visible = false;
            }
        }

        private void ResolveDamage()
        {
            int attackerDamage = attackerDice.CountFaceValue(Die.FaceValue.Sword, true);
            int defenderDamage = defenderDice.CountFaceValue(Die.FaceValue.Sword, true);

            //Animate HP bar taking one damage at a time
            const int renderDelay = 30;
            if (frameCounter % renderDelay == 0)
            {
                if (NonSwordDiceRemain())
                {
                    //Disable blank dice after all other dice resolved
                    attackerDice.DisableAllDiceWithValue(Die.FaceValue.Blank);
                    defenderDice.DisableAllDiceWithValue(Die.FaceValue.Blank);
                    attackerDice.DisableAllDiceWithValue(Die.FaceValue.Shield);
                    defenderDice.DisableAllDiceWithValue(Die.FaceValue.Shield);
                    AssetManager.DisableDiceSFX.Play();
                }
                else if (attackerDamage > 0 && attackerInRange)
                {
                    attackerDice.ResolveDamageNextDieWithValue(Die.FaceValue.Sword);
                    defender.DamageUnit(1);
                    attackerDamageCounter++;
                    AssetManager.CombatDamageSFX.Play();
                }
                else if (defenderDamage > 0 && defenderInRange)
                {
                    defenderDice.ResolveDamageNextDieWithValue(Die.FaceValue.Sword);
                    attacker.DamageUnit(1);
                    defenderDamageCounter++;
                    AssetManager.CombatDamageSFX.Play();
                }
                else
                {
                    currentlyResolvingDamage = false;

                    SetPromptWindowDamageReport();
                    attacker.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                    defender.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                    ResetDamageCounters();
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
            bool blanksLeft = (attackerDice.CountFaceValue(Die.FaceValue.Blank, true) > 0 ||
                               defenderDice.CountFaceValue(Die.FaceValue.Blank, true) > 0);
            bool shieldsLeft = (attackerDice.CountFaceValue(Die.FaceValue.Shield, true) > 0 ||
                                defenderDice.CountFaceValue(Die.FaceValue.Shield, true) > 0);

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
            battleUI.Draw(spriteBatch);
        }
    }
}