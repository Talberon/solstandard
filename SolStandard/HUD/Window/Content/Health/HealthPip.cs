using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window.Content.Health
{
    public class HealthPip
    {
        public bool Active { get; set; }
        private readonly Color activeColor;
        private readonly Color inactiveColor;
        private readonly ITexture2D whitePixel;

        public HealthPip(ITexture2D whitePixel, Color activeColor, Color inactiveColor)
        {
            this.whitePixel = whitePixel;
            this.activeColor = activeColor;
            this.inactiveColor = inactiveColor;
            Active = true;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 size)
        {
            //Draw Border
            spriteBatch.Draw(whitePixel.MonoGameTexture,
                new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y),
                new Rectangle(0, 0, whitePixel.Width, whitePixel.Height), new Color(50,50,50,200));

            //Draw Inside
            spriteBatch.Draw(whitePixel.MonoGameTexture,
                new Rectangle((int) position.X + 1, (int) position.Y + 1, (int) size.X - 1, (int) size.Y - 1),
                new Rectangle(0, 0, whitePixel.Width - 2, whitePixel.Height - 2), Active ? activeColor : inactiveColor);
        }

        public override string ToString()
        {
            return "Health Pip: { Active=" + Active + "}";
        }
    }
}