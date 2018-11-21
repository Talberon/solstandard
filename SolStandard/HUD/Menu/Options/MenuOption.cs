using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;
using SolStandard.Utility;

namespace SolStandard.HUD.Menu.Options
{
    public abstract class MenuOption : IRenderable
    {
        public Color DefaultColor { get; set; }
        protected IRenderable LabelContent;
        private Window.Window optionWindow;

        protected MenuOption(IRenderable labelContent, Color color)
        {
            DefaultColor = color;
            LabelContent = labelContent;
            optionWindow = BuildOptionWindow();
        }

        private Window.Window BuildOptionWindow()
        {
            return new Window.Window(
                LabelContent,
                DefaultColor,
                HorizontalAlignment.Left
            );
        }

        public abstract void Execute();

        public void UpdateLabel(IRenderable newContent)
        {
            LabelContent = newContent;
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
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            optionWindow.Draw(spriteBatch, position, colorOverride);
        }

        public abstract IRenderable Clone();
    }
}