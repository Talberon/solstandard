using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandardTest.Utility.Monogame
{
    public class FakeTexture2D : ITexture2D
    {
        public FakeTexture2D(string name)
        {
            Name = name;
        }

        public Rectangle Bounds => new Rectangle(0, 0, 0, 0);

        public int Height => 1;

        public int Width => 1;

        public string Name { get; }

        public Texture2D MonoGameTexture => null;
    }
}