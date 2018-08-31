using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Combat;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Containers.Contexts
{
    public class BattleContext
    {
        //TODO Implement procedure through steps via button presses in response to Window prompts from BattleUI
        public enum BattleState
        {
            Start,
            RollDice,
            CountDice,
            ResolveCombat,
        }

        private readonly BattleUI battleUI;
        private const int HpBarHeight = 20;

        private CombatDice attackerDice;
        private CombatDice defenderDice;

        private int frameCounter;
        private bool currentlyRolling;
        private int rollingCounter;
        private bool currentlyCountingDice;
        private bool currentlyResolvingDamage;

        public BattleState CurrentState { get; private set; }

        private GameUnit attacker;
        private GameUnit defender;

        public BattleContext(BattleUI battleUI)
        {
            this.battleUI = battleUI;
            frameCounter = 0;
            currentlyRolling = false;
            rollingCounter = 0;
            currentlyCountingDice = false;
            currentlyResolvingDamage = false;
            CurrentState = BattleState.Start;
            //TODO determine CombatFlow
        }

        public void StartNewCombat(GameUnit attacker, MapSlice attackerSlice, GameUnit defender, MapSlice defenderSlice)
        {
            this.attacker = attacker;
            this.defender = defender;

            const string helpText =
                "INFO: Swords deal 1 damage. Shields block swords. Blanks are ignored. Swords are ignored if not in range";
            const string legendNormal = "White=Normal ";
            const string legendBonus = "Green=Bonus ";
            const string legendDamage = "Red=Damage ";
            const string legendBlocked = "Blue=Blocked ";
            const string legendIgnored = "Grey=Ignored";
            IRenderable[,] textToRender =
            {
                {
                    new RenderText(GameDriver.WindowFont, helpText, Color.White),
                    new RenderBlank(),
                    new RenderBlank(),
                    new RenderBlank(),
                    new RenderBlank()
                },
                {
                    new RenderText(GameDriver.WindowFont, legendNormal, Color.White),
                    new RenderText(GameDriver.WindowFont, legendBonus, new Color(100,250,100)),
                    new RenderText(GameDriver.WindowFont, legendDamage, new Color(250,100,100)),
                    new RenderText(GameDriver.WindowFont, legendBlocked, new Color(100,100,250)),
                    new RenderText(GameDriver.WindowFont, legendIgnored, Color.Gray)
                }
            };
            WindowContentGrid helpTextWindowContentGrid = new WindowContentGrid(textToRender, 2);
            battleUI.GenerateHelpTextWindow(helpTextWindowContentGrid);

            SetupAttackerWindows(attackerSlice);
            SetupDefenderWindows(defenderSlice);
        }

        private void SetupAttackerWindows(MapSlice attackerSlice)
        {
            Color attackerWindowColor = MapHudGenerator.DetermineTeamColor(attacker.UnitTeam);
            battleUI.GenerateAttackerPortraitWindow(attackerWindowColor, attacker.LargePortrait);

            Vector2 portraitWidthOverride = new Vector2(battleUI.AttackerPortraitWindow.Width, 0);
            battleUI.GenerateAttackerLabelWindow(attackerWindowColor, portraitWidthOverride, attacker.Id);
            battleUI.GenerateAttackerHpWindow(attackerWindowColor, portraitWidthOverride, attacker, HpBarHeight);
            battleUI.GenerateAttackerAtkWindow(attackerWindowColor, portraitWidthOverride, attacker.Stats.Atk);

            bool attackerInRange = CoordinatesAreInRange(attacker.MapEntity.MapCoordinates,
                defender.MapEntity.MapCoordinates, attacker.Stats.AtkRange);
            battleUI.GenerateAttackerInRangeWindow(attackerWindowColor, portraitWidthOverride, attackerInRange);
            battleUI.GenerateAttackerDiceLabelWindow(attackerWindowColor);

            int terrainAttackBonus = battleUI.GenerateAttackerBonusWindow(attackerSlice, attackerWindowColor,
                portraitWidthOverride);
            attackerDice = new CombatDice(attacker.Stats.Atk, terrainAttackBonus, 3);
            battleUI.GenerateAttackerDiceWindow(attackerWindowColor, ref attackerDice);
        }

        private void SetupDefenderWindows(MapSlice defenderSlice)
        {
            Color defenderWindowColor = MapHudGenerator.DetermineTeamColor(defender.UnitTeam);
            battleUI.GenerateDefenderPortraitWindow(defenderWindowColor, defender.LargePortrait);

            Vector2 portraitWidthOverride = new Vector2(battleUI.DefenderPortraitWindow.Width, 0);
            battleUI.GenerateDefenderLabelWindow(defenderWindowColor, portraitWidthOverride, defender.Id);
            battleUI.GenerateDefenderHpWindow(defenderWindowColor, portraitWidthOverride, defender, HpBarHeight);
            battleUI.GenerateDefenderDefWindow(defenderWindowColor, portraitWidthOverride, defender.Stats.Def);

            bool defenderInRange = CoordinatesAreInRange(defender.MapEntity.MapCoordinates,
                attacker.MapEntity.MapCoordinates, defender.Stats.AtkRange);
            battleUI.GenerateDefenderRangeWindow(defenderWindowColor, portraitWidthOverride, defenderInRange);
            battleUI.GenerateDefenderDiceLabelWindow(defenderWindowColor);

            int terrainDefenseBonus =
                battleUI.GenerateDefenderBonusWindow(defenderSlice, defenderWindowColor, portraitWidthOverride);
            defenderDice = new CombatDice(defender.Stats.Def, terrainDefenseBonus, 3);
            battleUI.GenerateDefenderDiceWindow(defenderWindowColor, ref defenderDice);
        }

        //TODO use this to step through the combat steps
        public void ProceedToNextState()
        {
            if (CurrentState == BattleState.ResolveCombat)
            {
                CurrentState = 0;
                Trace.WriteLine("Resetting to initial combat state: " + CurrentState);
            }
            else
            {
                CurrentState++;
                Trace.WriteLine("Changing combat state: " + CurrentState);
            }
        }

        private static bool CoordinatesAreInRange(Vector2 sourcePosition, Vector2 targetPosition, IEnumerable<int> sourceRange)
        {
            /*Since distance is measured in horizontal and vertical steps, the absolute value of the difference of
             absolute positions should add up to the appropriate range.*/
            int horizontalDistance = Math.Abs(Math.Abs((int) targetPosition.X) -
                                              Math.Abs((int) sourcePosition.X));

            int verticalDistance = Math.Abs(Math.Abs((int) targetPosition.Y) -
                                            Math.Abs((int) sourcePosition.Y));
            return sourceRange.Any(range => horizontalDistance + verticalDistance == range);
        }

        public void RollDice()
        {
            if (!currentlyRolling)
            {
                currentlyRolling = true;
            }
        }

        public void StartCountingDice()
        {
            if (!currentlyCountingDice)
            {
                currentlyCountingDice = true;
            }
        }

        private void CountDice()
        {
            bool attackerInRange = CoordinatesAreInRange(attacker.MapEntity.MapCoordinates,
                defender.MapEntity.MapCoordinates, attacker.Stats.AtkRange);
            bool defenderInRange = CoordinatesAreInRange(defender.MapEntity.MapCoordinates,
                attacker.MapEntity.MapCoordinates, defender.Stats.AtkRange);

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
                }
                else if (defenderInRange && defenderSwords > 0 && attackerShields > 0)
                {
                    defenderDice.BlockNextDieWithValue(Die.FaceValue.Sword);
                    attackerDice.BlockNextDieWithValue(Die.FaceValue.Shield);
                }
                else
                {
                    //Don't count defender's attack dice if out of range
                    if (!defenderInRange) defenderDice.DisableAllDiceWithValue(Die.FaceValue.Sword);

                    currentlyCountingDice = false;
                }
            }
        }

        public void StartResolvingDamage()
        {
            if (!currentlyResolvingDamage)
            {
                currentlyResolvingDamage = true;
            }
        }

        private void ResolveDamage()
        {
            //TODO Calculate damage and reduce attacker and defender's HP bars point-by-point
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
                }
                else if (attackerDamage > 0)
                {
                    attackerDice.ResolveNextDieWithValue(Die.FaceValue.Sword);
                    defender.DamageUnit(1);
                }
                else if (defenderDamage > 0)
                {
                    defenderDice.ResolveNextDieWithValue(Die.FaceValue.Sword);
                    attacker.DamageUnit(1);
                    
                }
                else
                {
                    currentlyResolvingDamage = false;
                }
            }
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
                rollingCounter++;
                const int rollingFrames = 60;
                if (rollingCounter >= rollingFrames)
                {
                    rollingCounter = 0;
                    currentlyRolling = false;
                }

                const int renderDelay = 3;
                if (frameCounter % renderDelay == 0)
                {
                    attackerDice.RollDice();
                    defenderDice.RollDice();
                }
            }
            else if (currentlyCountingDice)
            {
                CountDice();
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