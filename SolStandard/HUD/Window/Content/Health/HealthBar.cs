using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class HealthBar : IRenderable
    {
        protected IResourcePoint[] Pips;
        private Vector2 barSize;

        private readonly int maxHp;
        private int currentHp;

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
        }

        private void PopulatePips()
        {
            Pips = new IResourcePoint[maxHp];

            for (int i = 0; i < Pips.Length; i++)
            {
                Pips[i] = new Heart(new Vector2(barSize.Y));
            }
        }

        private void UpdatePips()
        {
            for (int i = 0; i < Pips.Length; i++)
            {
                Pips[i].Active = i <= (currentHp - 1);
            }
        }

        private int PipWidth
        {
            get { return Pips.Length > 0 ? Pips.First().Width : 0; }
        }

        public int Height
        {
            get { return (int) barSize.Y; }
        }

        public int Width
        {
            get { return PipWidth * Pips.Length; }
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
            Vector2 pipOffset = new Vector2(position.X, position.Y);
            foreach (IResourcePoint pip in Pips)
            {
                pip.Draw(spriteBatch, pipOffset);
                pipOffset.X += PipWidth;
            }
        }
    }
}