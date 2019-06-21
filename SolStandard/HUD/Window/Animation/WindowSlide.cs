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

        private readonly Vector2 destinationCoordinates;
        private readonly int slideSpeed;
        public Vector2 CurrentPosition { get; private set; }

        public WindowSlide(SlideDirection slideDirection, float distanceToTravel, Vector2 destinationCoordinates,
            int slideSpeed = 10)
        {
            this.destinationCoordinates = destinationCoordinates;
            this.slideSpeed = slideSpeed;

            switch (slideDirection)
            {
                //Start away from destination
                case SlideDirection.Up:
                    CurrentPosition = new Vector2(
                        destinationCoordinates.X,
                        destinationCoordinates.Y - distanceToTravel
                    );
                    break;
                case SlideDirection.Down:
                    CurrentPosition = new Vector2(
                        destinationCoordinates.X,
                        destinationCoordinates.Y + distanceToTravel
                    );
                    break;
                case SlideDirection.Left:
                    CurrentPosition = new Vector2(
                        destinationCoordinates.X + distanceToTravel,
                        destinationCoordinates.Y
                    );
                    break;
                case SlideDirection.Right:
                    CurrentPosition = new Vector2(
                        destinationCoordinates.X - distanceToTravel,
                        destinationCoordinates.Y
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slideDirection), slideDirection, null);
            }
        }

        public void Update()
        {
            CurrentPosition =
                MapElement.UpdateCoordinatesToPosition(CurrentPosition, slideSpeed, destinationCoordinates);
        }
    }
}