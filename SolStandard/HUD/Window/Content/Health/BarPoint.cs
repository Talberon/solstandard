using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window.Content.Health
{
    public class BarPoint : IResourcePoint
    {
        private static readonly Color BorderColor = new Color(50, 50, 50, 200);
        public bool Active { get; set; }
        public Vector2 Size { get; set; }
        private Color activeColor;
        private readonly Color inactiveColor;

        private readonly ITexture2D whitePixel;

        public BarPoint(Vector2 size, Color activeColor, Color inactiveColor)
        {
            Size = size;
            whitePixel = AssetManager.WhitePixel;
            this.activeColor = activeColor;
            this.inactiveColor = inactiveColor;
            Active = true;
        }

        public Color DefaultColor
        {
            get => (Active) ? activeColor : inactiveColor;
            set => activeColor = value;
        }

        public int Height => (int) Size.Y;

        public int Width => (int) Size.X;

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            //Draw Border
            spriteBatch.Draw(whitePixel.MonoGameTexture,
                new Rectangle((int) position.X, (int) position.Y, (int) Size.X, (int) Size.Y),
                new Rectangle(0, 0, whitePixel.Width, whitePixel.Height), BorderColor);

            //Draw Inside
            spriteBatch.Draw(whitePixel.MonoGameTexture,
                new Rectangle((int) position.X + 1, (int) position.Y + 1, (int) Size.X - 1, (int) Size.Y - 1),
                new Rectangle(0, 0, whitePixel.Width - 2, whitePixel.Height - 2),
                (Active) ? colorOverride : inactiveColor);
        }

        public IRenderable Clone()
        {
            return new BarPoint(Size, activeColor, inactiveColor);
        }

        public override string ToString()
        {
            return "Health Pip: { Active=" + Active + "}";
        }
    }
}