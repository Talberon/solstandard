using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Combat;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class BattleView : IUserInterface
    {
        private static readonly Vector2 WindowEdgeBuffer = new Vector2(WindowSpacing);

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

        #region Attacker Windows

        public void GenerateAttackerSpriteWindow(GameUnit attacker, Color spriteColor, UnitAnimationState state)
        {
            AttackerSpriteWindow = BattlerWindow(attacker, spriteColor, state);
        }

        public void GenerateAttackerDamageWindow(Color attackerWindowColor, CombatDamage attackerDamage)
        {
            AttackerDiceWindow = new Window(attackerDamage, attackerWindowColor);
        }

        public void GenerateAttackerInRangeWindow(Color attackerWindowColor, bool inRange)
        {
            AttackerRangeWindow = RangeWindow(attackerWindowColor, inRange);
        }

        public void GenerateAttackerBonusWindow(int luckStat, int bonusLuck, Color attackerWindowColor)
        {
            AttackerBonusWindow = BonusWindow(luckStat, bonusLuck, attackerWindowColor);
        }

        public void GenerateAttackerAtkWindow(Color windowColor, UnitStatistics attackerStats)
        {
            AttackerAtkWindow = AtkStatWindow(windowColor, attackerStats);
        }

        public void GenerateAttackerHpWindow(Color windowColor, GameUnit attacker)
        {
            AttackerHpWindow = GenerateHpWindow(attacker, windowColor);
        }

        public void GenerateAttackerDetailWindow(Color attackerWindowColor,
            IRenderable attackerDetail)
        {
            AttackerDetailWindow = new Window(attackerDetail, attackerWindowColor);
        }

        public void GenerateAttackerPortraitWindow(Color attackerWindowColor, IRenderable attackerPortrait)
        {
            AttackerPortraitWindow = new Window(attackerPortrait, attackerWindowColor);
        }

        #endregion Attacker Windows

        #region Defender Windows

        public void GenerateDefenderSpriteWindow(GameUnit defender, Color spriteColor, UnitAnimationState state)
        {
            DefenderSpriteWindow = BattlerWindow(defender, spriteColor, state);
        }


        public void GenerateDefenderDamageWindow(Color defenderWindowColor, CombatDamage defenderDamage)
        {
            DefenderDiceWindow = new Window(defenderDamage, defenderWindowColor);
        }

        public void GenerateDefenderRangeWindow(Color defenderWindowColor, bool inRange)
        {
            DefenderRangeWindow = RangeWindow(defenderWindowColor, inRange);
        }


        public void GenerateDefenderBonusWindow(int luckStat, int bonusLuck, Color defenderWindowColor)
        {
            DefenderBonusWindow = BonusWindow(luckStat, bonusLuck, defenderWindowColor);
        }


        public void GenerateDefenderDefWindow(Color windowColor, UnitStatistics defenderStats)
        {
            DefenderAtkWindow = AtkStatWindow(windowColor, defenderStats);
        }


        public void GenerateDefenderHpWindow(Color windowColor, GameUnit defender)
        {
            DefenderHpWindow = GenerateHpWindow(defender, windowColor);
        }

        public void GenerateDefenderDetailWindow(Color defenderWindowColor,
            IRenderable defenderDetail)
        {
            DefenderDetailWindow = new Window(defenderDetail, defenderWindowColor);
        }

        public void GenerateDefenderPortraitWindow(Color defenderWindowColor, IRenderable defenderPortrait)
        {
            DefenderPortraitWindow = new Window(defenderPortrait, defenderWindowColor);
        }

        #endregion Defender Windows

        #region Window Generators

        private static Window GenerateHpWindow(GameUnit unit, Color windowColor)
        {
            return new Window(unit.GetCombatHealthBar(HpBarSize), windowColor);
        }

        private static Window RangeWindow(Color windowColor, bool inRange)
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
            return new Window(defenderRangeContentGrid, windowColor);
        }

        private static Window BonusWindow(int luckStat, int bonusLuck, Color attackerWindowColor)
        {
            IRenderable[,] attackerBonusContent =
            {
                {
                    (Convert.ToInt32(bonusLuck) > 0)
                        ? UnitStatistics.GetSpriteAtlas(StatIcons.Positive)
                        : UnitStatistics.GetSpriteAtlas(StatIcons.Negative),

                    new RenderText(AssetManager.WindowFont, "Bonus: " + luckStat),

                    (bonusLuck > 0)
                        ? new RenderText(AssetManager.WindowFont, " + " + bonusLuck,
                                (bonusLuck > 0) ? GameContext.PositiveColor : GameContext.NeutralColor) as
                            IRenderable
                        : new RenderBlank() as IRenderable
                }
            };
            WindowContentGrid attackerBonusContentGrid = new WindowContentGrid(attackerBonusContent, 0);
            return new Window(attackerBonusContentGrid, attackerWindowColor);
        }

        private static Window BattlerWindow(GameUnit attacker, Color spriteColor, UnitAnimationState state)
        {
            const int spriteSize = 200;

            return new Window(
                new WindowContentGrid(
                    new[,]
                    {
                        {
                            attacker.GetMapSprite(new Vector2(spriteSize), spriteColor, state)
                        }
                    },
                    1
                ),
                Color.Transparent
            );
        }

        private static Window AtkStatWindow(Color windowColor, UnitStatistics stats)
        {
            WindowContentGrid atkContentGrid = new WindowContentGrid(new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(StatIcons.Armor, new Vector2(GameDriver.CellSize)),
                        new RenderText(AssetManager.WindowFont, "ATK: "),
                        new RenderText(
                            AssetManager.WindowFont,
                            stats.Atk.ToString(),
                            UnitStatistics.DetermineStatColor(stats.Atk, stats.BaseAtk)
                        )
                    }
                },
                1
            );
            return new Window(atkContentGrid, windowColor);
        }

        #endregion Window Generators

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
            //Anchored below Range window
            Vector2 attackerRangeWindowPosition = AttackerRangeWindowPosition();

            return new Vector2(
                RightAlignWindow(AttackerAtkWindow, AttackerRangeWindow, attackerRangeWindowPosition.X),
                attackerRangeWindowPosition.Y + AttackerRangeWindow.Height + WindowSpacing
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
            Vector2 attackerSpritePosition = AttackerSpriteWindowPosition();
            //Anchored beneath Sprite Window
            return new Vector2(
                RightAlignWindow(AttackerRangeWindow, AttackerSpriteWindow, attackerSpritePosition.X),
                attackerSpritePosition.Y + AttackerSpriteWindow.Height + WindowSpacing
                - ((float) AttackerSpriteWindow.Height / 3)
            );
        }

        private Vector2 AttackerDiceWindowPosition()
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
            //Center of screen
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
            //Anchored below Range window
            Vector2 defenderRangeWindowPosition = DefenderRangeWindowPosition();

            return new Vector2(
                defenderRangeWindowPosition.X,
                defenderRangeWindowPosition.Y + DefenderRangeWindow.Height + WindowSpacing
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
            //Anchored beneath Sprite Window
            Vector2 defenderSpriteWindowPosition = DefenderSpriteWindowPosition();

            return new Vector2(
                defenderSpriteWindowPosition.X,
                defenderSpriteWindowPosition.Y + DefenderSpriteWindow.Height + WindowSpacing
                - ((float) DefenderSpriteWindow.Height / 3)
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
                    AttackerDiceWindow.Draw(spriteBatch, AttackerDiceWindowPosition());
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