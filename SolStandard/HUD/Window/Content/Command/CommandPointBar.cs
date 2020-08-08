using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content.Health;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Command
{
    public class CommandPointBar : IRenderable, IResizable
    {
        private readonly List<IResourcePoint> commandPips;
        private readonly int maxCommandPoints;
        private readonly int currentCommandPoints;

        private const int MaxPointsPerRow = 5;
        private Vector2 pipSize;
        private Vector2 barSize;
        public Color DefaultColor { get; set; }

        public CommandPointBar(int maxCommandPoints, int currentCommandPoints, Vector2 barSize)
        {
            this.maxCommandPoints = maxCommandPoints;
            this.currentCommandPoints = currentCommandPoints;
            commandPips = GenerateCommandPips(maxCommandPoints);
            UpdatePips(commandPips, currentCommandPoints);

            BarSize = barSize;
            DefaultColor = Color.White;
        }

        private void AddCommandPoint(ICollection<IResourcePoint> points)
        {
            points.Add(
                new ResourcePoint(
                    pipSize,
                    UnitStatistics.GetSpriteAtlas(Stats.CommandPoints),
                    UnitStatistics.GetSpriteAtlas(Stats.EmptyCommandPoints)
                )
            );
        }

        private List<IResourcePoint> GenerateCommandPips(int maxPips)
        {
            var pips = new List<IResourcePoint>();

            for (int i = 0; i < maxPips; i++)
            {
                AddCommandPoint(pips);
            }

            return pips;
        }


        private static void UpdatePips(IReadOnlyList<IResourcePoint> pips, int currentResource)
        {
            for (int i = 0; i < pips.Count; i++)
            {
                pips[i].Active = i <= (currentResource - 1);
            }
        }


        private Vector2 PipSize
        {
            set
            {
                pipSize = value;

                foreach (IResourcePoint pip in commandPips)
                {
                    pip.Size = value;
                }
            }
        }

        public Vector2 BarSize
        {
            set
            {
                barSize = value;

                float colCount = ColumnCount;
                float widthLimit = value.X / colCount;
                float heightLimit = value.Y / PipRowCount;

                PipSize = new Vector2((widthLimit > heightLimit) ? heightLimit : widthLimit);
            }
        }

        private float ColumnCount => (commandPips.Count > MaxPointsPerRow) ? MaxPointsPerRow : commandPips.Count;
        private float PipRowCount => Convert.ToSingle(Math.Ceiling((float) commandPips.Count / MaxPointsPerRow));
        public int Height => Convert.ToInt32(barSize.Y);
        public int Width => Convert.ToInt32(barSize.X);

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            Vector2 pipOffset = Vector2.Zero;

            DrawPips(spriteBatch, position, commandPips, pipOffset, colorOverride);
        }


        private void DrawPips(SpriteBatch spriteBatch, Vector2 position, List<IResourcePoint> points, Vector2 pipOffset,
            Color colorOverride)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Draw(spriteBatch, position + pipOffset, colorOverride);

                pipOffset.X += (barSize.X / ColumnCount);

                if ((i + 1) % MaxPointsPerRow != 0) continue;

                pipOffset.Y += pipSize.Y;
                pipOffset.X = 0;
            }
        }

        public IRenderable Resize(Vector2 newSize)
        {
            return new CommandPointBar(maxCommandPoints, currentCommandPoints, newSize);
        }

        public IRenderable Clone()
        {
            return new CommandPointBar(maxCommandPoints, currentCommandPoints, barSize);
        }
    }
}