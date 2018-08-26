using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window.Content.HealthBar
{
    public class HealthBar : IRenderable
    {
        private HealthPip[] pips;
        private readonly ITexture2D whitePixel;
        private readonly int pipWidth;
        private readonly Vector2 barSize;

        private readonly int maxHp;
        private int currentHp;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public HealthBar(ITexture2D whitePixel, int maxHp, int currentHp, Vector2 barSize)
        {
            this.whitePixel = whitePixel;
            this.maxHp = maxHp;
            this.currentHp = currentHp;
            this.barSize = barSize;
            PopulatePips();
            pipWidth = CalculatePipWidth();
            Height = CalculateHeight();
            Width = CalculateWidth();
        }

        private int CalculatePipWidth()
        {
            return (int) Math.Round(barSize.X / maxHp, MidpointRounding.AwayFromZero);
        }

        private int CalculateHeight()
        {
            return (int) barSize.Y;
        }

        private int CalculateWidth()
        {
            return pipWidth * pips.Length;
        }

        private void PopulatePips()
        {
            pips = new HealthPip[this.maxHp];

            for (int i = 0; i < pips.Length; i++)
            {
                pips[i] = new HealthPip(whitePixel, new Color(0, 200, 0), new Color(40, 40, 40, 200));
            }
        }

        public void DealDamage(int damage)
        {
            //TODO Animation might drive this sort of thing, so keep that in mind
            for (int i = 0; (i < damage) && (i >= 0); i--)
            {
                pips[currentHp - i].Active = false;
            }

            currentHp -= damage;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            Vector2 pipOffset = new Vector2(0);
            foreach (HealthPip pip in pips)
            {
                pip.Draw(spriteBatch, pipOffset, new Vector2(pipWidth, barSize.Y));
                pipOffset.X += pipWidth;
            }
        }
    }
}