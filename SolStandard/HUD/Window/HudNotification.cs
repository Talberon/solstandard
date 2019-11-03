using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Window
{
    public class HudNotification : IRenderable
    {
        private static readonly Color WindowColor = new Color(40, 40, 40, 180);

        private readonly IRenderable content;
        public int Height => content.Height;
        public int Width => content.Width;
        public Color DefaultColor { get; set; }

        public HudNotification(string notificationMessage)
        {
            content = new Window(new RenderText(AssetManager.WindowFont, notificationMessage), WindowColor);
            DefaultColor = WindowColor;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            content.Draw(spriteBatch, position, colorOverride);
        }

        public IRenderable Clone()
        {
            return content.Clone();
        }
    }
}