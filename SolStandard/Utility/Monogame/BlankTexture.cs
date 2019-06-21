using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.Monogame
{
    public class BlankTexture : ITexture2D
    {
        public BlankTexture()
        {
            Width = 1;
            Height = 1;
        }

        public BlankTexture(int width, int height)
        {
            Width = width;
            Height = height;
        }
        
        public Rectangle Bounds => Rectangle.Empty;

        public int Width { get; }

        public int Height { get; }

        public string Name => "BLANK_TEXTURE";

        public Texture2D MonoGameTexture => new Texture2D(null, 1, 1);
    }
}