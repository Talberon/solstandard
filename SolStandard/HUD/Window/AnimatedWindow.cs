using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Animation;
using SolStandard.Utility;

namespace SolStandard.HUD.Window
{
    public class AnimatedWindow : IWindow
    {
        private Window Window { get; }
        private IWindowAnimation WindowAnimation { get; }

        public Color DefaultColor
        {
            set => Window.DefaultColor = value;
        }


        public AnimatedWindow(Window window, IWindowAnimation windowAnimation)
        {
            Window = window;
            WindowAnimation = windowAnimation;
        }

        public int Height => Window.Height;
        public int Width => Window.Width;


        public void FadeAtRate(int newOpacity, int fadeRatePerFrame)
        {
            Window.FadeAtRate(newOpacity, fadeRatePerFrame);
        }

        public void ResetOpacity()
        {
            Window.ResetOpacity();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            WindowAnimation.Update(position);
            Window.Draw(spriteBatch, WindowAnimation.CurrentPosition);
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