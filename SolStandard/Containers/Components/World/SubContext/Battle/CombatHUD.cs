using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Animation;
using SolStandard.HUD.Window.Content;
using SolStandard.HUD.Window.Content.Combat;
using SolStandard.Utility;
using SolStandard.Utility.Assets;


namespace SolStandard.Containers.Components.World.SubContext.Battle
{
    public class CombatHUD : IUserInterface
    {
        private static readonly Vector2 WindowEdgeBuffer = new Vector2(WindowSpacing);

        private const int WindowSlideSpeed = 5;
        private const int WindowVerticalSlideDistance = 200;
        private const int WindowHorizontalSlideDistance = 50;
        private const int UnitSlideDistance = 10;

        private const int CombatDelay = 3;
        private const int RegularDelay = 12;

        private const int WindowSpacing = 5;
        private static readonly Vector2 HpBarSize = new Vector2(350, 80);

        private AnimatedRenderable AttackerPortraitWindow { get; set; }
        private AnimatedRenderable AttackerDetailWindow { get; set; }
        private AnimatedRenderable AttackerHpWindow { get; set; }
        private AnimatedRenderable AttackerAtkWindow { get; set; }
        public AnimatedRenderable AttackerBonusWindow { get; private set; }
        private AnimatedRenderable AttackerRangeWindow { get; set; }
        private AnimatedRenderable AttackerDiceWindow { get; set; }
        private AnimatedRenderable AttackerSpriteWindow { get; set; }

        private AnimatedRenderable DefenderPortraitWindow { get; set; }
        private AnimatedRenderable DefenderDetailWindow { get; set; }
        private AnimatedRenderable DefenderHpWindow { get; set; }
        private AnimatedRenderable DefenderAtkWindow { get; set; }
        private AnimatedRenderable DefenderBonusWindow { get; set; }
        private AnimatedRenderable DefenderRangeWindow { get; set; }
        private AnimatedRenderable DefenderDiceWindow { get; set; }
        private AnimatedRenderable DefenderSpriteWindow { get; set; }

        private AnimatedRenderable HelpTextWindow { get; set; }
        private Window UserPromptWindow { get; set; }

        private static IRenderableAnimation RightBattlerAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Left, UnitSlideDistance, WindowSlideSpeed);

        private static IRenderableAnimation LeftBattlerAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Right, UnitSlideDistance, WindowSlideSpeed);

        private static IRenderableAnimation RightSideRenderableAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Left, WindowHorizontalSlideDistance, WindowSlideSpeed);

        private static IRenderableAnimation LeftSideRenderableAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Right, WindowHorizontalSlideDistance, WindowSlideSpeed);

        private static IRenderableAnimation BottomRenderableAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Up, WindowVerticalSlideDistance, WindowSlideSpeed);

        private static IRenderableAnimation TopRenderableAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Down, WindowVerticalSlideDistance, WindowSlideSpeed);

        #region View Management

        public void HidePromptWindow()
        {
            UserPromptWindow.Visible = false;
        }

        #endregion View Management

        #region Generation

        public void GenerateHelpTextWindow(WindowContentGrid helpTextContent)
        {
            var helpTextWindowColor = new Color(20, 20, 20, 200);
            HelpTextWindow =
                new AnimatedRenderable(new Window(helpTextContent, helpTextWindowColor), TopRenderableAnimation);
        }

        public void GenerateUserPromptWindow(WindowContentGrid promptTextContent, Vector2 sizeOverride)
        {
            var promptWindowColor = new Color(40, 30, 40, 200);
            UserPromptWindow = new Window(promptTextContent, promptWindowColor,
                sizeOverride);
        }

        #region Attacker Windows

        public void GenerateAttackerSpriteWindow(GameUnit attacker, Color spriteColor, UnitAnimationState state)
        {
            int frameDelay = state switch
            {
                UnitAnimationState.Attack => CombatDelay,
                UnitAnimationState.Hit => CombatDelay,
                _ => RegularDelay
            };

            AttackerSpriteWindow =
                new AnimatedRenderable(BattlerWindow(attacker, spriteColor, state, frameDelay, false),
                    LeftBattlerAnimation);
        }

        public void GenerateAttackerDamageWindow(Color attackerWindowColor, CombatDamage attackerDamage)
        {
            AttackerDiceWindow =
                new AnimatedRenderable(new Window(attackerDamage, attackerWindowColor), LeftSideRenderableAnimation);
        }

        public void GenerateAttackerInRangeWindow(Color attackerWindowColor, bool inRange)
        {
            AttackerRangeWindow =
                new AnimatedRenderable(RangeWindow(attackerWindowColor, inRange), LeftSideRenderableAnimation);
        }

        public void GenerateAttackerBonusWindow(BonusStatistics bonusStatistics, Color attackerWindowColor)
        {
            AttackerBonusWindow =
                new AnimatedRenderable(BonusWindow(bonusStatistics, attackerWindowColor), LeftSideRenderableAnimation);
        }

        public void GenerateAttackerAtkWindow(Color windowColor, UnitStatistics attackerStats, Stats combatStat)
        {
            AttackerAtkWindow = new AnimatedRenderable(CombatStatWindow(windowColor, attackerStats, combatStat),
                LeftSideRenderableAnimation);
        }

        public void GenerateAttackerHpWindow(Color windowColor, GameUnit attacker,
            IRenderableAnimation animation = null)
        {
            AttackerHpWindow = new AnimatedRenderable(GenerateHpWindow(attacker, windowColor),
                animation ?? LeftSideRenderableAnimation);
        }

        public void GenerateAttackerUnitCard(Color attackerWindowColor, GameUnit attacker, bool animated = true)
        {
            GenerateAttackerDetailWindow(attackerWindowColor, attacker.DetailPane, animated);
            GenerateAttackerPortraitWindow(attackerWindowColor, attacker.MediumPortrait, animated);
        }

        private void GenerateAttackerDetailWindow(Color attackerWindowColor,
            IRenderable attackerDetail, bool animated)
        {
            AttackerDetailWindow =
                new AnimatedRenderable(new Window(attackerDetail, attackerWindowColor),
                    animated ? BottomRenderableAnimation : new RenderableStatic(AttackerDetailWindowPosition()));
        }

        private void GenerateAttackerPortraitWindow(Color attackerWindowColor, IRenderable attackerPortrait,
            bool animated)
        {
            AttackerPortraitWindow =
                new AnimatedRenderable(new Window(attackerPortrait, attackerWindowColor),
                    animated ? BottomRenderableAnimation : new RenderableStatic(AttackerPortraitWindowPosition()));
        }

        #endregion Attacker Windows

        #region Defender Windows

        public void GenerateDefenderSpriteWindow(GameUnit defender, Color spriteColor, UnitAnimationState state)
        {
            int frameDelay = state switch
            {
                UnitAnimationState.Attack => CombatDelay,
                UnitAnimationState.Hit => CombatDelay,
                _ => RegularDelay
            };

            DefenderSpriteWindow =
                new AnimatedRenderable(BattlerWindow(defender, spriteColor, state, frameDelay, true),
                    RightBattlerAnimation);
        }


        public void GenerateDefenderDamageWindow(Color defenderWindowColor, CombatDamage defenderDamage)
        {
            DefenderDiceWindow =
                new AnimatedRenderable(new Window(defenderDamage, defenderWindowColor), RightSideRenderableAnimation);
        }

        public void GenerateDefenderRangeWindow(Color defenderWindowColor, bool inRange)
        {
            DefenderRangeWindow =
                new AnimatedRenderable(RangeWindow(defenderWindowColor, inRange), RightSideRenderableAnimation);
        }


        public void GenerateDefenderBonusWindow(BonusStatistics bonusStatistics, Color defenderWindowColor)
        {
            DefenderBonusWindow =
                new AnimatedRenderable(BonusWindow(bonusStatistics, defenderWindowColor), RightSideRenderableAnimation);
        }


        public void GenerateDefenderRetWindow(Color windowColor, UnitStatistics defenderStats, Stats combatStat)
        {
            DefenderAtkWindow = new AnimatedRenderable(CombatStatWindow(windowColor, defenderStats, combatStat),
                RightSideRenderableAnimation);
        }


        public void GenerateDefenderHpWindow(Color windowColor, GameUnit defender,
            IRenderableAnimation animation = null)
        {
            DefenderHpWindow = new AnimatedRenderable(GenerateHpWindow(defender, windowColor),
                animation ?? RightSideRenderableAnimation);
        }

        public void GenerateDefenderUnitCard(Color defenderWindowColor, GameUnit defender, bool animated = true)
        {
            GenerateDefenderDetailWindow(defenderWindowColor, defender.DetailPane, animated);
            GenerateDefenderPortraitWindow(defenderWindowColor, defender.MediumPortrait, animated);
        }

        private void GenerateDefenderDetailWindow(Color defenderWindowColor,
            IRenderable defenderDetail, bool animated)
        {
            DefenderDetailWindow =
                new AnimatedRenderable(new Window(defenderDetail, defenderWindowColor),
                    animated ? BottomRenderableAnimation : new RenderableStatic(DefenderDetailWindowPosition()));
        }

        private void GenerateDefenderPortraitWindow(Color defenderWindowColor, IRenderable defenderPortrait,
            bool animated)
        {
            DefenderPortraitWindow =
                new AnimatedRenderable(new Window(defenderPortrait, defenderWindowColor),
                    animated ? BottomRenderableAnimation : new RenderableStatic(DefenderPortraitWindowPosition()));
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
                    UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                    new RenderText(AssetManager.WindowFont, "In Range: "),
                    new RenderText(AssetManager.WindowFont, inRange.ToString(),
                        (inRange) ? GlobalContext.PositiveColor : GlobalContext.NegativeColor)
                }
            };

            var defenderRangeContentGrid = new WindowContentGrid(defenderRangeContent);
            return new Window(defenderRangeContentGrid, windowColor);
        }

        private static Window BonusWindow(BonusStatistics bonusStatistics, Color attackerWindowColor)
        {
            IRenderable[,] attackerBonusContent =
            {
                {
                    ((bonusStatistics.AtkBonus + bonusStatistics.RetBonus + bonusStatistics.BlockBonus +
                      bonusStatistics.LuckBonus) > 0)
                        ? UnitStatistics.GetSpriteAtlas(Stats.Positive)
                        : UnitStatistics.GetSpriteAtlas(Stats.Negative),

                    new RenderText(AssetManager.WindowFont, "Bonus")
                }
            };
            var attackerBonusContentGrid = new WindowContentGrid(attackerBonusContent, 0);
            return new Window(attackerBonusContentGrid, attackerWindowColor);
        }

        private static Window BattlerWindow(GameUnit attacker, Color spriteColor, UnitAnimationState state,
            int frameDelay, bool isFlipped)
        {
            const int spriteSize = 200;

            return new Window(
                new WindowContentGrid(
                    new[,]
                    {
                        {
                            attacker.GetMapSprite(new Vector2(spriteSize), spriteColor, state, frameDelay, isFlipped)
                        }
                    }
                ),
                Color.Transparent
            );
        }

        private static Window CombatStatWindow(Color windowColor, UnitStatistics stats, Stats combatStat)
        {
            int statValue;
            int baseStatValue;

            switch (combatStat)
            {
                case Stats.Atk:
                    statValue = stats.Atk;
                    baseStatValue = stats.BaseAtk;
                    break;
                case Stats.Retribution:
                    statValue = stats.Ret;
                    baseStatValue = stats.BaseRet;
                    break;
                default:
                    statValue = -1;
                    baseStatValue = -1;
                    break;
            }

            var atkContentGrid = new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(combatStat, GameDriver.CellSizeVector),
                        new RenderText(AssetManager.WindowFont, UnitStatistics.Abbreviation[combatStat] + ": "),
                        new RenderText(
                            AssetManager.WindowFont,
                            statValue.ToString(),
                            UnitStatistics.DetermineStatColor(statValue, baseStatValue)
                        )
                    }
                }
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
            Vector2 attackerDiceWindowPosition = AttackerDiceWindowPosition();
            //Anchored beneath Dice Window
            return new Vector2(
                RightAlignWindow(AttackerRangeWindow, AttackerDiceWindow, attackerDiceWindowPosition.X),
                attackerDiceWindowPosition.Y + AttackerDiceWindow.Height + WindowSpacing
            );
        }

        private Vector2 AttackerDiceWindowPosition()
        {
            Vector2 attackerSpriteWindowPosition = AttackerSpriteWindowPosition();
            //Anchored beneath Sprite Window
            return new Vector2(
                RightAlignWindow(AttackerDiceWindow, AttackerSpriteWindow, attackerSpriteWindowPosition.X),
                attackerSpriteWindowPosition.Y + AttackerSpriteWindow.Height + WindowSpacing
                - ((float) AttackerSpriteWindow.Height / 3)
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
            (float x, float y) = DefenderPortraitWindowPosition();

            return new Vector2(
                x - DefenderDetailWindow.Width,
                BottomAlignWindow(DefenderDetailWindow, DefenderPortraitWindow, y)
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
            (float x, float y) = DefenderSpriteWindowPosition();

            return new Vector2(
                x,
                y - DefenderHpWindow.Height - WindowSpacing
            );
        }

        private Vector2 DefenderAtkWindowPosition()
        {
            //Anchored below Range window
            (float x, float y) = DefenderRangeWindowPosition();

            return new Vector2(
                x,
                y + DefenderRangeWindow.Height + WindowSpacing
            );
        }

        private Vector2 DefenderBonusWindowPosition()
        {
            //Anchored below Atk window
            (float x, float y) = DefenderAtkWindowPosition();

            return new Vector2(
                x,
                y + DefenderAtkWindow.Height + WindowSpacing
            );
        }

        private Vector2 DefenderDiceWindowPosition()
        {
            //Anchored beneath Sprite Window
            (float x, float y) = DefenderSpriteWindowPosition();

            return new Vector2(
                x,
                y + DefenderSpriteWindow.Height + WindowSpacing
                - ((float) DefenderSpriteWindow.Height / 3)
            );
        }

        private Vector2 DefenderRangeWindowPosition()
        {
            //Anchored beneath Dice Window
            (float x, float y) = DefenderDiceWindowPosition();

            return new Vector2(
                x,
                y + DefenderDiceWindow.Height + WindowSpacing
            );
        }

        private Vector2 DefenderSpriteWindowPosition()
        {
            return GameDriver.ScreenSize / 2 - new Vector2(-WindowSpacing, DefenderSpriteWindow.Height);
        }

        #endregion Defender

        #endregion Window Positions


        

        public void Draw(SpriteBatch spriteBatch)
        {
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

            UserPromptWindow?.Draw(spriteBatch, UserPromptWindowPosition());
        }
    }
}