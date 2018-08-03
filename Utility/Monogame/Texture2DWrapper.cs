using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.Monogame
{
    public class Texture2DWrapper : ITexture2D
    {
        Texture2D texture;

        public Texture2DWrapper(Texture2D texture)
        {
            this.texture = texture;
        }

        public Rectangle GetBounds()
        {
            return texture.Bounds;
        }

        public int GetWidth()
        {
            return texture.Width;
        }

        public int GetHeight()
        {
            return texture.Width;
        }

        public Texture2D GetTexture2D()
        {
            return texture;
        }
    }
}
