using Microsoft.Xna.Framework;

namespace SolStandard.Utility
{
    public static class SpriteResizer
    {
        public static IRenderable TryResizeRenderable(IRenderable renderable, Vector2 newSize)
        {
            IRenderable resizedRenderable = (renderable is IResizable resizable) ? resizable.Resize(newSize) : renderable;

            return resizedRenderable;
        }
    }
}