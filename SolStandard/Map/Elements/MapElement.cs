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
        private const int BaseSlideSpeed = 10;
        private int SlideSpeed { get; }
        public bool Visible { protected get; set; }

        protected MapElement(IRenderable sprite, Vector2 mapCoordinates)
        {
            Visible = true;
            SlideSpeed = BaseSlideSpeed;
            Sprite = sprite;
            MapCoordinates = mapCoordinates;
            CurrentDrawCoordinates = MapPixelCoordinates;
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

        public IRenderable RenderSprite => Sprite;

        protected Vector2 CurrentDrawCoordinates { get; private set; }

        public Vector2 MapPixelCoordinates => MapCoordinates * GameDriver.CellSize;

        public void SlideToCoordinates(Vector2 coordinates)
        {
            MapCoordinates = coordinates;
        }

        public void SnapToCoordinates(Vector2 coordinates)
        {
            MapCoordinates = coordinates;
            CurrentDrawCoordinates = MapPixelCoordinates;
        }

        protected void UpdateRenderCoordinates()
        {
            CurrentDrawCoordinates =
                UpdateCoordinatesToPosition(CurrentDrawCoordinates, SlideSpeed, MapPixelCoordinates);
        }

        private Vector2 DrawOffset
        {
            get
            {
                var spriteSize = new Vector2(Sprite.Width, Sprite.Height);

                if (spriteSize == GameDriver.CellSizeVector) return Vector2.Zero;

                return GameDriver.CellSizeVector / 2 - spriteSize / 2;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, ElementColor);
        }

        protected virtual void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            UpdateRenderCoordinates();

            if (Visible) Sprite.Draw(spriteBatch, CurrentDrawCoordinates + DrawOffset, colorOverride);
        }
    }
}