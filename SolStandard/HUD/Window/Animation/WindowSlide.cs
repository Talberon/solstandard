using System;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;

namespace SolStandard.HUD.Window.Animation
{
    public class WindowSlide : IWindowAnimation
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

        public WindowSlide(SlideDirection slideDirection, float distanceToTravel, int slideSpeed = 10)
        {
            this.slideDirection = slideDirection;
            this.distanceToTravel = distanceToTravel;
            this.slideSpeed = slideSpeed;
            hasMoved = false;
        }

        private static Vector2 SetInitialPosition(Vector2 destinationCoordinates, SlideDirection slideDirection,
            float distanceToTravel)
        {
            Vector2 initialPosition;
            (float destX, float destY) = destinationCoordinates;
            switch (slideDirection)
            {
                //Start away from destination
                case SlideDirection.Up:
                    initialPosition = new Vector2(
                        destX,
                        destY - distanceToTravel
                    );
                    break;
                case SlideDirection.Down:
                    initialPosition = new Vector2(
                        destX,
                        destY + distanceToTravel
                    );
                    break;
                case SlideDirection.Left:
                    initialPosition = new Vector2(
                        destX + distanceToTravel,
                        destY
                    );
                    break;
                case SlideDirection.Right:
                    initialPosition = new Vector2(
                        destX - distanceToTravel,
                        destY
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slideDirection), slideDirection, null);
            }

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