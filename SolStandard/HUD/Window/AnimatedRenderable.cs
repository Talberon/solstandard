using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Animation;
using SolStandard.Utility;

namespace SolStandard.HUD.Window
{
    public class AnimatedRenderable : IRenderable
    {
        private IRenderableAnimation RenderableAnimation { get; }
        private IRenderable Content { get; }
        public int Height => Content.Height;
        public int Width => Content.Width;

        public AnimatedRenderable(IRenderable content, IRenderableAnimation renderableAnimation)
        {
            Content = content;
            RenderableAnimation = renderableAnimation;
        }

        public Color DefaultColor
        {
            get => Content.DefaultColor;
            set => Content.DefaultColor = value;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            RenderableAnimation.Update(position);
            Content.Draw(spriteBatch, RenderableAnimation.CurrentPosition);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            RenderableAnimation.Update(position);
            Content.Draw(spriteBatch, RenderableAnimation.CurrentPosition, colorOverride);
        }

        public IRenderable Clone()
        {
            return new AnimatedRenderable(Content, RenderableAnimation);
        }
    }
}