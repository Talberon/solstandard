using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Combat;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.View
{
    public class BattleUI : IUserInterface
    {
        //TODO Make this scale properly with resolution
        //TODO Calculate total content size to center UI
        private static readonly Vector2 WindowEdgeBuffer = new Vector2(200, 150);

        private const int WindowSpacing = 5;

        public Window AttackerLabelWindow { get; private set; }
        public Window AttackerPortraitWindow { get; private set; }
        public Window AttackerClassWindow { get; private set; }
        public Window AttackerHpWindow { get; private set; }
        public Window AttackerAtkWindow { get; private set; }
        public Window AttackerBonusWindow { get; private set; }
        public Window AttackerRangeWindow { get; private set; }
        public Window AttackerDiceLabelWindow { get; private set; }
        public Window AttackerDiceWindow { get; private set; }

        public Window DefenderLabelWindow { get; private set; }
        public Window DefenderPortraitWindow { get; private set; }
        public Window DefenderClassWindow { get; private set; }
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

        public BattleUI()
        {
            windowTexture = AssetManager.WindowTexture;
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
                new RenderText(AssetManager.WindowFont, "Attacking"), attackerWindowColor);
        }

        internal void GenerateAttackerInRangeWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            bool inRange)
        {
            IRenderable[,] attackerRangeContent =
            {
                {
                    UnitStatistics.GetSpriteAtlas(StatIcons.AtkRange),
                    new RenderText(AssetManager.WindowFont, "In Range: "),
                    new RenderText(AssetManager.WindowFont, inRange.ToString(),
                        (inRange) ? GameContext.PositiveColor : GameContext.NegativeColor)
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
            if (attackerSlice.TerrainEntity != null)
            {
                if (attackerSlice.TerrainEntity.TiledProperties.ContainsKey("Stat") &&
                    attackerSlice.TerrainEntity.TiledProperties["Stat"] == "ATK")
                {
                    if (attackerSlice.TerrainEntity.TiledProperties.ContainsKey("Modifier"))
                    {
                        terrainAttackBonus = attackerSlice.TerrainEntity.TiledProperties["Modifier"];
                    }
                }
            }

            IRenderable[,] attackerBonusContent =
            {
                {
                    UnitStatistics.GetSpriteAtlas(StatIcons.BonusAtk),
                    new RenderText(AssetManager.WindowFont, "Bonus: "),
                    new RenderText(AssetManager.WindowFont, terrainAttackBonus,
                        (Convert.ToInt32(terrainAttackBonus) > 0)
                            ? GameContext.PositiveColor
                            : GameContext.NeutralColor)
                }
            };
            WindowContentGrid attackerBonusContentGrid = new WindowContentGrid(attackerBonusContent, 1);
            AttackerBonusWindow = new Window("Attacker Bonus Info", windowTexture, attackerBonusContentGrid,
                attackerWindowColor, portraitWidthOverride);

            return Convert.ToInt32(terrainAttackBonus);
        }

        internal void GenerateAttackerAtkWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            UnitStatistics attackerStats)
        {
            IRenderable[,] attackerAtkContent =
            {
                {
                    UnitStatistics.GetSpriteAtlas(StatIcons.Atk),
                    new RenderText(AssetManager.WindowFont, "ATK: "),
                    new RenderText(
                        AssetManager.WindowFont,
                        attackerStats.Atk.ToString(),
                        UnitStatistics.DetermineStatColor(attackerStats.Atk, attackerStats.BaseAtk)
                    )
                }
            };
            WindowContentGrid attackerAtkContentGrid = new WindowContentGrid(attackerAtkContent, 1);
            AttackerAtkWindow = new Window("Attacker ATK Info", windowTexture, attackerAtkContentGrid,
                attackerWindowColor, portraitWidthOverride);
        }

        internal void GenerateAttackerHpWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            GameUnit attacker, int hpBarHeight)
        {
            IRenderable hpIcon = new SpriteAtlas(AssetManager.StatIcons, new Vector2(GameDriver.CellSize), 1);
            IRenderable hpLabel = new RenderText(AssetManager.WindowFont, "HP: ");
            Vector2 hpBarSize = new Vector2(attacker.LargePortrait.Width - hpLabel.Width - hpIcon.Width, hpBarHeight);
            IRenderable hpBar = attacker.GetCombatHealthBar(hpBarSize);
            IRenderable[,] attackerHpContent =
            {
                {hpIcon, hpLabel, hpBar}
            };
            WindowContentGrid attackerHpContentGrid = new WindowContentGrid(attackerHpContent, 1);
            AttackerHpWindow =
                new Window("Attacker HP Bar", windowTexture, attackerHpContentGrid, attackerWindowColor,
                    portraitWidthOverride);
        }

        internal void GenerateAttackerLabelWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            string attackerName)
        {
            IRenderable attackerLabelText = new RenderText(AssetManager.HeaderFont, attackerName);
            AttackerLabelWindow = new Window("Attacker Name Label", windowTexture, attackerLabelText,
                attackerWindowColor, portraitWidthOverride);
        }

        internal void GenerateAttackerClassWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            string attackerClass)
        {
            IRenderable attackerLabelText = new RenderText(AssetManager.WindowFont, "Class: " + attackerClass);
            AttackerClassWindow = new Window("Attacker Class Label", windowTexture, attackerLabelText,
                attackerWindowColor, portraitWidthOverride);
        }

        internal void GenerateAttackerPortraitWindow(Color attackerWindowColor, IRenderable attackerPortrait)
        {
            AttackerPortraitWindow =
                new Window("Attacker Portrait", windowTexture, attackerPortrait, attackerWindowColor);
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
                new RenderText(AssetManager.WindowFont, "Defending"), defenderWindowColor);
        }

        internal void GenerateDefenderRangeWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            bool inRange)
        {
            IRenderable[,] defenderRangeContent =
            {
                {
                    UnitStatistics.GetSpriteAtlas(StatIcons.AtkRange),
                    new RenderText(AssetManager.WindowFont, "In Range: "),
                    new RenderText(AssetManager.WindowFont, inRange.ToString(),
                        (inRange) ? GameContext.PositiveColor : GameContext.NegativeColor)
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
            if (defenderSlice.TerrainEntity != null)
            {
                if (defenderSlice.TerrainEntity.TiledProperties.ContainsKey("Stat") &&
                    defenderSlice.TerrainEntity.TiledProperties["Stat"] == "DEF")
                {
                    if (defenderSlice.TerrainEntity.TiledProperties.ContainsKey("Modifier"))
                    {
                        terrainDefenseBonus = defenderSlice.TerrainEntity.TiledProperties["Modifier"];
                    }
                }
            }

            IRenderable[,] defenderBonusContent =
            {
                {
                    UnitStatistics.GetSpriteAtlas(StatIcons.BonusDef),
                    new RenderText(AssetManager.WindowFont, "Bonus: "),
                    new RenderText(AssetManager.WindowFont, terrainDefenseBonus,
                        (Convert.ToInt32(terrainDefenseBonus) > 0)
                            ? GameContext.PositiveColor
                            : GameContext.NeutralColor)
                }
            };
            WindowContentGrid defenderBonusContentGrid = new WindowContentGrid(defenderBonusContent, 1);
            DefenderBonusWindow = new Window("Defender Bonus Info", windowTexture, defenderBonusContentGrid,
                defenderWindowColor, portraitWidthOverride);

            return Convert.ToInt32(terrainDefenseBonus);
        }

        internal void GenerateDefenderDefWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            UnitStatistics defenderStats)
        {
            IRenderable[,] defenderAtkContent =
            {
                {
                    new SpriteAtlas(AssetManager.StatIcons, new Vector2(GameDriver.CellSize), 3),
                    new RenderText(AssetManager.WindowFont, "DEF: "),
                    new RenderText(
                        AssetManager.WindowFont,
                        defenderStats.Def.ToString(),
                        UnitStatistics.DetermineStatColor(defenderStats.Def, defenderStats.BaseDef)
                    )
                }
            };
            WindowContentGrid defenderAtkContentGrid = new WindowContentGrid(defenderAtkContent, 1);
            DefenderDefWindow = new Window("Defender DEF Info", windowTexture, defenderAtkContentGrid,
                defenderWindowColor, portraitWidthOverride);
        }

        internal void GenerateDefenderHpWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            GameUnit defender, int hpBarHeight)
        {
            IRenderable hpIcon = new SpriteAtlas(AssetManager.StatIcons, new Vector2(GameDriver.CellSize), 1);
            IRenderable hpLabel = new RenderText(AssetManager.WindowFont, "HP: ");
            Vector2 hpBarSize = new Vector2(defender.LargePortrait.Width - hpLabel.Width - hpIcon.Width, hpBarHeight);
            IRenderable hpBar = defender.GetCombatHealthBar(hpBarSize);
            IRenderable[,] defenderHpContent =
            {
                {hpIcon, hpLabel, hpBar}
            };
            WindowContentGrid defenderHpContentGrid = new WindowContentGrid(defenderHpContent, 1);
            DefenderHpWindow =
                new Window("Defender HP Bar", windowTexture, defenderHpContentGrid, defenderWindowColor,
                    portraitWidthOverride);
        }

        internal void GenerateDefenderLabelWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            string defenderName)
        {
            IRenderable defenderLabelText = new RenderText(AssetManager.HeaderFont, defenderName);
            DefenderLabelWindow = new Window("Defender Name Label", windowTexture, defenderLabelText,
                defenderWindowColor, portraitWidthOverride);
        }

        internal void GenerateDefenderClassWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            string defenderClass)
        {
            IRenderable attackerLabelText = new RenderText(AssetManager.WindowFont, "Class: " + defenderClass);
            DefenderClassWindow = new Window("Defender Class Label", windowTexture, attackerLabelText,
                attackerWindowColor, portraitWidthOverride);
        }

        internal void GenerateDefenderPortraitWindow(Color defenderWindowColor, IRenderable defenderPortrait)
        {
            DefenderPortraitWindow =
                new Window("Defender Portrait", windowTexture, defenderPortrait, defenderWindowColor);
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

        private Vector2 AttackerClassWindowPosition()
        {
            //Anchored beneath portrait window
            Vector2 attackerPortraitWindowPosition = AttackerPortraitWindowPosition();

            return new Vector2(attackerPortraitWindowPosition.X,
                attackerPortraitWindowPosition.Y + AttackerPortraitWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerHpWindowPosition()
        {
            //Anchored beneath class window
            Vector2 attackerClassWindowPosition = AttackerClassWindowPosition();

            return new Vector2(attackerClassWindowPosition.X,
                attackerClassWindowPosition.Y + AttackerClassWindow.Height + WindowSpacing);
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
            return new Vector2(GameDriver.ScreenSize.X - DefenderLabelWindow.Width - WindowEdgeBuffer.X,
                HelpTextWindowPosition().Y + HelpTextWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderPortraitWindowPosition()
        {
            //Anchored beneath below label window
            Vector2 defenderLabelWindowPosition = DefenderLabelWindowPosition();

            return new Vector2(GameDriver.ScreenSize.X - DefenderPortraitWindow.Width - WindowEdgeBuffer.X,
                defenderLabelWindowPosition.Y + DefenderLabelWindow.Height + WindowSpacing);
        }


        private Vector2 DefenderClassWindowPosition()
        {
            //Anchored beneath portrait window
            Vector2 defenderPortraitWindowPosition = DefenderPortraitWindowPosition();

            return new Vector2(defenderPortraitWindowPosition.X,
                defenderPortraitWindowPosition.Y + DefenderPortraitWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderHpWindowPosition()
        {
            //Anchored beneath class window
            Vector2 defenderClassWindowPosition = DefenderClassWindowPosition();

            return new Vector2(defenderClassWindowPosition.X,
                defenderClassWindowPosition.Y + DefenderClassWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderDefWindowPosition()
        {
            //Anchored beneath HP window
            Vector2 defenderHpWindowPosition = DefenderHpWindowPosition();

            return new Vector2(GameDriver.ScreenSize.X - DefenderDefWindow.Width - WindowEdgeBuffer.X,
                defenderHpWindowPosition.Y + DefenderHpWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderBonusWindowPosition()
        {
            //Anchored beneath ATK window
            Vector2 defenderDefWindowPosition = DefenderDefWindowPosition();

            return new Vector2(GameDriver.ScreenSize.X - DefenderBonusWindow.Width - WindowEdgeBuffer.X,
                defenderDefWindowPosition.Y + DefenderDefWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderRangeWindowPosition()
        {
            //Anchored beneath Bonus window
            Vector2 defenderBonusWindowPosition = DefenderBonusWindowPosition();

            return new Vector2(GameDriver.ScreenSize.X - DefenderRangeWindow.Width - WindowEdgeBuffer.X,
                defenderBonusWindowPosition.Y + DefenderBonusWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderDiceLabelPosition()
        {
            //Anchored right of class label window
            Vector2 defenderLabelWindowPosition = DefenderLabelWindowPosition();

            return new Vector2(
                GameDriver.ScreenSize.X - DefenderDiceLabelWindow.Width - DefenderLabelWindow.Width - WindowSpacing -
                WindowEdgeBuffer.X, defenderLabelWindowPosition.Y);
        }

        private Vector2 DefenderDicePosition()
        {
            //Anchored right of class portrait window
            Vector2 defenderPortraitWindowPosition = DefenderPortraitWindowPosition();

            return new Vector2(
                GameDriver.ScreenSize.X - DefenderDiceWindow.Width - DefenderPortraitWindow.Width - WindowSpacing -
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
                    AttackerClassWindow.Draw(spriteBatch, AttackerClassWindowPosition());
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
                    DefenderClassWindow.Draw(spriteBatch, DefenderClassWindowPosition());
                    DefenderHpWindow.Draw(spriteBatch, DefenderHpWindowPosition());
                    DefenderDefWindow.Draw(spriteBatch, DefenderDefWindowPosition());
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