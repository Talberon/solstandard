using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu.Options
{
    public abstract class MenuOption : IRenderable
    {
        private readonly Color color;
        private readonly string labelText;
        private readonly Window.Window optionWindow;

        protected MenuOption(Color color, string labelText, ISpriteFont font)
        {
            this.color = color;
            this.labelText = labelText;
            optionWindow = BuildOptionWindow(font);
        }

        private Window.Window BuildOptionWindow(ISpriteFont font)
        {
            return new Window.Window(
                labelText,
                AssetManager.WindowTexture,
                new RenderText(font, labelText),
                color
            );
        }

        public abstract void Execute();

        public int Height
        {
            get { return optionWindow.Height; }
        }

        public int Width
        {
            get { return optionWindow.Width; }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, color);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            optionWindow.Draw(spriteBatch, position, colorOverride);
        }
    }
}