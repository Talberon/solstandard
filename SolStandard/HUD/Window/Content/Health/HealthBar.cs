using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class HealthBar : IRenderable
    {
        protected IResourcePoint[] Pips;

        private readonly int maxHp;
        private int currentHp;
        private const int MaxPointsPerRow = 10;
        private Vector2 pipSize;
        private Vector2 barSize;

        public HealthBar(int maxHp, int currentHp, Vector2 barSize)
        {
            this.maxHp = maxHp;
            this.currentHp = currentHp;
            PopulatePips();
            UpdatePips();
            BarSize = barSize;
        }

        private Vector2 PipSize
        {
            set
            {
                pipSize = value;

                foreach (IResourcePoint pip in Pips)
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

                float colCount = (Pips.Length > MaxPointsPerRow) ? MaxPointsPerRow : Pips.Length;
                float widthLimit = value.X / colCount;

                float rowCount = Convert.ToSingle(Math.Ceiling((float) Pips.Length / MaxPointsPerRow));
                float heightLimit = value.Y / rowCount;
                PipSize = new Vector2((widthLimit > heightLimit) ? heightLimit : widthLimit);
            }
        }

        private void PopulatePips()
        {
            Pips = new IResourcePoint[maxHp];

            for (int i = 0; i < Pips.Length; i++)
            {
                Pips[i] = new Heart(pipSize);
            }
        }

        private void UpdatePips()
        {
            for (int i = 0; i < Pips.Length; i++)
            {
                Pips[i].Active = i <= (currentHp - 1);
            }
        }

        public int Height
        {
            get { return Convert.ToInt32(barSize.Y); }
        }

        public int Width
        {
            get { return Convert.ToInt32(barSize.X); }
        }

        public void DealDamage(int damage)
        {
            currentHp -= damage;
            UpdatePips();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            Vector2 pipOffset = Vector2.Zero;
            float colCount = (Pips.Length > MaxPointsPerRow) ? MaxPointsPerRow : Pips.Length;

            for (int i = 0; i < Pips.Length; i++)
            {
                Pips[i].Draw(spriteBatch, position + pipOffset);
                pipOffset.X += (barSize.X / colCount);
                if ((i + 1) % MaxPointsPerRow == 0)
                {
                    pipOffset.Y += pipSize.Y;
                    pipOffset.X = 0;
                }
            }
        }
    }
}