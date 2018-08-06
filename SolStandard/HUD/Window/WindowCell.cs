using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window
{
    public class WindowCell : IRenderable
    {
        private readonly TileCell cell;
        private readonly Vector2 coordinates;

        public WindowCell(TileCell cell, Vector2 coordinates)
        {
            this.cell = cell;
            this.coordinates = coordinates;
        }

        public int GetHeight()
        {
            return cell.GetHeight();
        }

        public int GetWidth()
        {
            return cell.GetWidth();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            Vector2 relativePosition = new Vector2(coordinates.X + offset.X, coordinates.Y + offset.Y);
            cell.Draw(spriteBatch, relativePosition);
        }
    }
}