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
using SolStandard.Utility.Monogame;

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
        private readonly ITexture2D windowTexture;
        private const int HpBarHeight = 20;

        private CombatDice attackerDice;
        private CombatDice defenderDice;

        private int frameCounter;
        private bool currentlyRolling;
        private int rollingCounter;

        public BattleState CurrentState { get; private set; }

        private GameUnit attacker;
        private GameUnit defender;

        public BattleContext(BattleUI battleUI, ITexture2D windowTexture)
        {
            this.battleUI = battleUI;
            this.windowTexture = windowTexture;
            frameCounter = 0;
            currentlyRolling = false;
            rollingCounter = 0;
            CurrentState = BattleState.Start;
            //TODO determine CombatFlow
        }

        public void StartNewCombat(GameUnit attacker, MapSlice attackerSlice,
            GameUnit defender, MapSlice defenderSlice)
        {
            this.attacker = attacker;
            this.defender = defender;

            //Help Window
            IRenderable textToRender = new RenderText(GameDriver.WindowFont,
                "INFO: Swords deal 1 damage. Shields block swords. Blanks are ignored.");
            battleUI.HelpTextWindow =
                new Window("Help Window", windowTexture, textToRender, new Color(30, 30, 30, 150));

            SetupAttackerWindows(attackerSlice);
            SetupDefenderWindows(defenderSlice);
        }

        private void SetupAttackerWindows(MapSlice attackerSlice)
        {
            Color attackerWindowColor = MapHudGenerator.DetermineTeamColor(attacker.UnitTeam);


            //Portrait Window
            IRenderable attackerPortrait =
                new SpriteAtlas(attacker.LargePortrait, attacker.LargePortrait.Height, 1);
            battleUI.AttackerPortraitWindow =
                new Window("Attacker Portrait", windowTexture, attackerPortrait, attackerWindowColor);


            Vector2 portraitWidthOverride = new Vector2(battleUI.AttackerPortraitWindow.Width, 0);

            //Name Label Window
            IRenderable attackerLabelText = new RenderText(GameDriver.WindowFont, attacker.Id);
            battleUI.AttackerLabelWindow = new Window("Attacker Label", windowTexture, attackerLabelText,
                attackerWindowColor, portraitWidthOverride);

            //HP Window
            IRenderable[,] attackerHpContent = new IRenderable[1, 2];
            IRenderable hpLabel = new RenderText(GameDriver.WindowFont, "HP:");
            Vector2 hpBarSize = new Vector2(attacker.LargePortrait.Width - hpLabel.Width, HpBarHeight);
            IRenderable hpBar = attacker.GetCustomHealthBar(hpBarSize);
            attackerHpContent[0, 0] = hpLabel;
            attackerHpContent[0, 1] = hpBar;
            WindowContentGrid attackerHpContentGrid = new WindowContentGrid(attackerHpContent, 1);
            battleUI.AttackerHpWindow =
                new Window("Attacker HP Bar", windowTexture, attackerHpContentGrid, attackerWindowColor,
                    portraitWidthOverride);

            //ATK Window
            IRenderable[,] attackerAtkContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "ATK:"),
                    new RenderText(GameDriver.WindowFont, attacker.Stats.Atk.ToString())
                }
            };
            WindowContentGrid attackerAtkContentGrid = new WindowContentGrid(attackerAtkContent, 1);
            battleUI.AttackerAtkWindow = new Window("Attacker ATK Info", windowTexture, attackerAtkContentGrid,
                attackerWindowColor, portraitWidthOverride);

            //Bonus Window
            string terrainAttackBonus = "0";
            if (attackerSlice.GeneralEntity != null)
            {
                if (attackerSlice.GeneralEntity.TiledProperties.ContainsKey("Stat") &&
                    attackerSlice.GeneralEntity.TiledProperties["Stat"] == "ATK")
                {
                    if (attackerSlice.GeneralEntity.TiledProperties.ContainsKey("Modifier"))
                    {
                        terrainAttackBonus = attackerSlice.GeneralEntity.TiledProperties["Modifier"];
                    }
                }
            }

            IRenderable[,] attackerBonusContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "Bonus:"),
                    new RenderText(GameDriver.WindowFont, terrainAttackBonus)
                }
            };
            WindowContentGrid attackerBonusContentGrid = new WindowContentGrid(attackerBonusContent, 1);
            battleUI.AttackerBonusWindow = new Window("Attacker Bonus Info", windowTexture, attackerBonusContentGrid,
                attackerWindowColor, portraitWidthOverride);

            //AttackerRangeWindow
            string attackerInRange = CoordinatesAreInRange(attacker.MapEntity.MapCoordinates,
                defender.MapEntity.MapCoordinates, attacker.Stats.AtkRange).ToString();
            IRenderable[,] attackerRangeContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "In Range:"),
                    new RenderText(GameDriver.WindowFont, attackerInRange)
                }
            };
            WindowContentGrid attackerRangeContentGrid = new WindowContentGrid(attackerRangeContent, 1);
            battleUI.AttackerRangeWindow = new Window("Attacker Range Info", windowTexture, attackerRangeContentGrid,
                attackerWindowColor, portraitWidthOverride);

            //Dice Label Window
            battleUI.AttackerDiceLabelWindow = new Window("Dice Label", windowTexture,
                new RenderText(GameDriver.WindowFont, "Attacking"), attackerWindowColor);

            //Dice Content Window
            attackerDice = new CombatDice(attacker.Stats.Atk, int.Parse(terrainAttackBonus), 3);
            IRenderable[,] diceWindowContent =
            {
                {new RenderText(GameDriver.WindowFont, "PLACEHOLDER\nDICE WILL GO HERE")},
                {attackerDice}
            };
            WindowContentGrid attackerDiceContentGrid = new WindowContentGrid(diceWindowContent, 1);
            battleUI.AttackerDiceWindow = new Window("Dice Rolling Window", windowTexture, attackerDiceContentGrid,
                attackerWindowColor);
        }

        //FIXME This is almost entirely duplicated logic and it's ugly; figure out how to DRY this
        private void SetupDefenderWindows(MapSlice defenderSlice)
        {
            Color defenderWindowColor = MapHudGenerator.DetermineTeamColor(defender.UnitTeam);


            //Portrait Window
            IRenderable defenderPortrait =
                new SpriteAtlas(defender.LargePortrait, defender.LargePortrait.Height, 1);
            battleUI.DefenderPortraitWindow =
                new Window("Defender Portrait", windowTexture, defenderPortrait, defenderWindowColor);


            Vector2 portraitWidthOverride = new Vector2(battleUI.DefenderPortraitWindow.Width, 0);

            //Name Label Window
            IRenderable defenderLabelText = new RenderText(GameDriver.WindowFont, defender.Id);
            battleUI.DefenderLabelWindow = new Window("Defender Label", windowTexture, defenderLabelText,
                defenderWindowColor, portraitWidthOverride);

            //HP Window
            IRenderable[,] defenderHpContent = new IRenderable[1, 2];
            IRenderable hpLabel = new RenderText(GameDriver.WindowFont, "HP:");
            Vector2 hpBarSize = new Vector2(defender.LargePortrait.Width - hpLabel.Width, HpBarHeight);
            IRenderable hpBar = defender.GetCustomHealthBar(hpBarSize);
            defenderHpContent[0, 0] = hpLabel;
            defenderHpContent[0, 1] = hpBar;
            WindowContentGrid defenderHpContentGrid = new WindowContentGrid(defenderHpContent, 1);
            battleUI.DefenderHpWindow =
                new Window("Defender HP Bar", windowTexture, defenderHpContentGrid, defenderWindowColor,
                    portraitWidthOverride);

            //DEF Window
            IRenderable[,] defenderAtkContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "DEF:"),
                    new RenderText(GameDriver.WindowFont, defender.Stats.Def.ToString())
                }
            };
            WindowContentGrid defenderAtkContentGrid = new WindowContentGrid(defenderAtkContent, 1);
            battleUI.DefenderAtkWindow = new Window("Defender DEF Info", windowTexture, defenderAtkContentGrid,
                defenderWindowColor, portraitWidthOverride);

            //Bonus Window
            string terrainDefenseBonus = "0";
            if (defenderSlice.GeneralEntity != null)
            {
                if (defenderSlice.GeneralEntity.TiledProperties.ContainsKey("Stat") &&
                    defenderSlice.GeneralEntity.TiledProperties["Stat"] == "DEF")
                {
                    if (defenderSlice.GeneralEntity.TiledProperties.ContainsKey("Modifier"))
                    {
                        terrainDefenseBonus = defenderSlice.GeneralEntity.TiledProperties["Modifier"];
                    }
                }
            }

            IRenderable[,] defenderBonusContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "Bonus:"),
                    new RenderText(GameDriver.WindowFont, terrainDefenseBonus)
                }
            };
            WindowContentGrid defenderBonusContentGrid = new WindowContentGrid(defenderBonusContent, 1);
            battleUI.DefenderBonusWindow = new Window("Defender Bonus Info", windowTexture, defenderBonusContentGrid,
                defenderWindowColor, portraitWidthOverride);

            //DefenderRangeWindow
            string defenderInRange = CoordinatesAreInRange(defender.MapEntity.MapCoordinates,
                attacker.MapEntity.MapCoordinates, defender.Stats.AtkRange).ToString();

            IRenderable[,] defenderRangeContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "In Range:"),
                    new RenderText(GameDriver.WindowFont, defenderInRange)
                }
            };

            WindowContentGrid defenderRangeContentGrid = new WindowContentGrid(defenderRangeContent, 1);
            battleUI.DefenderRangeWindow = new Window("Defender Range Info", windowTexture, defenderRangeContentGrid,
                defenderWindowColor, portraitWidthOverride);

            //Dice Label Window
            battleUI.DefenderDiceLabelWindow = new Window("Dice Label", windowTexture,
                new RenderText(GameDriver.WindowFont, "Defending"), defenderWindowColor);

            //Dice Content Window
            defenderDice = new CombatDice(defender.Stats.Def, int.Parse(terrainDefenseBonus), 3);

            IRenderable[,] diceWindowContent =
            {
                {new RenderText(GameDriver.WindowFont, "PLACEHOLDER\nDICE WILL GO HERE")},
                {defenderDice}
            };

            WindowContentGrid defenderDiceContentGrid = new WindowContentGrid(diceWindowContent, 1);
            battleUI.DefenderDiceWindow = new Window("Dice Rolling Window", windowTexture, defenderDiceContentGrid,
                defenderWindowColor);
        }

        //TODO use this to step through the combat steps
        private void ProceedToNextState()
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
//Since distance is measured in horizontal and vertical steps,
//the absolute value of the difference of absolute positions should add up to the appropriate range. 
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