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
        private readonly IRenderable labelContent;
        private readonly Window.Window optionWindow;

        protected MenuOption(Color color, IRenderable labelContent, ISpriteFont font)
        {
            this.color = color;
            this.labelContent = labelContent;
            optionWindow = BuildOptionWindow(font);
        }

        private Window.Window BuildOptionWindow(ISpriteFont font)
        {
            return new Window.Window(
                "Option",
                AssetManager.WindowTexture,
                labelContent,
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