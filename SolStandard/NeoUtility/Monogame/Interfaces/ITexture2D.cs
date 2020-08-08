using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.NeoUtility.Monogame.Interfaces
{
    public interface ITexture2D
    {
        Rectangle Bounds { get; }
        int Width { get; }
        int Height { get; }
        string Name { get; }
        Texture2D MonoGameTexture { get; }
    }
}