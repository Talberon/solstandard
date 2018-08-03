using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.Monogame
{
    /**
     * ITexture2D
     * 
     */
    public interface ITexture2D
    {
        Rectangle GetBounds();
        int GetWidth();
        int GetHeight();
        Texture2D GetTexture2D();
    }
}
