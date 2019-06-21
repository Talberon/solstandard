using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window
{
    /**
     * Toast Windows appear on the GameMap and are used to provide contextual information before disappearing.
     */
    public class ToastWindow
    {
        private static readonly Color ToastColor = new Color(40, 40, 40, 180);


        private int MaxLifetimeInFrames { get; }
        private int CurrentLifetimeInFrames { get; set; }

        private readonly Window window;

        private readonly Vector2 originalCoordinates;
        private Vector2 offsetCoordinates;

        public bool Expired { get; private set; }

        public ToastWindow(IRenderable windowContent, Vector2 originalCoordinates, int maxLifetimeInFrames)
        {
            window = new Window(windowContent, ToastColor);
            MaxLifetimeInFrames = maxLifetimeInFrames;
            this.originalCoordinates = originalCoordinates;
            CurrentLifetimeInFrames = 0;

            offsetCoordinates = Vector2.Zero;
            Expired = false;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (Expired) return;

            if (CurrentLifetimeInFrames > MaxLifetimeInFrames) Expired = true;

            CurrentLifetimeInFrames++;
            offsetCoordinates.Y -= 0.2f;

            window.Draw(spriteBatch, originalCoordinates + offsetCoordinates, ToastColor);
        }
    }
}