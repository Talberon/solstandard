using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steelbreakers.Utility.Graphics;
using Steelbreakers.Utility.Juice;
using Steelbreakers.Utility.Monogame.Assets;

namespace Steelbreakers.Utility.GUI.HUD.Menus
{
    public abstract class MenuOption : IPositionedRenderable, IWindowContent
    {
        public delegate void OnConfirm(MenuOption option);

        public delegate void OnHover(MenuOption option);

        public delegate void OffHover(MenuOption option);

        public Vector2 WindowPositionOffset { get; set; }

        public readonly OnConfirm Confirm;
        public readonly OnHover Hover;
        public readonly OffHover Unhover;
        public JuiceBox JuiceBox => JuicyWindow.JuiceBox;

        protected Window.JuicyWindow JuicyWindow { get; set; }

        protected MenuOption(OnConfirm onConfirm, OnHover onHover, OffHover offHover, Window.JuicyWindow juicyWindow)
        {
            Confirm = onConfirm;
            Hover = onHover;
            Unhover = offHover;
            JuicyWindow = juicyWindow;
            WindowPositionOffset = Vector2.Zero;
        }

        protected MenuOption(OnConfirm onConfirm, OnHover onHover, OffHover offHover, string text, Color windowColor,
            float speed = 3f)
        {
            Confirm = onConfirm;
            Hover = onHover;
            Unhover = offHover;
            JuicyWindow = GenerateJuicyWindow(text, windowColor, speed);
            WindowPositionOffset = Vector2.Zero;
        }

        private static Window.JuicyWindow GenerateJuicyWindow(string optionText, Color windowColor, float defaultSpeed)
        {
            Window optionWindow = new Window.Builder()
                .Content(new RenderText(AssetManager.WindowFont, optionText))
                .WindowColor(windowColor)
                .Build();

            return optionWindow.ToJuicyWindow(defaultSpeed);
        }

        //Window stuff

        public float Width => JuicyWindow.Width;
        public float Height => JuicyWindow.Height;

        public void ImmediatelyOverrideSize(Vector2 newSize)
        {
            JuicyWindow.ResetNewSize(newSize);
        }

        public void SetTextContent(string textToSet)
        {
            JuicyWindow.SetTextContent(textToSet);
        }

        public virtual void Update(GameTime gameTime)
        {
            JuicyWindow.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (JuicyWindow.JuiceBox.CurrentColor.A == 0) return;
            JuicyWindow.Draw(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            if (JuicyWindow.JuiceBox.CurrentColor.A == 0) return;
            JuicyWindow.Draw(spriteBatch, JuiceBox.ApplyShake(coordinates) + WindowPositionOffset);
        }

        public Vector2 TopLeftPoint
        {
            get => JuicyWindow.CurrentPosition;
            set => JuicyWindow.CurrentPosition = value;
        }
    }
}