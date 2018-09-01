using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Combat;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.UI
{
    public class BattleUI : IUserInterface
    {
        private readonly Vector2 screenSize;

        //TODO Make this scale properly with resolution
        private static readonly Vector2 WindowEdgeBuffer = new Vector2(200, 200);

        private const int WindowSpacing = 5;
        private static readonly Color PositiveColor = new Color(30, 200, 30);
        private static readonly Color NegativeColor = new Color(250, 10, 10);
        private static readonly Color NeutralColor = new Color(200, 200, 200);


        //TODO decide if this should stay or be removed
        public Window DebugWindow { get; set; }

        public Window AttackerLabelWindow { get; private set; }
        public Window AttackerPortraitWindow { get; private set; }
        public Window AttackerHpWindow { get; private set; }
        public Window AttackerAtkWindow { get; private set; }
        public Window AttackerBonusWindow { get; private set; }
        public Window AttackerRangeWindow { get; private set; }
        public Window AttackerDiceLabelWindow { get; private set; }
        public Window AttackerDiceWindow { get; private set; }

        public Window DefenderLabelWindow { get; private set; }
        public Window DefenderPortraitWindow { get; private set; }
        public Window DefenderHpWindow { get; private set; }
        public Window DefenderDefWindow { get; private set; }
        public Window DefenderBonusWindow { get; private set; }
        public Window DefenderRangeWindow { get; private set; }
        public Window DefenderDiceLabelWindow { get; private set; }
        public Window DefenderDiceWindow { get; private set; }

        public Window HelpTextWindow { get; set; }
        public Window UserPromptWindow { get; set; }

        private bool visible;

        private readonly ITexture2D windowTexture;

        public BattleUI(Vector2 screenSize, ITexture2D windowTexture)
        {
            this.screenSize = screenSize;
            this.windowTexture = windowTexture;
            visible = true;
        }

        internal void GenerateHelpTextWindow(WindowContentGrid helpTextContent)
        {
            Color helpTextWindowColor = new Color(20, 20, 20, 200);
            HelpTextWindow = new Window("Help Window", windowTexture, helpTextContent, helpTextWindowColor);
        }

        internal void GenerateUserPromptWindow(WindowContentGrid promptTextContent, Vector2 sizeOverride)
        {
            Color promptWindowColor = new Color(40, 30, 40, 200);
            UserPromptWindow = new Window("User Prompt Window", windowTexture, promptTextContent, promptWindowColor,
                sizeOverride);
        }


        internal void GenerateAttackerDiceWindow(Color attackerWindowColor,
            ref CombatDice attackerDice)
        {
            IRenderable[,] diceWindowContent =
            {
                {attackerDice}
            };
            WindowContentGrid attackerDiceContentGrid = new WindowContentGrid(diceWindowContent, 1);
            AttackerDiceWindow = new Window("Dice Rolling Window", windowTexture, attackerDiceContentGrid,
                attackerWindowColor);
        }

        internal void GenerateAttackerDiceLabelWindow(Color attackerWindowColor)
        {
            AttackerDiceLabelWindow = new Window("Dice Label", windowTexture,
                new RenderText(GameDriver.WindowFont, "Attacking"), attackerWindowColor);
        }

        internal void GenerateAttackerInRangeWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            bool inRange)
        {
            IRenderable[,] attackerRangeContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "In Range: "),
                    new RenderText(GameDriver.WindowFont, inRange.ToString(), (inRange) ? PositiveColor : NegativeColor)
                }
            };
            WindowContentGrid attackerRangeContentGrid = new WindowContentGrid(attackerRangeContent, 1);
            AttackerRangeWindow = new Window("Attacker Range Info", windowTexture, attackerRangeContentGrid,
                attackerWindowColor, portraitWidthOverride);
        }

        internal int GenerateAttackerBonusWindow(MapSlice attackerSlice, Color attackerWindowColor,
            Vector2 portraitWidthOverride)
        {
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
                    new RenderText(GameDriver.WindowFont, "Bonus: "),
                    new RenderText(GameDriver.WindowFont, terrainAttackBonus,
                        (Convert.ToInt32(terrainAttackBonus) > 0) ? PositiveColor : NeutralColor)
                }
            };
            WindowContentGrid attackerBonusContentGrid = new WindowContentGrid(attackerBonusContent, 1);
            AttackerBonusWindow = new Window("Attacker Bonus Info", windowTexture, attackerBonusContentGrid,
                attackerWindowColor, portraitWidthOverride);

            return Convert.ToInt32(terrainAttackBonus);
        }

        internal void GenerateAttackerAtkWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            int atkValue)
        {
            IRenderable[,] attackerAtkContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "ATK: "),
                    new RenderText(GameDriver.WindowFont, atkValue.ToString())
                }
            };
            WindowContentGrid attackerAtkContentGrid = new WindowContentGrid(attackerAtkContent, 1);
            AttackerAtkWindow = new Window("Attacker ATK Info", windowTexture, attackerAtkContentGrid,
                attackerWindowColor, portraitWidthOverride);
        }

        internal void GenerateAttackerHpWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            GameUnit attacker, int hpBarHeight)
        {
            IRenderable[,] attackerHpContent = new IRenderable[1, 2];
            IRenderable hpLabel = new RenderText(GameDriver.WindowFont, "HP: ");
            Vector2 hpBarSize = new Vector2(attacker.LargePortrait.Width - hpLabel.Width, hpBarHeight);
            IRenderable hpBar = attacker.GetCombatHealthBar(hpBarSize);
            attackerHpContent[0, 0] = hpLabel;
            attackerHpContent[0, 1] = hpBar;
            WindowContentGrid attackerHpContentGrid = new WindowContentGrid(attackerHpContent, 1);
            AttackerHpWindow =
                new Window("Attacker HP Bar", windowTexture, attackerHpContentGrid, attackerWindowColor,
                    portraitWidthOverride);
        }

        internal void GenerateAttackerLabelWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            string attackerName)
        {
            IRenderable attackerLabelText = new RenderText(GameDriver.WindowFont, attackerName);
            AttackerLabelWindow = new Window("Attacker Label", windowTexture, attackerLabelText,
                attackerWindowColor, portraitWidthOverride);
        }

        internal void GenerateAttackerPortraitWindow(Color attackerWindowColor, ITexture2D attackerPortrait)
        {
            IRenderable attackerPortraitContent =
                new SpriteAtlas(attackerPortrait, attackerPortrait.Height, 1);
            AttackerPortraitWindow =
                new Window("Attacker Portrait", windowTexture, attackerPortraitContent, attackerWindowColor);
        }


        internal void GenerateDefenderDiceWindow(Color defenderWindowColor, ref CombatDice defenderDice)
        {
            IRenderable[,] diceWindowContent =
            {
                {defenderDice}
            };

            WindowContentGrid defenderDiceContentGrid = new WindowContentGrid(diceWindowContent, 1);
            DefenderDiceWindow = new Window("Dice Rolling Window", windowTexture, defenderDiceContentGrid,
                defenderWindowColor);
        }

        internal void GenerateDefenderDiceLabelWindow(Color defenderWindowColor)
        {
            DefenderDiceLabelWindow = new Window("Dice Label", windowTexture,
                new RenderText(GameDriver.WindowFont, "Defending"), defenderWindowColor);
        }

        internal void GenerateDefenderRangeWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            bool inRange)
        {
            IRenderable[,] defenderRangeContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "In Range: "),
                    new RenderText(GameDriver.WindowFont, inRange.ToString(), (inRange) ? PositiveColor : NegativeColor)
                }
            };

            WindowContentGrid defenderRangeContentGrid = new WindowContentGrid(defenderRangeContent, 1);
            DefenderRangeWindow = new Window("Defender Range Info", windowTexture, defenderRangeContentGrid,
                defenderWindowColor, portraitWidthOverride);
        }

        internal int GenerateDefenderBonusWindow(MapSlice defenderSlice, Color defenderWindowColor,
            Vector2 portraitWidthOverride)
        {
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
                    new RenderText(GameDriver.WindowFont, "Bonus: "),
                    new RenderText(GameDriver.WindowFont, terrainDefenseBonus,
                        (Convert.ToInt32(terrainDefenseBonus) > 0) ? PositiveColor : NeutralColor)
                }
            };
            WindowContentGrid defenderBonusContentGrid = new WindowContentGrid(defenderBonusContent, 1);
            DefenderBonusWindow = new Window("Defender Bonus Info", windowTexture, defenderBonusContentGrid,
                defenderWindowColor, portraitWidthOverride);

            return Convert.ToInt32(terrainDefenseBonus);
        }

        internal void GenerateDefenderDefWindow(Color defenderWindowColor, Vector2 portraitWidthOverride, int defValue)
        {
            IRenderable[,] defenderAtkContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, "DEF: "),
                    new RenderText(GameDriver.WindowFont, defValue.ToString())
                }
            };
            WindowContentGrid defenderAtkContentGrid = new WindowContentGrid(defenderAtkContent, 1);
            DefenderDefWindow = new Window("Defender DEF Info", windowTexture, defenderAtkContentGrid,
                defenderWindowColor, portraitWidthOverride);
        }

        internal void GenerateDefenderHpWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            GameUnit defender, int hpBarHeight)
        {
            IRenderable[,] defenderHpContent = new IRenderable[1, 2];
            IRenderable hpLabel = new RenderText(GameDriver.WindowFont, "HP: ");
            Vector2 hpBarSize = new Vector2(defender.LargePortrait.Width - hpLabel.Width, hpBarHeight);
            IRenderable hpBar = defender.GetCombatHealthBar(hpBarSize);
            defenderHpContent[0, 0] = hpLabel;
            defenderHpContent[0, 1] = hpBar;
            WindowContentGrid defenderHpContentGrid = new WindowContentGrid(defenderHpContent, 1);
            DefenderHpWindow =
                new Window("Defender HP Bar", windowTexture, defenderHpContentGrid, defenderWindowColor,
                    portraitWidthOverride);
        }

        internal void GenerateDefenderLabelWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            string defenderName)
        {
            IRenderable defenderLabelText = new RenderText(GameDriver.WindowFont, defenderName);
            DefenderLabelWindow = new Window("Defender Label", windowTexture, defenderLabelText,
                defenderWindowColor, portraitWidthOverride);
        }

        internal void GenerateDefenderPortraitWindow(Color defenderWindowColor, ITexture2D defenderPortrait)
        {
            IRenderable defenderPortraitContent =
                new SpriteAtlas(defenderPortrait, defenderPortrait.Height, 1);
            DefenderPortraitWindow =
                new Window("Defender Portrait", windowTexture, defenderPortraitContent, defenderWindowColor);
        }


        //Window Render Positions

        private Vector2 HelpTextWindowPosition()
        {
            //Top-left
            return WindowEdgeBuffer;
        }

        private Vector2 UserPromptWindowPosition()
        {
            return new Vector2(GameDriver.ScreenSize.X / 2 - (float) UserPromptWindow.Width / 2,
                AttackerAtkWindowPosition().Y);
        }

        #region Attacker

        private Vector2 AttackerLabelWindowPosition()
        {
            //Top-left, below help window
            return new Vector2(WindowEdgeBuffer.X, HelpTextWindowPosition().Y + HelpTextWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerPortraitWindowPosition()
        {
            //Anchored beneath below label window
            Vector2 attackerLabelWindowPosition = AttackerLabelWindowPosition();

            return new Vector2(attackerLabelWindowPosition.X,
                attackerLabelWindowPosition.Y + AttackerLabelWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerHpWindowPosition()
        {
            //Anchored beneath portrait window
            Vector2 attackerPortraitWindowPosition = AttackerPortraitWindowPosition();

            return new Vector2(attackerPortraitWindowPosition.X,
                attackerPortraitWindowPosition.Y + AttackerPortraitWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerAtkWindowPosition()
        {
            //Anchored beneath HP window
            Vector2 attackerHpWindowPosition = AttackerHpWindowPosition();

            return new Vector2(attackerHpWindowPosition.X,
                attackerHpWindowPosition.Y + AttackerHpWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerBonusWindowPosition()
        {
            //Anchored beneath ATK window
            Vector2 attackerAtkWindowPosition = AttackerAtkWindowPosition();

            return new Vector2(attackerAtkWindowPosition.X,
                attackerAtkWindowPosition.Y + AttackerAtkWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerRangeWindowPosition()
        {
            //Anchored beneath Bonus window
            Vector2 attackerBonusWindowPosition = AttackerBonusWindowPosition();

            return new Vector2(attackerBonusWindowPosition.X,
                attackerBonusWindowPosition.Y + AttackerBonusWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerDiceLabelPosition()
        {
            //Anchored right of class label window
            Vector2 attackerLabelWindowPosition = AttackerLabelWindowPosition();

            return new Vector2(attackerLabelWindowPosition.X + AttackerLabelWindow.Width + WindowSpacing,
                attackerLabelWindowPosition.Y);
        }

        private Vector2 AttackerDicePosition()
        {
            //Anchored right of class portrait window
            Vector2 attackerPortraitWindowPosition = AttackerPortraitWindowPosition();

            return new Vector2(attackerPortraitWindowPosition.X + AttackerPortraitWindow.Width + WindowSpacing,
                attackerPortraitWindowPosition.Y);
        }

        #endregion Attacker

        #region Defender

        private Vector2 DefenderLabelWindowPosition()
        {
            //Top-left, below help window
            return new Vector2(screenSize.X - DefenderLabelWindow.Width - WindowEdgeBuffer.X,
                HelpTextWindowPosition().Y + HelpTextWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderPortraitWindowPosition()
        {
            //Anchored beneath below label window
            Vector2 defenderLabelWindowPosition = DefenderLabelWindowPosition();

            return new Vector2(screenSize.X - DefenderPortraitWindow.Width - WindowEdgeBuffer.X,
                defenderLabelWindowPosition.Y + DefenderLabelWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderHpWindowPosition()
        {
            //Anchored beneath portrait window
            Vector2 defenderPortraitWindowPosition = DefenderPortraitWindowPosition();

            return new Vector2(screenSize.X - DefenderHpWindow.Width - WindowEdgeBuffer.X,
                defenderPortraitWindowPosition.Y + DefenderPortraitWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderAtkWindowPosition()
        {
            //Anchored beneath HP window
            Vector2 defenderHpWindowPosition = DefenderHpWindowPosition();

            return new Vector2(screenSize.X - DefenderDefWindow.Width - WindowEdgeBuffer.X,
                defenderHpWindowPosition.Y + DefenderHpWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderBonusWindowPosition()
        {
            //Anchored beneath ATK window
            Vector2 defenderAtkWindowPosition = DefenderAtkWindowPosition();

            return new Vector2(screenSize.X - DefenderBonusWindow.Width - WindowEdgeBuffer.X,
                defenderAtkWindowPosition.Y + DefenderHpWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderRangeWindowPosition()
        {
            //Anchored beneath Bonus window
            Vector2 defenderBonusWindowPosition = DefenderBonusWindowPosition();

            return new Vector2(screenSize.X - DefenderRangeWindow.Width - WindowEdgeBuffer.X,
                defenderBonusWindowPosition.Y + DefenderBonusWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderDiceLabelPosition()
        {
            //Anchored right of class label window
            Vector2 defenderLabelWindowPosition = DefenderLabelWindowPosition();

            return new Vector2(
                screenSize.X - DefenderDiceLabelWindow.Width - DefenderLabelWindow.Width - WindowSpacing -
                WindowEdgeBuffer.X, defenderLabelWindowPosition.Y);
        }

        private Vector2 DefenderDicePosition()
        {
            //Anchored right of class portrait window
            Vector2 defenderPortraitWindowPosition = DefenderPortraitWindowPosition();

            return new Vector2(
                screenSize.X - DefenderDiceWindow.Width - DefenderPortraitWindow.Width - WindowSpacing -
                WindowEdgeBuffer.X, defenderPortraitWindowPosition.Y);
        }

        #endregion Defender

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (HelpTextWindow != null)
            {
                HelpTextWindow.Draw(spriteBatch, HelpTextWindowPosition());

                if (AttackerPortraitWindow != null)
                {
                    AttackerLabelWindow.Draw(spriteBatch, AttackerLabelWindowPosition());
                    AttackerPortraitWindow.Draw(spriteBatch, AttackerPortraitWindowPosition());
                    AttackerHpWindow.Draw(spriteBatch, AttackerHpWindowPosition());
                    AttackerAtkWindow.Draw(spriteBatch, AttackerAtkWindowPosition());
                    AttackerBonusWindow.Draw(spriteBatch, AttackerBonusWindowPosition());
                    AttackerRangeWindow.Draw(spriteBatch, AttackerRangeWindowPosition());
                    AttackerDiceLabelWindow.Draw(spriteBatch, AttackerDiceLabelPosition());
                    AttackerDiceWindow.Draw(spriteBatch, AttackerDicePosition());
                }

                if (DefenderPortraitWindow != null)
                {
                    DefenderLabelWindow.Draw(spriteBatch, DefenderLabelWindowPosition());
                    DefenderPortraitWindow.Draw(spriteBatch, DefenderPortraitWindowPosition());
                    DefenderHpWindow.Draw(spriteBatch, DefenderHpWindowPosition());
                    DefenderDefWindow.Draw(spriteBatch, DefenderAtkWindowPosition());
                    DefenderBonusWindow.Draw(spriteBatch, DefenderBonusWindowPosition());
                    DefenderRangeWindow.Draw(spriteBatch, DefenderRangeWindowPosition());
                    DefenderDiceLabelWindow.Draw(spriteBatch, DefenderDiceLabelPosition());
                    DefenderDiceWindow.Draw(spriteBatch, DefenderDicePosition());
                }
            }

            if (UserPromptWindow != null)
            {
                UserPromptWindow.Draw(spriteBatch, UserPromptWindowPosition());
            }
        }
    }
}