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
        
        public Rectangle GetBounds()
        {
            return new Rectangle(0,0,0,0);
        }

        public int GetHeight()
        {
            return 0;
        }

        public string GetName()
        {
            return name;
        }

        public Texture2D GetTexture2D()
        {
            return null;
        }

        public int GetWidth()
        {
            return 0;
        }
    }
}
