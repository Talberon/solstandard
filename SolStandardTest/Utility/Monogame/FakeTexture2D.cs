using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandardTest.Utility.Monogame
{
    public class FakeTexture2D : ITexture2D
    {
        private readonly string name;

        public FakeTexture2D(string name)
        {
            this.name = name;
        }

        public Rectangle Bounds
        {
            get { return new Rectangle(0, 0, 0, 0); }
        }

        public int Height
        {
            get { return 1; }
        }

        public int Width
        {
            get { return 1; }
        }

        public string Name
        {
            get { return name; }
        }

        public Texture2D MonoGameTexture
        {
            get { return null; }
        }
    }
}