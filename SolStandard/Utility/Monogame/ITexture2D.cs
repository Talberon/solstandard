using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.Monogame
{
    /**
     * ITexture2D
     * 
     */
    public interface ITexture2D
    {
        int Width { get; }
        int Height { get; }
        string Name { get; }
        Texture2D MonoGameTexture { get; }
    }
}