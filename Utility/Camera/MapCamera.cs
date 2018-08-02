using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Camera
{
    public enum CameraDirection
    {
        Up,
        Right,
        Down,
        Left
    }
    
    public class MapCamera
    {
        private Vector2 currentCamPosition;
        private float currentZoom;
        
        private Vector2 targetCamPosition;
        private float cameraPanRate;
        
        public MapCamera(float cameraPanRate)
        {
            this.currentCamPosition = new Vector2(0);
            this.targetCamPosition = new Vector2(0);
            this.currentZoom = 1;
            this.cameraPanRate = cameraPanRate;
        }

        public void SetTargetCameraPosition(Vector2 targetPosition)
        {
            this.targetCamPosition = targetPosition;
        }
        
        public void SetCameraZoom(float zoom)
        {
            this.currentZoom = zoom;
        }

        public void MoveCameraInDirection(CameraDirection direction)
        {
            switch (direction)
            {
                case CameraDirection.Down:
                    targetCamPosition.Y -= cameraPanRate;
                    break;
                case CameraDirection.Right:
                    targetCamPosition.X -= cameraPanRate;
                    break;
                case CameraDirection.Up:
                    targetCamPosition.Y += cameraPanRate;
                    break;
                case CameraDirection.Left:
                    targetCamPosition.X += cameraPanRate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        public void PanCameraToTarget()
        {
            if (currentCamPosition.X < targetCamPosition.X)
            {
                if (currentCamPosition.X + cameraPanRate > targetCamPosition.X)
                {
                    currentCamPosition.X = targetCamPosition.X;
                }
                else
                {
                    currentCamPosition.X += cameraPanRate;
                }
            }
            if (currentCamPosition.X > targetCamPosition.X)
            {
                if (currentCamPosition.X - cameraPanRate < targetCamPosition.X)
                {
                    currentCamPosition.X = targetCamPosition.X;
                }
                else
                {
                    currentCamPosition.X -= cameraPanRate;
                }
            }
            if (currentCamPosition.Y < targetCamPosition.Y)
            {
                if (currentCamPosition.Y + cameraPanRate > targetCamPosition.Y)
                {
                    currentCamPosition.Y = targetCamPosition.Y;
                }
                else
                {
                    currentCamPosition.Y += cameraPanRate;
                }
            }
            if (currentCamPosition.Y > targetCamPosition.Y)
            {
                if (currentCamPosition.Y - cameraPanRate < targetCamPosition.Y)
                {
                    currentCamPosition.Y = targetCamPosition.Y;
                }
                else
                {
                    currentCamPosition.Y -= cameraPanRate;
                }
            }
        }
        
        public Matrix GetCameraMatrix()
        {
            return Matrix.CreateTranslation(currentCamPosition.X, currentCamPosition.Y, 0) *
                Matrix.CreateScale(new Vector3(currentZoom, currentZoom, 1));
        }
    }
}