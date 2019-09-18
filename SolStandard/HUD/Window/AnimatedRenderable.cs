using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Animation;
using SolStandard.Utility;

namespace SolStandard.HUD.Window
{
    public class AnimatedRenderable : IWindow
    {
        private IRenderableAnimation RenderableAnimation { get; }
        private IRenderable Window { get; }
        public int Height => Window.Height;
        public int Width => Window.Width;

        public AnimatedRenderable(IRenderable window, IRenderableAnimation renderableAnimation)
        {
            Window = window;
            RenderableAnimation = renderableAnimation;
        }

        public Color DefaultColor
        {
            get => Window.DefaultColor;
            set => Window.DefaultColor = value;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            RenderableAnimation.Update(position);
            Window.Draw(spriteBatch, RenderableAnimation.CurrentPosition);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            RenderableAnimation.Update(position);
            Window.Draw(spriteBatch, RenderableAnimation.CurrentPosition, colorOverride);
        }

        public IRenderable Clone()
        {
            return new AnimatedRenderable(Window, RenderableAnimation);
        }
    }
}