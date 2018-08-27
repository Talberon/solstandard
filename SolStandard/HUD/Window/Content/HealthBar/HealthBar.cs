using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.HealthBar
{
    //TODO Use this class as part of the battle UI, also the initiative list (maybe board?)
    public class HealthBar : IRenderable
    {
        private HealthPip[] pips;
        private readonly int pipWidth;
        private readonly Vector2 barSize;

        private readonly int maxHp;
        private int currentHp;
        private static readonly Color ActiveColor = new Color(0, 200, 0);
        private static readonly Color InactiveColor = new Color(140, 20, 20, 200);

        public int Height { get; private set; }
        public int Width { get; private set; }

        public HealthBar(int maxHp, int currentHp, Vector2 barSize)
        {
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
            pips = new HealthPip[maxHp];

            for (int i = 0; i < pips.Length; i++)
            {
                pips[i] = new HealthPip(GameDriver.WhitePixel, ActiveColor, InactiveColor);
            }
        }

        public void DealDamage(int damage)
        {
            //TODO Animation might drive this sort of thing, so keep that in mind
            for (int i = 0; (i < damage) && (i >= 0) && (currentHp > 0); i--)
            {
                pips[(currentHp - 1) - i].Active = false;
            }

            currentHp -= damage;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            Vector2 pipOffset = new Vector2(position.X, position.Y);
            foreach (HealthPip pip in pips)
            {
                pip.Draw(spriteBatch, pipOffset, new Vector2(pipWidth, barSize.Y));
                pipOffset.X += pipWidth;
            }
        }
    }
}