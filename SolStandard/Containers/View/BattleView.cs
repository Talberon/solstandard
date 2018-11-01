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
        private static readonly Vector2 HpBarSize = new Vector2(300, 64);

        public Window AttackerPortraitWindow { get; private set; }
        private Window AttackerDetailWindow { get; set; }
        private Window AttackerHpWindow { get; set; }
        private Window AttackerAtkWindow { get; set; }
        public Window AttackerBonusWindow { get; private set; }
        private Window AttackerRangeWindow { get; set; }
        private Window AttackerDiceWindow { get; set; }
        private Window AttackerSpriteWindow { get; set; }

        public Window DefenderPortraitWindow { get; private set; }
        private Window DefenderDetailWindow { get; set; }
        private Window DefenderHpWindow { get; set; }
        private Window DefenderAtkWindow { get; set; }
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

        public void GenerateAttackerHpWindow(Color windowColor, Vector2 sizeOverride, GameUnit attacker)
        {
            AttackerHpWindow = GenerateHpWindow(attacker, sizeOverride, windowColor);
        }

        private Window GenerateHpWindow(GameUnit unit, Vector2 sizeOverride, Color windowColor)
        {
            return new Window(unit.GetCombatHealthBar(HpBarSize), windowColor, sizeOverride);
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
            DefenderAtkWindow = new Window(defenderAtkContentGrid,
                defenderWindowColor, portraitWidthOverride);
        }

        public void GenerateDefenderHpWindow(Color windowColor, Vector2 sizeOverride, GameUnit defender)
        {
            DefenderHpWindow = GenerateHpWindow(defender, sizeOverride, windowColor);
        }

        public void GenerateDefenderDetailWindow(Color defenderWindowColor, Vector2 portraitWidthOverride,
            IRenderable defenderDetail)
        {
            DefenderDetailWindow = new Window(defenderDetail, defenderWindowColor, portraitWidthOverride);
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

        private static float BottomAlignWindow(IRenderable placingWindow, IRenderable referenceWindow,
            float referenceWindowYPosition)
        {
            return referenceWindowYPosition + referenceWindow.Height - placingWindow.Height;
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
            //Anchored right of portrait window
            Vector2 attackerPortraitWindowPosition = AttackerPortraitWindowPosition();

            return new Vector2(
                attackerPortraitWindowPosition.X + AttackerPortraitWindow.Width,
                BottomAlignWindow(AttackerDetailWindow, AttackerPortraitWindow, attackerPortraitWindowPosition.Y)
            );
        }

        private Vector2 AttackerPortraitWindowPosition()
        {
            //Bottom-left of screen
            return new Vector2(
                WindowSpacing,
                GameDriver.ScreenSize.Y - AttackerPortraitWindow.Height - WindowSpacing
            );
        }

        private Vector2 AttackerHpWindowPosition()
        {
            //Anchored above Sprite window
            Vector2 attackerSpriteWindowPosition = AttackerSpriteWindowPosition();

            return new Vector2(
                RightAlignWindow(AttackerHpWindow, AttackerSpriteWindow, attackerSpriteWindowPosition.X),
                attackerSpriteWindowPosition.Y - AttackerHpWindow.Height - WindowSpacing
            );
        }

        private Vector2 AttackerAtkWindowPosition()
        {
            //Anchored below Sprite window
            Vector2 attackerSpriteWindowPosition = AttackerSpriteWindowPosition();

            return new Vector2(
                RightAlignWindow(AttackerAtkWindow, AttackerSpriteWindow, attackerSpriteWindowPosition.X),
                attackerSpriteWindowPosition.Y + AttackerSpriteWindow.Height + WindowSpacing
            );
        }

        private Vector2 AttackerBonusWindowPosition()
        {
            //Anchored below Atk window
            Vector2 attackerAtkWindowPosition = AttackerAtkWindowPosition();

            return new Vector2(
                RightAlignWindow(AttackerBonusWindow, AttackerAtkWindow, attackerAtkWindowPosition.X),
                attackerAtkWindowPosition.Y + AttackerAtkWindow.Height + WindowSpacing
            );
        }

        private Vector2 AttackerRangeWindowPosition()
        {
            Vector2 attackerDicePosition = AttackerDicePosition();
            //Anchored beneath Dice Window
            return new Vector2(
                RightAlignWindow(AttackerRangeWindow, AttackerDiceWindow, attackerDicePosition.X),
                attackerDicePosition.Y + AttackerDiceWindow.Height + WindowSpacing
            );
        }

        private Vector2 AttackerDicePosition()
        {
            Vector2 attackerBonusWindowPosition = AttackerBonusWindowPosition();
            //Anchored beneath Bonus Window
            return new Vector2(
                RightAlignWindow(AttackerDiceWindow, AttackerBonusWindow, attackerBonusWindowPosition.X),
                attackerBonusWindowPosition.Y + AttackerBonusWindow.Height + WindowSpacing
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
            //Anchored left of portrait window
            Vector2 defenderPortraitWindowPosition = DefenderPortraitWindowPosition();

            return new Vector2(
                defenderPortraitWindowPosition.X - DefenderDetailWindow.Width,
                BottomAlignWindow(DefenderDetailWindow, DefenderPortraitWindow, defenderPortraitWindowPosition.Y)
            );
        }

        private Vector2 DefenderPortraitWindowPosition()
        {
            //Bottom-right of screen
            return new Vector2(
                GameDriver.ScreenSize.X - DefenderPortraitWindow.Width - WindowSpacing,
                GameDriver.ScreenSize.Y - DefenderPortraitWindow.Height - WindowSpacing
            );
        }

        private Vector2 DefenderHpWindowPosition()
        {
            //Anchored above Sprite window
            Vector2 defenderSpriteWindowPosition = DefenderSpriteWindowPosition();

            return new Vector2(
                defenderSpriteWindowPosition.X,
                defenderSpriteWindowPosition.Y - DefenderHpWindow.Height - WindowSpacing
            );
        }

        private Vector2 DefenderAtkWindowPosition()
        {
            //Anchored below Sprite window
            Vector2 defenderSpriteWindowPosition = DefenderSpriteWindowPosition();

            return new Vector2(
                defenderSpriteWindowPosition.X,
                defenderSpriteWindowPosition.Y + DefenderSpriteWindow.Height + WindowSpacing
            );
        }

        private Vector2 DefenderBonusWindowPosition()
        {
            //Anchored below Atk window
            Vector2 defenderAtkWindowPosition = DefenderAtkWindowPosition();

            return new Vector2(
                defenderAtkWindowPosition.X,
                defenderAtkWindowPosition.Y + DefenderAtkWindow.Height + WindowSpacing
            );
        }

        private Vector2 DefenderDiceWindowPosition()
        {
            //Anchored beneath Bonus Window
            Vector2 defenderBonusWindowPosition = DefenderBonusWindowPosition();

            return new Vector2(
                defenderBonusWindowPosition.X,
                defenderBonusWindowPosition.Y + DefenderBonusWindow.Height + WindowSpacing
            );
        }

        private Vector2 DefenderRangeWindowPosition()
        {
            //Anchored beneath Dice Window
            Vector2 defenderDiceWindowPosition = DefenderDiceWindowPosition();

            return new Vector2(
                defenderDiceWindowPosition.X,
                defenderDiceWindowPosition.Y + DefenderDiceWindow.Height + WindowSpacing
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
                    DefenderDetailWindow.Draw(spriteBatch, DefenderDetailWindowPosition());
                    DefenderHpWindow.Draw(spriteBatch, DefenderHpWindowPosition());
                    DefenderAtkWindow.Draw(spriteBatch, DefenderAtkWindowPosition());
                    DefenderBonusWindow.Draw(spriteBatch, DefenderBonusWindowPosition());
                    DefenderRangeWindow.Draw(spriteBatch, DefenderRangeWindowPosition());
                    DefenderDiceWindow.Draw(spriteBatch, DefenderDiceWindowPosition());
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