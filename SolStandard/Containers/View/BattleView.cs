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

namespace SolStandard.Containers.View
{
    public class BattleView : IUserInterface
    {
        //TODO Make this scale properly with resolution
        //TODO Calculate total content size to center UI
        private static readonly Vector2 WindowEdgeBuffer = new Vector2(200, 150);

        private const int WindowSpacing = 5;

        public Window AttackerPortraitWindow { get; private set; }
        private Window AttackerDetailWindow { get; set; }
        private Window AttackerHpWindow { get; set; }
        private Window AttackerAtkWindow { get; set; }
        public Window AttackerBonusWindow { get; private set; }
        private Window AttackerRangeWindow { get; set; }
        private Window AttackerDiceWindow { get; set; }
        private Window AttackerSpriteWindow { get; set; }

        public Window DefenderPortraitWindow { get; private set; }
        private Window DefenderClassWindow { get; set; }
        private Window DefenderHpWindow { get; set; }
        private Window DefenderDefWindow { get; set; }
        private Window DefenderBonusWindow { get; set; }
        private Window DefenderRangeWindow { get; set; }
        private Window DefenderDiceWindow { get; set; }
        private Window DefenderSpriteWindow { get; set; }

        private Window HelpTextWindow { get; set; }
        private Window UserPromptWindow { get; set; }

        private bool visible;

        public BattleView()
        {
            visible = true;
        }

        #region View Management

        public void HidePromptWindow()
        {
            UserPromptWindow.Visible = false;
        }

        #endregion View Management

        #region Generation

        public void GenerateHelpTextWindow(WindowContentGrid helpTextContent)
        {
            Color helpTextWindowColor = new Color(20, 20, 20, 200);
            HelpTextWindow = new Window(helpTextContent, helpTextWindowColor);
        }

        public void GenerateUserPromptWindow(WindowContentGrid promptTextContent, Vector2 sizeOverride)
        {
            Color promptWindowColor = new Color(40, 30, 40, 200);
            UserPromptWindow = new Window(promptTextContent, promptWindowColor,
                sizeOverride);
        }

        public void GenerateAttackerSpriteWindow(Color windowColor, GameUnit attacker)
        {
            AttackerSpriteWindow = new Window(
                new WindowContentGrid(
                    new[,]
                    {
                        {
                            attacker.GetMapSprite(new Vector2(120), UnitAnimationState.Attack)
                        }
                    },
                    1
                ),
                windowColor
            );
        }

        public void GenerateAttackerDiceWindow(Color attackerWindowColor,
            ref CombatDice attackerDice)
        {
            IRenderable[,] diceWindowContent =
            {
                {attackerDice}
            };
            WindowContentGrid attackerDiceContentGrid = new WindowContentGrid(diceWindowContent, 1);
            AttackerDiceWindow = new Window(attackerDiceContentGrid,
                attackerWindowColor);
        }

        public void GenerateAttackerInRangeWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            bool inRange)
        {
            IRenderable[,] attackerRangeContent =
            {
                {
                    UnitStatistics.GetSpriteAtlas(StatIcons.Crosshair),
                    new RenderText(AssetManager.WindowFont, "In Range: "),
                    new RenderText(AssetManager.WindowFont, inRange.ToString(),
                        (inRange) ? GameContext.PositiveColor : GameContext.NegativeColor)
                }
            };
            WindowContentGrid attackerRangeContentGrid = new WindowContentGrid(attackerRangeContent, 1);
            AttackerRangeWindow = new Window(attackerRangeContentGrid,
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
                    (Convert.ToInt32(terrainAttackBonus) > 0)
                        ? UnitStatistics.GetSpriteAtlas(StatIcons.Positive)
                        : UnitStatistics.GetSpriteAtlas(StatIcons.Negative),
                    new RenderText(AssetManager.WindowFont, "Bonus: "),
                    new RenderText(AssetManager.WindowFont, terrainAttackBonus,
                        (Convert.ToInt32(terrainAttackBonus) > 0)
                            ? GameContext.PositiveColor
                            : GameContext.NeutralColor)
                }
            };
            WindowContentGrid attackerBonusContentGrid = new WindowContentGrid(attackerBonusContent, 1);
            AttackerBonusWindow = new Window(attackerBonusContentGrid,
                attackerWindowColor, portraitWidthOverride);

            return Convert.ToInt32(terrainAttackBonus);
        }

        public void GenerateAttackerAtkWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            UnitStatistics attackerStats)
        {
            IRenderable[,] attackerAtkContent =
            {
                {
                    UnitStatistics.GetSpriteAtlas(StatIcons.Atk, new Vector2(GameDriver.CellSize)),
                    new RenderText(AssetManager.WindowFont, "ATK: "),
                    new RenderText(
                        AssetManager.WindowFont,
                        attackerStats.Atk.ToString(),
                        UnitStatistics.DetermineStatColor(attackerStats.Atk, attackerStats.BaseAtk)
                    )
                }
            };
            WindowContentGrid attackerAtkContentGrid = new WindowContentGrid(attackerAtkContent, 1);
            AttackerAtkWindow = new Window(attackerAtkContentGrid,
                attackerWindowColor, portraitWidthOverride);
        }

        public void GenerateAttackerHpWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            GameUnit attacker, int hpBarHeight)
        {
            IRenderable hpIcon = UnitStatistics.GetSpriteAtlas(StatIcons.Hp, new Vector2(GameDriver.CellSize));
            IRenderable hpLabel = new RenderText(AssetManager.WindowFont, "HP: ");
            Vector2 hpBarSize = new Vector2(attacker.LargePortrait.Width - hpLabel.Width - hpIcon.Width, hpBarHeight);
            IRenderable hpBar = attacker.GetCombatHealthBar(hpBarSize);
            IRenderable[,] attackerHpContent =
            {
                {hpIcon, hpLabel, hpBar}
            };
            WindowContentGrid attackerHpContentGrid = new WindowContentGrid(attackerHpContent, 1);
            AttackerHpWindow =
                new Window(attackerHpContentGrid, attackerWindowColor,
                    portraitWidthOverride);
        }

        public void GenerateAttackerDetailWindow(Color attackerWindowColor, Vector2 portraitWidthOverride,
            IRenderable attackerDetail)
        {
            AttackerDetailWindow = new Window(attackerDetail, attackerWindowColor, portraitWidthOverride);
        }

        public void GenerateAttackerPortraitWindow(Color attackerWindowColor, IRenderable attackerPortrait)
        {
            AttackerPortraitWindow =
                new Window(attackerPortrait, attackerWindowColor);
        }


        public void GenerateDefenderSpriteWindow(Color windowColor, GameUnit defender)
        {
            DefenderSpriteWindow = new Window(
                new WindowContentGrid(
                    new[,]
                    {
                        {
                            defender.GetMapSprite(new Vector2(120), UnitAnimationState.Attack)
                        }
                    },
                    1
                ),
                windowColor
            );
        }


        public void GenerateDefenderDiceWindow(Color defenderWindowColor, ref CombatDice defenderDice)
        {
            IRenderable[,] diceWindowContent =
            {
                {defenderDice}
            };

            WindowContentGrid defenderDiceContentGrid = new WindowContentGrid(diceWindowContent, 1);
            DefenderDiceWindow = new Window(defenderDiceContentGrid,
                defenderWindowColor);
        }

        public void GenerateDefenderRangeWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            bool inRange)
        {
            IRenderable[,] defenderRangeContent =
            {
                {
                    UnitStatistics.GetSpriteAtlas(StatIcons.Crosshair),
                    new RenderText(AssetManager.WindowFont, "In Range: "),
                    new RenderText(AssetManager.WindowFont, inRange.ToString(),
                        (inRange) ? GameContext.PositiveColor : GameContext.NegativeColor)
                }
            };

            WindowContentGrid defenderRangeContentGrid = new WindowContentGrid(defenderRangeContent, 1);
            DefenderRangeWindow = new Window(defenderRangeContentGrid,
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
                    (Convert.ToInt32(terrainDefenseBonus) > 0)
                        ? UnitStatistics.GetSpriteAtlas(StatIcons.Positive)
                        : UnitStatistics.GetSpriteAtlas(StatIcons.Negative),
                    new RenderText(AssetManager.WindowFont, "Bonus: "),
                    new RenderText(AssetManager.WindowFont, terrainDefenseBonus,
                        (Convert.ToInt32(terrainDefenseBonus) > 0)
                            ? GameContext.PositiveColor
                            : GameContext.NeutralColor)
                }
            };
            WindowContentGrid defenderBonusContentGrid = new WindowContentGrid(defenderBonusContent, 1);
            DefenderBonusWindow = new Window(defenderBonusContentGrid,
                defenderWindowColor, portraitWidthOverride);

            return Convert.ToInt32(terrainDefenseBonus);
        }

        public void GenerateDefenderDefWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            UnitStatistics defenderStats)
        {
            IRenderable[,] defenderAtkContent =
            {
                {
                    UnitStatistics.GetSpriteAtlas(StatIcons.Def, new Vector2(GameDriver.CellSize)),
                    new RenderText(AssetManager.WindowFont, "DEF: "),
                    new RenderText(
                        AssetManager.WindowFont,
                        defenderStats.Def.ToString(),
                        UnitStatistics.DetermineStatColor(defenderStats.Def, defenderStats.BaseDef)
                    )
                }
            };
            WindowContentGrid defenderAtkContentGrid = new WindowContentGrid(defenderAtkContent, 1);
            DefenderDefWindow = new Window(defenderAtkContentGrid,
                defenderWindowColor, portraitWidthOverride);
        }

        public void GenerateDefenderHpWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            GameUnit defender, int hpBarHeight)
        {
            IRenderable hpIcon = UnitStatistics.GetSpriteAtlas(StatIcons.Hp, new Vector2(GameDriver.CellSize));
            IRenderable hpLabel = new RenderText(AssetManager.WindowFont, "HP: ");
            Vector2 hpBarSize = new Vector2(defender.LargePortrait.Width - hpLabel.Width - hpIcon.Width, hpBarHeight);
            IRenderable hpBar = defender.GetCombatHealthBar(hpBarSize);
            IRenderable[,] defenderHpContent =
            {
                {hpIcon, hpLabel, hpBar}
            };
            WindowContentGrid defenderHpContentGrid = new WindowContentGrid(defenderHpContent, 1);
            DefenderHpWindow =
                new Window(defenderHpContentGrid, defenderWindowColor,
                    portraitWidthOverride);
        }

        public void GenerateDefenderDetailWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            IRenderable defenderDetail)
        {
            DefenderClassWindow = new Window(defenderDetail, defenderWindowColor, portraitWidthOverride);
        }

        public void GenerateDefenderPortraitWindow(Color defenderWindowColor, IRenderable defenderPortrait)
        {
            DefenderPortraitWindow =
                new Window(defenderPortrait, defenderWindowColor);
        }

        #endregion Generation

        #region Window Positions

        private Vector2 HelpTextWindowPosition()
        {
            //Top-left
            return WindowEdgeBuffer;
        }

        private Vector2 UserPromptWindowPosition()
        {
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - (float) UserPromptWindow.Width / 2,
                GameDriver.ScreenSize.Y - UserPromptWindow.Height - WindowSpacing
            );
        }

        private static float RightAlignWindow(IRenderable placingWindow, IRenderable referenceWindow,
            float referenceWindowXPosition)
        {
            return referenceWindowXPosition + referenceWindow.Width - placingWindow.Width;
        }

        private static float CenterHorizontally(IRenderable placingWindow, IRenderable referenceWindow,
            float referenceWindowXPosition)
        {
            return referenceWindowXPosition +
                   (float) referenceWindow.Width / 2 -
                   (float) placingWindow.Width / 2;
        }

        #region Attacker

        private Vector2 AttackerDetailWindowPosition()
        {
            //Anchored beneath health window
            Vector2 attackerHpWindowPosition = AttackerHpWindowPosition();

            return new Vector2(
                RightAlignWindow(AttackerDetailWindow, AttackerHpWindow, attackerHpWindowPosition.X),
                attackerHpWindowPosition.Y + AttackerHpWindow.Height + WindowSpacing
            );
        }

        private Vector2 AttackerPortraitWindowPosition()
        {
            //Anchored left of detail window
            Vector2 attackerDetailWindowPosition = AttackerDetailWindowPosition();

            return new Vector2(
                attackerDetailWindowPosition.X - AttackerPortraitWindow.Width - WindowSpacing,
                attackerDetailWindowPosition.Y
            );
        }

        private Vector2 AttackerHpWindowPosition()
        {
            //Anchored beneath Range window
            Vector2 attackerRangeWindowPosition = AttackerRangeWindowPosition();

            return new Vector2(
                RightAlignWindow(AttackerHpWindow, AttackerRangeWindow, attackerRangeWindowPosition.X),
                attackerRangeWindowPosition.Y + AttackerHpWindow.Height + WindowSpacing
            );
        }

        private Vector2 AttackerAtkWindowPosition()
        {
            //Anchored above bonus window
            Vector2 attackerBonusWindowPosition = AttackerBonusWindowPosition();

            return new Vector2(
                CenterHorizontally(AttackerAtkWindow, AttackerBonusWindow, attackerBonusWindowPosition.X),
                attackerBonusWindowPosition.Y - AttackerAtkWindow.Height - WindowSpacing
            );
        }

        private Vector2 AttackerBonusWindowPosition()
        {
            //Anchored above dice window
            Vector2 attackerDicePosition = AttackerDicePosition();

            return new Vector2(
                CenterHorizontally(AttackerBonusWindow, AttackerDiceWindow, attackerDicePosition.X),
                attackerDicePosition.Y - AttackerBonusWindow.Height - WindowSpacing
            );
        }

        private Vector2 AttackerRangeWindowPosition()
        {
            Vector2 attackerSpriteWindowPosition = AttackerSpriteWindowPosition();
            //Anchored beneath Sprite Window
            return new Vector2(
                RightAlignWindow(AttackerRangeWindow, AttackerSpriteWindow, attackerSpriteWindowPosition.X),
                attackerSpriteWindowPosition.Y + AttackerSpriteWindow.Height + WindowSpacing
            );
        }

        private Vector2 AttackerDicePosition()
        {
            //Left quarter of screen
            return new Vector2(
                GameDriver.ScreenSize.X / 4 - (float) AttackerDiceWindow.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) AttackerDiceWindow.Height / 2
            );
        }

        private Vector2 AttackerSpriteWindowPosition()
        {
            return GameDriver.ScreenSize / 2 -
                   new Vector2(AttackerSpriteWindow.Width + WindowSpacing, AttackerSpriteWindow.Height);
        }

        #endregion Attacker

        #region Defender

        private Vector2 DefenderDetailWindowPosition()
        {
            //Anchored beneath Health window
            Vector2 defenderHpWindowPosition = DefenderHpWindowPosition();

            return new Vector2(defenderHpWindowPosition.X,
                defenderHpWindowPosition.Y + DefenderHpWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderPortraitWindowPosition()
        {
            //Anchored right of defender details window
            Vector2 defenderClassWindowPosition = DefenderDetailWindowPosition();

            return new Vector2(
                defenderClassWindowPosition.X + DefenderClassWindow.Width + WindowSpacing,
                defenderClassWindowPosition.Y
            );
        }

        private Vector2 DefenderBonusWindowPosition()
        {
            //Anchored above Dice window
            Vector2 defenderDicePosition = DefenderDicePosition();

            return new Vector2(
                CenterHorizontally(DefenderBonusWindow, DefenderDiceWindow, defenderDicePosition.X),
                defenderDicePosition.Y - DefenderDefWindow.Height - WindowSpacing
            );
        }


        private Vector2 DefenderHpWindowPosition()
        {
            //Anchored beneath Range window
            Vector2 defenderRangeWindowPosition = DefenderRangeWindowPosition();

            return new Vector2(
                GameDriver.ScreenSize.X / 2 + WindowSpacing,
                defenderRangeWindowPosition.Y + DefenderBonusWindow.Height + WindowSpacing
            );
        }

        private Vector2 DefenderDefWindowPosition()
        {
            //Anchored above Bonus window
            Vector2 defenderBonusWindowPosition = DefenderBonusWindowPosition();

            return new Vector2(
                (defenderBonusWindowPosition.X + (float) DefenderBonusWindow.Width / 2) -
                ((float) DefenderDefWindow.Width / 2),
                defenderBonusWindowPosition.Y - DefenderDefWindow.Height - WindowSpacing
            );
        }

        private Vector2 DefenderRangeWindowPosition()
        {
            //Anchored beneath Sprite Window
            return new Vector2(
                GameDriver.ScreenSize.X / 2 + WindowSpacing,
                DefenderSpriteWindowPosition().Y + DefenderSpriteWindow.Height + WindowSpacing
            );
        }

        private Vector2 DefenderDicePosition()
        {
            //Right quarter of screen
            return new Vector2(
                GameDriver.ScreenSize.X / 4 * 3 - (float) DefenderDiceWindow.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) DefenderDiceWindow.Height / 2
            );
        }

        private Vector2 DefenderSpriteWindowPosition()
        {
            return GameDriver.ScreenSize / 2 - new Vector2(-WindowSpacing, DefenderSpriteWindow.Height);
        }

        #endregion Defender

        #endregion Window Positions

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            if (HelpTextWindow != null)
            {
                HelpTextWindow.Draw(spriteBatch, HelpTextWindowPosition());

                if (AttackerPortraitWindow != null)
                {
                    AttackerPortraitWindow.Draw(spriteBatch, AttackerPortraitWindowPosition());
                    AttackerDetailWindow.Draw(spriteBatch, AttackerDetailWindowPosition());
                    AttackerHpWindow.Draw(spriteBatch, AttackerHpWindowPosition());
                    AttackerAtkWindow.Draw(spriteBatch, AttackerAtkWindowPosition());
                    AttackerBonusWindow.Draw(spriteBatch, AttackerBonusWindowPosition());
                    AttackerRangeWindow.Draw(spriteBatch, AttackerRangeWindowPosition());
                    AttackerDiceWindow.Draw(spriteBatch, AttackerDicePosition());
                    AttackerSpriteWindow.Draw(spriteBatch,
                        AttackerSpriteWindowPosition());
                }

                if (DefenderPortraitWindow != null)
                {
                    DefenderPortraitWindow.Draw(spriteBatch, DefenderPortraitWindowPosition());
                    DefenderClassWindow.Draw(spriteBatch, DefenderDetailWindowPosition());
                    DefenderHpWindow.Draw(spriteBatch, DefenderHpWindowPosition());
                    DefenderDefWindow.Draw(spriteBatch, DefenderDefWindowPosition());
                    DefenderBonusWindow.Draw(spriteBatch, DefenderBonusWindowPosition());
                    DefenderRangeWindow.Draw(spriteBatch, DefenderRangeWindowPosition());
                    DefenderDiceWindow.Draw(spriteBatch, DefenderDicePosition());
                    DefenderSpriteWindow.Draw(spriteBatch, DefenderSpriteWindowPosition());
                }
            }

            if (UserPromptWindow != null)
            {
                UserPromptWindow.Draw(spriteBatch, UserPromptWindowPosition());
            }
        }
    }
}