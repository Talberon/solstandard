using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options
{
    public abstract class MenuOption : IRenderable
    {
        private readonly Color color;
        private IRenderable labelContent;
        private Window.Window optionWindow;

        protected MenuOption(IRenderable labelContent, Color color)
        {
            this.color = color;
            this.labelContent = labelContent;
            optionWindow = BuildOptionWindow();
        }

        private Window.Window BuildOptionWindow()
        {
            return new Window.Window(
                labelContent,
                color,
                HorizontalAlignment.Left
            );
        }

        public abstract void Execute();

        public void UpdateLabel(IRenderable newContent)
        {
            labelContent = newContent;
            optionWindow = BuildOptionWindow();
        }
        
        public int Height
        {
            get { return optionWindow.Height; }
            set { optionWindow.Height = value; }
        }

        public int Width
        {
            get { return optionWindow.Width; }
            set { optionWindow.Width = value; }
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