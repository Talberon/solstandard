using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Animation;
using SolStandard.Utility;

namespace SolStandard.HUD.Window
{
    public class AnimatedWindow : IRenderable
    {
        private Window Window { get; }
        private IWindowAnimation WindowAnimation { get; }
        
        public Color DefaultColor
        {
            private get => Window.DefaultColor;
            set => Window.DefaultColor = value;
        }

        public AnimatedWindow(Window window, IWindowAnimation windowAnimation)
        {
            Window = window;
            WindowAnimation = windowAnimation;
        }

        public int Height => Window.Height;
        public int Width => Window.Width;

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            WindowAnimation.Update(position);
            Window.Draw(spriteBatch, WindowAnimation.CurrentPosition, colorOverride);
        }

        public IRenderable Clone()
        {
            return new AnimatedWindow(Window, WindowAnimation);
        }
    }
}