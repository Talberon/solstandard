using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.Monogame
{
    public class Texture2DWrapper : ITexture2D
    {
        public Texture2DWrapper(Texture2D texture)
        {
            MonoGameTexture = texture;
        }

        public Rectangle Bounds => MonoGameTexture.Bounds;

        public int Width => MonoGameTexture.Width;

        public int Height => MonoGameTexture.Height;

        public string Name => MonoGameTexture.Name;

        public Texture2D MonoGameTexture { get; }
    }
}