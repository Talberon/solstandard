using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.Map.Elements
{
    public abstract class MapElement
    {
        protected readonly IRenderable Sprite;
        protected Color ElementColor = Color.White;
        public Vector2 MapCoordinates { get; private set; }
        private Vector2 currentRenderCoordinates;
        private const int BaseSlideSpeed = 10;
        protected int SlideSpeed { private get; set; }
        public bool Visible { protected get; set; }

        protected MapElement(IRenderable sprite, Vector2 mapCoordinates)
        {
            Visible = true;
            SlideSpeed = BaseSlideSpeed;
            Sprite = sprite;
            MapCoordinates = mapCoordinates;
            currentRenderCoordinates = MapPixelCoordinates;
        }

        public IRenderable RenderSprite
        {
            get { return Sprite; }
        }

        public Vector2 CurrentDrawCoordinates
        {
            get { return currentRenderCoordinates; }
        }

        private Vector2 MapPixelCoordinates
        {
            get { return MapCoordinates * GameDriver.CellSize; }
        }

        public void SlideToCoordinates(Vector2 coordinates)
        {
            MapCoordinates = coordinates;
        }

        public void SnapToCoordinates(Vector2 coordinates)
        {
            MapCoordinates = coordinates;
            currentRenderCoordinates = MapPixelCoordinates;
        }

        protected void UpdateRenderCoordinates()
        {
            //Slide the cursor sprite to the actual tile coordinates for smooth animation
            bool leftOfDestination = currentRenderCoordinates.X - SlideSpeed < MapPixelCoordinates.X;
            bool rightOfDestination = currentRenderCoordinates.X + SlideSpeed > MapPixelCoordinates.X;
            bool aboveDestination = currentRenderCoordinates.Y - SlideSpeed < MapPixelCoordinates.Y;
            bool belowDestionation = currentRenderCoordinates.Y + SlideSpeed > MapPixelCoordinates.Y;

            if (leftOfDestination) currentRenderCoordinates.X += SlideSpeed;
            if (rightOfDestination) currentRenderCoordinates.X -= SlideSpeed;
            if (aboveDestination) currentRenderCoordinates.Y += SlideSpeed;
            if (belowDestionation) currentRenderCoordinates.Y -= SlideSpeed;

            //Don't slide past the cursor's actual coordinates
            bool slidingRightWouldPassMapCoordinates =
                leftOfDestination && (currentRenderCoordinates.X + SlideSpeed) > MapPixelCoordinates.X;
            bool slidingLeftWouldPassMapCoordinates =
                rightOfDestination && (currentRenderCoordinates.X - SlideSpeed) < MapPixelCoordinates.X;
            bool slidingDownWouldPassMapCoordinates =
                aboveDestination && (currentRenderCoordinates.Y + SlideSpeed) > MapPixelCoordinates.Y;
            bool slidingUpWouldPassMapCoordinates =
                belowDestionation && (currentRenderCoordinates.Y - SlideSpeed) < MapPixelCoordinates.Y;

            if (slidingRightWouldPassMapCoordinates) currentRenderCoordinates.X = MapPixelCoordinates.X;
            if (slidingLeftWouldPassMapCoordinates) currentRenderCoordinates.X = MapPixelCoordinates.X;
            if (slidingDownWouldPassMapCoordinates) currentRenderCoordinates.Y = MapPixelCoordinates.Y;
            if (slidingUpWouldPassMapCoordinates) currentRenderCoordinates.Y = MapPixelCoordinates.Y;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, ElementColor);
        }

        protected virtual void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            UpdateRenderCoordinates();

            if (Visible) Sprite.Draw(spriteBatch, currentRenderCoordinates, colorOverride);
        }
    }
}