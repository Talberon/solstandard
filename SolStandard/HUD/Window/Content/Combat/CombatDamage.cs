using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Combat
{
    public class CombatDamage : IRenderable
    {
        private const int MaxRowSize = 5;
        private const int DieSizeAdjustment = 16;
        private readonly List<AttackPoint> atkPoints;
        private CombatDice CombatDice { get; set; }
        private Window CombatDamageWindow { get; set; }

        public CombatDamage(UnitStatistics stats, int bonusDice, int pointSize)
        {
            atkPoints = InitializeAtkPoints(stats, pointSize);
            CombatDice = new CombatDice(stats.Luck, bonusDice, MaxRowSize, pointSize + DieSizeAdjustment);
            CombatDamageWindow = ConstructDamageWindow();
        }

        public int Height
        {
            get { return CombatDamageWindow.Height; }
        }

        public int Width
        {
            get { return CombatDamageWindow.Width; }
        }

        private static List<AttackPoint> InitializeAtkPoints(UnitStatistics stats, int pointSize)
        {
            List<AttackPoint> points = new List<AttackPoint>();
            for (int i = 0; i < stats.Atk; i++)
            {
                points.Add(new AttackPoint(pointSize));
            }

            return points;
        }

        public int CountDamage()
        {
            return atkPoints.Count(point => point.Enabled) + CombatDice.CountFaceValue(Die.FaceValue.Sword, true);
        }

        public int CountShields()
        {
            return CombatDice.CountFaceValue(Die.FaceValue.Shield, true);
        }

        public int CountBlanks()
        {
            return CombatDice.CountFaceValue(Die.FaceValue.Blank, true);
        }

        public void ResolveBlockDie()
        {
            CombatDice.BlockNextDieWithValue(Die.FaceValue.Shield);
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
            atkPoints.ForEach(point => point.Disable(CombatDice.IgnoredDieColor));
        }

        public void DisableAllDiceWithValue(Die.FaceValue value)
        {
            CombatDice.DisableAllDiceWithValue(value);
        }

        public void RollDice()
        {
            CombatDice.RollDice();
        }

        private Window ConstructDamageWindow()
        {
            WindowContentGrid attackPointGrid = ConstructAttackPointGrid();

            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {attackPointGrid},
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

            IRenderable[,] damagePoints = new IRenderable[rows, columns];

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
                        damagePoints[row, column] = new RenderBlank();
                    }
                    else
                    {
                        damagePoints[row, column] = atkPoints[pointCounter];
                    }

                    pointCounter++;
                }
            }

            WindowContentGrid atkPointGrid = new WindowContentGrid(
                damagePoints,
                2,
                HorizontalAlignment.Centered
            );
            return atkPointGrid;
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.Transparent);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            CombatDamageWindow.Draw(spriteBatch, position, colorOverride);
        }
    }
}