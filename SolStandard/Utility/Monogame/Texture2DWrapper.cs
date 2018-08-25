using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.Monogame
{
    public class Texture2DWrapper : ITexture2D
    {
        private readonly Texture2D texture;

        public Texture2DWrapper(Texture2D texture)
        {
            this.texture = texture;
        }

        public Rectangle Bounds
        {
            get { return texture.Bounds; }
        }

        public int Width
        {
            get { return texture.Width; }
        }

        public int Height
        {
            get { return texture.Height; }
        }

        public string Name
        {
            get { return texture.Name; }
        }

        public Texture2D MonoGameTexture
        {
            get { return texture; }
        }
    }
}