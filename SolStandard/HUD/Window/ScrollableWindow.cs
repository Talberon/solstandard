using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Window
{
    public class ScrollableWindow : IWindow
    {
        private const int BorderPadding = 2;
        private static readonly RasterizerState ScizzorRasterState = new RasterizerState {ScissorTestEnable = true};

        public int Height { get; }
        public int Width { get; }
        public Color DefaultColor { get; set; }
        private IRenderable WindowContents { get; }

        private Vector2 contentOffset;

        public ScrollableWindow(IRenderable content, Vector2 windowSize, Color windowColor)
        {
            WindowContents = content;
            DefaultColor = windowColor;
            contentOffset = Vector2.Zero;
            (float width, float height) = windowSize;
            Width = Convert.ToInt32(width);
            Height = Convert.ToInt32(height);
        }

        public void ScrollWindowContents(Direction direction, float distance)
        {
            switch (direction)
            {
                case Direction.None:
                    return;
                case Direction.Up:
                    if (contentOffset.Y < 0) contentOffset.Y += distance;
                    else contentOffset.Y = 0;
                    break;
                case Direction.Right:
                    if (contentOffset.X > -(WindowContents.Width - Width)) contentOffset.X -= distance;
                    break;
                case Direction.Down:
                    if (contentOffset.Y > -(WindowContents.Height - Height)) contentOffset.Y -= distance;
                    break;
                case Direction.Left:
                    if (contentOffset.X < 0) contentOffset.X += distance;
                    else contentOffset.X = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            var borderPane = new Rectangle(
                Convert.ToInt32(position.X) - BorderPadding,
                Convert.ToInt32(position.Y) - BorderPadding,
                Width + (BorderPadding * 2),
                Height + (BorderPadding * 2)
            );
            spriteBatch.Draw(AssetManager.WindowTexture.MonoGameTexture, borderPane, colorOverride);

            //Restart the spriteBatch to enable clipping to the window
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, ScizzorRasterState);

            Rectangle originalScizzorRect = spriteBatch.GraphicsDevice.ScissorRectangle;


            var innerClipPane = new Rectangle(
                Convert.ToInt32(position.X),
                Convert.ToInt32(position.Y),
                Width,
                Height
            );

            spriteBatch.Draw(AssetManager.WindowTexture.MonoGameTexture, innerClipPane, colorOverride);

            spriteBatch.GraphicsDevice.ScissorRectangle = innerClipPane;

            //Show the contents based on their offset from the top-left corner.
            //Do not render the inner contents beyond the dimensions of the window.
            WindowContents.Draw(spriteBatch, position + contentOffset);

            //Clipping only happens on spriteBatch.End(), so restart the spriteBatch
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = originalScizzorRect;
            //FIXME This class shouldn't know what the spriteBatch properties should be
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
        }


        public IRenderable Clone()
        {
            return new ScrollableWindow(WindowContents, new Vector2(Width, Height), DefaultColor);
        }
    }
}