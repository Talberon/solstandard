using Microsoft.Xna.Framework;

namespace SolStandard.Utility
{
    public static class SpriteResizer
    {
        public static IRenderable TryResizeRenderable(IRenderable renderable, Vector2 newSize)
        {
            IResizable resizable = renderable as IResizable;
            IRenderable resizedRenderable = (resizable != null) ? resizable.Resize(newSize) : renderable;

            return resizedRenderable;
        }
    }
}