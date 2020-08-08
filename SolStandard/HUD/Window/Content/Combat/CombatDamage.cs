using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Combat
{
    public class CombatDamage : IRenderable
    {
        private const int MaxRowSize = 5;
        private const int DieSizeAdjustment = 16;
        private readonly List<AttackPoint> atkPoints;
        private readonly List<BlockPoint> blockPoints;
        private CombatDice CombatDice { get; }
        private Window CombatDamageWindow { get; }
        public Color DefaultColor { get; set; }

        private readonly int damage;
        private readonly int block;
        private readonly int luck;
        private readonly int bonusDamage;
        private readonly int bonusBlock;
        private readonly int bonusLuck;
        private readonly int pointSize;

        public CombatDamage(int damage, int block, int luck, int bonusDamage, int bonusBlock, int bonusLuck, int pointSize)
        {
            this.damage = damage;
            this.block = block;
            this.luck = luck;
            this.bonusDamage = bonusDamage;
            this.bonusBlock = bonusBlock;
            this.bonusLuck = bonusLuck;
            this.pointSize = pointSize;
            atkPoints = InitializeAtkPoints(damage, bonusDamage, pointSize);
            blockPoints = InitializeBlockPoints(block, bonusBlock, pointSize);
            CombatDice = new CombatDice(luck, bonusLuck, MaxRowSize, pointSize + DieSizeAdjustment);
            CombatDamageWindow = ConstructDamageWindow();
            DefaultColor = Color.Transparent;
        }

        public int Height => CombatDamageWindow.Height;

        public int Width => CombatDamageWindow.Width;

        private static List<AttackPoint> InitializeAtkPoints(int atk, int bonusAtk, int pointSize)
        {
            var points = new List<AttackPoint>();
            for (int i = 0; i < atk; i++)
            {
                points.Add(new AttackPoint(pointSize, Color.White));
            }

            for (int i = 0; i < bonusAtk; i++)
            {
                points.Add(new AttackPoint(pointSize, CombatDice.BonusDieColor));
            }

            return points;
        }

        private static List<BlockPoint> InitializeBlockPoints(int block, int bonusBlock, int pointSize)
        {
            var points = new List<BlockPoint>();
            for (int i = 0; i < block; i++)
            {
                points.Add(new BlockPoint(pointSize, Color.White));
            }
            for (int i = 0; i < bonusBlock; i++)
            {
                points.Add(new BlockPoint(pointSize, CombatDice.BonusDieColor));
            }

            return points;
        }

        public int CountDamage()
        {
            return atkPoints.Count(point => point.Enabled) + CombatDice.CountFaceValue(Die.FaceValue.Sword, true);
        }

        public int CountShields()
        {
            return blockPoints.Count(point => point.Enabled) + CombatDice.CountFaceValue(Die.FaceValue.Shield, true);
        }

        public int CountBlanks()
        {
            return CombatDice.CountFaceValue(Die.FaceValue.Blank, true);
        }

        public void ResolveBlockPoint()
        {
            if (CombatDice.CountFaceValue(Die.FaceValue.Shield, true) > 0)
            {
                CombatDice.BlockNextDieWithValue(Die.FaceValue.Shield);
            }
            else if (blockPoints.Count(point => point.Enabled) > 0)
            {
                blockPoints.FindLast(point => point.Enabled).Disable(CombatDice.BlockedDieColor);
            }
        }

        public void BlockAttackPoint()
        {
            if (CountDamage() <= 0) return;

            if (CombatDice.CountFaceValue(Die.FaceValue.Sword, true) > 0)
            {
                CombatDice.BlockNextDieWithValue(Die.FaceValue.Sword);
            }
            else if (atkPoints.Count(point => point.Enabled) > 0)
            {
                atkPoints.FindLast(point => point.Enabled).Disable(CombatDice.BlockedDieColor);
            }
        }

        public void ResolveDamagePoint()
        {
            if (CountDamage() <= 0) return;

            if (CombatDice.CountFaceValue(Die.FaceValue.Sword, true) > 0)
            {
                CombatDice.ResolveDamageNextDieWithValue(Die.FaceValue.Sword);
            }
            else if (atkPoints.Count(point => point.Enabled) > 0)
            {
                atkPoints.FindLast(point => point.Enabled).Disable(CombatDice.DamageDieColor);
            }
        }

        public void DisableAllAttackPoints()
        {
            foreach (AttackPoint point in atkPoints)
            {
                if (point.Enabled)
                {
                    point.Disable(CombatDice.IgnoredDieColor);
                }
            }

            if (CombatDice.CountFaceValue(Die.FaceValue.Sword, true) > 0)
            {
                CombatDice.DisableAllDiceWithValue(Die.FaceValue.Sword);
            }
        }

        public void DisableAllDiceWithValue(Die.FaceValue value)
        {
            CombatDice.DisableAllDiceWithValue(value);
        }

        public void DisableRemainingShields()
        {
            blockPoints.Where(point => point.Enabled).ToList()
                .ForEach(point => point.Disable(CombatDice.IgnoredDieColor));
        }

        public void RollDice()
        {
            CombatDice.RollDice();
        }

        private Window ConstructDamageWindow()
        {
            WindowContentGrid attackPointGrid = ConstructAttackPointGrid();
            WindowContentGrid blockPointGrid = ConstructBlockPointGrid();

            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {attackPointGrid},
                        {blockPointGrid},
                        {CombatDice}
                    },
                    1,
                    HorizontalAlignment.Centered
                ),
                Color.Transparent,
                HorizontalAlignment.Centered
            );
        }

        private WindowContentGrid ConstructAttackPointGrid()
        {
            int rows = Convert.ToInt32(Math.Ceiling((float) atkPoints.Count / MaxRowSize));
            int columns = (MaxRowSize > atkPoints.Count) ? atkPoints.Count : MaxRowSize;

            var damagePoints = new IRenderable[rows, columns];

            int pointCounter = 0;
            bool allPointsCounted = false;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (pointCounter == atkPoints.Count)
                    {
                        allPointsCounted = true;
                    }

                    if (allPointsCounted)
                    {
                        damagePoints[row, column] = RenderBlank.Blank;
                    }
                    else
                    {
                        damagePoints[row, column] = atkPoints[pointCounter];
                    }

                    pointCounter++;
                }
            }

            var atkPointGrid = new WindowContentGrid(
                damagePoints,
                2,
                HorizontalAlignment.Centered
            );
            return atkPointGrid;
        }

        private WindowContentGrid ConstructBlockPointGrid()
        {
            int rows = Convert.ToInt32(Math.Ceiling((float) blockPoints.Count / MaxRowSize));
            int columns = (MaxRowSize > blockPoints.Count) ? blockPoints.Count : MaxRowSize;

            var damagePoints = new IRenderable[rows, columns];

            int pointCounter = 0;
            bool allPointsCounted = false;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (pointCounter == blockPoints.Count)
                    {
                        allPointsCounted = true;
                    }

                    if (allPointsCounted)
                    {
                        damagePoints[row, column] = RenderBlank.Blank;
                    }
                    else
                    {
                        damagePoints[row, column] = blockPoints[pointCounter];
                    }

                    pointCounter++;
                }
            }

            var blockPointGrid = new WindowContentGrid(
                damagePoints,
                2,
                HorizontalAlignment.Centered
            );
            return blockPointGrid;
        }


        public IRenderable Clone()
        {
            return new CombatDamage(damage, block, luck, bonusDamage, bonusBlock, bonusLuck, pointSize);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            CombatDamageWindow.Draw(spriteBatch, position, colorOverride);
        }
    }
}