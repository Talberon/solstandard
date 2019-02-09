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
        
        public Rectangle Bounds
        {
            get { return Rectangle.Empty; }
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public string Name
        {
            get { return "BLANK_TEXTURE"; }
        }

        public Texture2D MonoGameTexture
        {
            get { return new Texture2D(null, 1, 1); }
        }
    }
}