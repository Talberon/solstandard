using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Content.Health;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Command
{
    public class MiniCommandPointBar : IRenderable
    {
        private static readonly Color CommandPointColor = new Color(239, 245, 81);

        private readonly List<IResourcePoint> commandPips;
        private int currentCommandPoints;

        private Vector2 barSize;

        private readonly int maxCommandPoints;

        public MiniCommandPointBar(int maxCommandPoints, Vector2 barSize)
        {
            this.maxCommandPoints = maxCommandPoints;
            currentCommandPoints = 0;
            commandPips = GenerateCommandPips(maxCommandPoints);
            UpdateCommandPoints(currentCommandPoints);

            this.barSize = barSize;
        }

        private static void UpdatePips(IReadOnlyList<IResourcePoint> pips, int currentResource)
        {
            for (int i = 0; i < pips.Count; i++)
            {
                pips[i].Active = i <= (currentResource - 1);
            }
        }

        private Vector2 GetPipSize(int maxStatValue)
        {
            return new Vector2((float) Math.Floor(barSize.X / maxStatValue), barSize.Y);
        }

        private List<IResourcePoint> GenerateCommandPips(int maxValue)
        {
            var pips = new List<IResourcePoint>();

            for (int i = 0; i < maxValue; i++)
            {
                pips.Add(new BarPoint(GetPipSize(maxValue), CommandPointColor, Color.Transparent));
            }

            return pips;
        }

        public Vector2 BarSize
        {
            set
            {
                barSize = value;
                commandPips.ForEach(pip => pip.Size = new Vector2(barSize.X / maxCommandPoints, barSize.Y));
            }
        }

        public void UpdateCommandPoints(int commandPoints)
        {
            currentCommandPoints = commandPoints;
            UpdatePips(commandPips, currentCommandPoints);
        }

        public int Height => Convert.ToInt32(barSize.Y);
        public int Width => Convert.ToInt32(barSize.X);

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            var pipOffset = new Vector2(position.X, position.Y);

            foreach (IResourcePoint pip in commandPips)
            {
                pip.Draw(spriteBatch, pipOffset);
                pipOffset.X += pip.Width;
            }
        }

        public Color DefaultColor
        {
            get => CommandPointColor;
            set => throw new InvalidOperationException("Cannot set command bar color.");
        }


        public IRenderable Clone()
        {
            return new MiniHealthBar(maxCommandPoints, currentCommandPoints, barSize);
        }
    }
}