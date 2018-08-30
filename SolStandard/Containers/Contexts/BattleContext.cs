using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content.Combat;
using SolStandard.Map.Elements.Cursor;

namespace SolStandard.Containers.Contexts
{
    public class BattleContext
    {
        //TODO Implement procedure through steps via button presses in response to Window prompts from BattleUI
        public enum BattleState
        {
            Start,
            RollDice,
            CalculateDamage,
            DealDamage,
            End
        }

        private readonly BattleUI battleUI;
        private const int HpBarHeight = 20;

        private CombatDice attackerDice;
        private CombatDice defenderDice;

        private int frameCounter;
        private bool currentlyRolling;
        private int rollingCounter;

        public BattleState CurrentState { get; private set; }

        private GameUnit attacker;
        private GameUnit defender;

        public BattleContext(BattleUI battleUI)
        {
            this.battleUI = battleUI;
            frameCounter = 0;
            currentlyRolling = false;
            rollingCounter = 0;
            CurrentState = BattleState.Start;
            //TODO determine CombatFlow
        }

        public void StartNewCombat(GameUnit attacker, MapSlice attackerSlice, GameUnit defender, MapSlice defenderSlice)
        {
            this.attacker = attacker;
            this.defender = defender;

            const string helpText = "INFO: Swords deal 1 damage. Shields block swords. Blanks are ignored.";
            battleUI.GenerateHelpTextWindow(helpText);

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
            if (CurrentState == BattleState.End)
            {
                CurrentState = 0;
                Trace.WriteLine("Resetting to initial state: " + CurrentState);
            }
            else
            {
                CurrentState++;
                Trace.WriteLine("Changing state: " + CurrentState);
            }
        }

        private bool CoordinatesAreInRange(Vector2 sourcePosition, Vector2 targetPosition, IEnumerable<int> sourceRange)
        {
            //TODO Write unit tests for this
            /*Since distance is measured in horizontal and vertical steps, the absolute value of the difference of
             absolute positions should add up to the appropriate range.*/
            int horizontalDistance = Math.Abs(Math.Abs((int) targetPosition.X) -
                                              Math.Abs((int) sourcePosition.X));

            int verticalDistance = Math.Abs(Math.Abs((int) targetPosition.Y) -
                                            Math.Abs((int) sourcePosition.Y));
            return sourceRange.Any(range => horizontalDistance + verticalDistance == range);
        }

        //FIXME Find a more appropriate way to roll the dice and actually track the values
        public void RollDice()
        {
            if (!currentlyRolling)
            {
                currentlyRolling = true;
            }
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
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            frameCounter++;
            UpdateDice();
            battleUI.Draw(spriteBatch);
        }
    }
}