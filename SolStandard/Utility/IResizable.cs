using Microsoft.Xna.Framework;

namespace SolStandard.Utility
{
    public interface IResizable
    {
        IRenderable Resize(Vector2 newSize);
    }
}