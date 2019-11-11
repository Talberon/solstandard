using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;
using SolStandard.Utility;

namespace SolStandard.HUD.Menu.Options
{
    public abstract class MenuOption : IRenderable
    {
        public Color DefaultColor { get; set; }
        protected IRenderable LabelContent { get; set; }
        private Window.Window optionWindow;
        private readonly HorizontalAlignment horizontalAlignment;

        protected MenuOption(IRenderable labelContent, Color color,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
        {
            DefaultColor = color;
            LabelContent = labelContent;
            this.horizontalAlignment = horizontalAlignment;
            optionWindow = BuildOptionWindow();
        }

        private Window.Window BuildOptionWindow()
        {
            return new Window.Window(
                LabelContent,
                DefaultColor,
                horizontalAlignment
            );
        }

        public virtual void Refresh()
        {
            optionWindow = BuildOptionWindow();
        }

        public abstract void Execute();

        protected void UpdateLabel(IRenderable newContent)
        {
            LabelContent = newContent;
            optionWindow = BuildOptionWindow();
        }

        public int Height
        {
            get => optionWindow.Height;
            set => optionWindow.Height = value;
        }

        public int Width
        {
            get => optionWindow.Width;
            set => optionWindow.Width = value;
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