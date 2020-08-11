using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.HUD.Neo
{
    public static class HUDExtensions
    {
        public static void DrawRoundedRect(this SpriteBatch spriteBatch, Rectangle destinationRectangle,
            Texture2D texture, int border, Color color)
        {
            // Top left
            spriteBatch.Draw(
                texture,
                new Rectangle(destinationRectangle.Location, new Point(border)),
                new Rectangle(0, 0, border, border),
                color);

            // Top
            spriteBatch.Draw(
                texture,
                new Rectangle(destinationRectangle.Location + new Point(border, 0),
                    new Point(destinationRectangle.Width - border * 2, border)),
                new Rectangle(border, 0, texture.Width - border * 2, border),
                color);

            // Top right
            spriteBatch.Draw(
                texture,
                new Rectangle(destinationRectangle.Location + new Point(destinationRectangle.Width - border, 0),
                    new Point(border)),
                new Rectangle(texture.Width - border, 0, border, border),
                color);

            // Middle left
            spriteBatch.Draw(
                texture,
                new Rectangle(destinationRectangle.Location + new Point(0, border),
                    new Point(border, destinationRectangle.Height - border * 2)),
                new Rectangle(0, border, border, texture.Height - border * 2),
                color);

            // Middle
            spriteBatch.Draw(
                texture,
                new Rectangle(destinationRectangle.Location + new Point(border),
                    destinationRectangle.Size - new Point(border * 2)),
                new Rectangle(border, border, texture.Width - border * 2, texture.Height - border * 2),
                color);

            // Middle right
            spriteBatch.Draw(
                texture,
                new Rectangle(destinationRectangle.Location + new Point(destinationRectangle.Width - border, border),
                    new Point(border, destinationRectangle.Height - border * 2)),
                new Rectangle(texture.Width - border, border, border, texture.Height - border * 2),
                color);

            // Bottom left
            spriteBatch.Draw(
                texture,
                new Rectangle(destinationRectangle.Location + new Point(0, destinationRectangle.Height - border),
                    new Point(border)),
                new Rectangle(0, texture.Height - border, border, border),
                color);

            // Bottom
            spriteBatch.Draw(
                texture,
                new Rectangle(destinationRectangle.Location + new Point(border, destinationRectangle.Height - border),
                    new Point(destinationRectangle.Width - border * 2, border)),
                new Rectangle(border, texture.Height - border, texture.Width - border * 2, border),
                color);

            // Bottom right
            spriteBatch.Draw(
                texture,
                new Rectangle(destinationRectangle.Location + destinationRectangle.Size - new Point(border),
                    new Point(border)),
                new Rectangle(texture.Width - border, texture.Height - border, border, border),
                color);
        }
    }
}