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
        private int SlideSpeed { get; set; }
        public bool Visible { protected get; set; }

        protected MapElement(IRenderable sprite, Vector2 mapCoordinates)
        {
            Visible = true;
            SlideSpeed = BaseSlideSpeed;
            Sprite = sprite;
            MapCoordinates = mapCoordinates;
            currentRenderCoordinates = MapPixelCoordinates;
        }

        public static Vector2 UpdateCoordinatesToPosition(Vector2 currentPosition, int slideSpeed, Vector2 destination)
        {
            if (currentPosition == destination) return currentPosition;
            Vector2 newPosition = currentPosition;

            //Slide the cursor sprite to the actual tile coordinates for smooth animation
            bool leftOfDestination = newPosition.X - slideSpeed < destination.X;
            bool rightOfDestination = newPosition.X + slideSpeed > destination.X;
            bool aboveDestination = newPosition.Y - slideSpeed < destination.Y;
            bool belowDestionation = newPosition.Y + slideSpeed > destination.Y;

            if (leftOfDestination) newPosition.X += slideSpeed;
            if (rightOfDestination) newPosition.X -= slideSpeed;
            if (aboveDestination) newPosition.Y += slideSpeed;
            if (belowDestionation) newPosition.Y -= slideSpeed;

            //Don't slide past the cursor's actual coordinates
            bool slidingRightWouldPassMapCoordinates =
                leftOfDestination && (newPosition.X + slideSpeed) > destination.X;
            bool slidingLeftWouldPassMapCoordinates =
                rightOfDestination && (newPosition.X - slideSpeed) < destination.X;
            bool slidingDownWouldPassMapCoordinates =
                aboveDestination && (newPosition.Y + slideSpeed) > destination.Y;
            bool slidingUpWouldPassMapCoordinates =
                belowDestionation && (newPosition.Y - slideSpeed) < destination.Y;

            if (slidingRightWouldPassMapCoordinates) newPosition.X = destination.X;
            if (slidingLeftWouldPassMapCoordinates) newPosition.X = destination.X;
            if (slidingDownWouldPassMapCoordinates) newPosition.Y = destination.Y;
            if (slidingUpWouldPassMapCoordinates) newPosition.Y = destination.Y;

            return newPosition;
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
            currentRenderCoordinates =
                UpdateCoordinatesToPosition(currentRenderCoordinates, SlideSpeed, MapPixelCoordinates);
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