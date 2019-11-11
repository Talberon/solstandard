using System;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;

namespace SolStandard.HUD.Window.Animation
{
    public class RenderableSlide : IRenderableAnimation
    {
        public enum SlideDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        private readonly SlideDirection slideDirection;
        private readonly float distanceToTravel;
        private readonly int slideSpeed;
        public Vector2 CurrentPosition { get; private set; }
        private bool hasMoved;

        public RenderableSlide(SlideDirection slideDirection, float distanceToTravel, int slideSpeed = 10)
        {
            this.slideDirection = slideDirection;
            this.distanceToTravel = distanceToTravel;
            this.slideSpeed = slideSpeed;
            hasMoved = false;
        }

        private static Vector2 SetInitialPosition(Vector2 destinationCoordinates, SlideDirection slideDirection,
            float distanceToTravel)
        {
            (float destX, float destY) = destinationCoordinates;
            Vector2 initialPosition = slideDirection switch
            {
                //Start away from destination
                SlideDirection.Up => new Vector2(destX, destY + distanceToTravel),
                SlideDirection.Down => new Vector2(destX, destY - distanceToTravel),
                SlideDirection.Left => new Vector2(destX + distanceToTravel, destY),
                SlideDirection.Right => new Vector2(destX - distanceToTravel, destY),
                _ => throw new ArgumentOutOfRangeException(nameof(slideDirection), slideDirection, null)
            };

            return initialPosition;
        }

        public void Update(Vector2 destination)
        {
            if (!hasMoved)
            {
                hasMoved = true;
                CurrentPosition = SetInitialPosition(destination, slideDirection, distanceToTravel);
            }

            CurrentPosition =
                MapElement.UpdateCoordinatesToPosition(CurrentPosition, slideSpeed, destination);
        }
    }
}