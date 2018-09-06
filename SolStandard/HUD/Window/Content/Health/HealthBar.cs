using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class HealthBar : IRenderable
    {
        protected HealthPip[] Pips;
        private int pipWidth;
        private Vector2 barSize;

        private readonly int maxHp;
        private int currentHp;
        private static readonly Color ActiveColor = new Color(30, 200, 30);
        private static readonly Color InactiveColor = new Color(140, 10, 10, 150);

        public int Height { get; private set; }
        public int Width { get; private set; }

        public HealthBar(int maxHp, int currentHp, Vector2 barSize)
        {
            this.maxHp = maxHp;
            this.currentHp = currentHp;
            SetSize(barSize);
        }

        public void SetSize(Vector2 size)
        {
            barSize = size;
            PopulatePips();
            UpdatePips();
            pipWidth = CalculatePipWidth();
            Height = CalculateHeight();
            Width = CalculateWidth();
        }

        private int CalculatePipWidth()
        {
            return (int) Math.Floor(barSize.X / maxHp);
        }

        private int CalculateHeight()
        {
            return (int) barSize.Y;
        }

        private int CalculateWidth()
        {
            return pipWidth * Pips.Length;
        }

        private void PopulatePips()
        {
            Pips = new HealthPip[maxHp];

            for (int i = 0; i < Pips.Length; i++)
            {
                Pips[i] = new HealthPip(GameDriver.WhitePixel, ActiveColor, InactiveColor);
            }
        }

        public void DealDamage(int damage)
        {
            currentHp -= damage;
            UpdatePips();
        }

        private void UpdatePips()
        {
            for (int i = 0; i < Pips.Length; i++)
            {
                Pips[i].Active = i <= (currentHp - 1);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            Vector2 pipOffset = new Vector2(position.X, position.Y);
            foreach (HealthPip pip in Pips)
            {
                pip.Draw(spriteBatch, pipOffset, new Vector2(pipWidth, barSize.Y));
                pipOffset.X += pipWidth;
            }
        }
    }
}