using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.Map.Elements.Cursor
{
    public class MapCursor : MapElement
    {
        private readonly Vector2 mapSize;
        private Vector2 renderCoordinates;

        public MapCursor(IRenderable sprite, Vector2 mapCoordinates, Vector2 mapSize)
        {
            Sprite = sprite;
            MapCoordinates = mapCoordinates;
            renderCoordinates = mapCoordinates;
            this.mapSize = mapSize;
        }

        public void MoveCursorInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    MapCoordinates = new Vector2(MapCoordinates.X, MapCoordinates.Y + 1);
                    break;
                case Direction.Right:
                    MapCoordinates = new Vector2(MapCoordinates.X + 1, MapCoordinates.Y);
                    break;
                case Direction.Up:
                    MapCoordinates = new Vector2(MapCoordinates.X, MapCoordinates.Y - 1);
                    break;
                case Direction.Left:
                    MapCoordinates = new Vector2(MapCoordinates.X - 1, MapCoordinates.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            PreventCursorLeavingMapBounds();
        }

        private void PreventCursorLeavingMapBounds()
        {
            if (MapCoordinates.X < 0)
            {
                MapCoordinates = new Vector2(0, MapCoordinates.Y);
            }

            if (MapCoordinates.X >= mapSize.X)
            {
                MapCoordinates = new Vector2(mapSize.X - 1, MapCoordinates.Y);
            }

            if (MapCoordinates.Y < 0)
            {
                MapCoordinates = new Vector2(MapCoordinates.X, 0);
            }

            if (MapCoordinates.Y >= mapSize.Y)
            {
                MapCoordinates = new Vector2(MapCoordinates.X, mapSize.Y - 1);
            }
        }

        private void UpdateRenderCoordinates()
        {
            Vector2 mapPixelCoordinates = MapCoordinates * GameDriver.CellSize;

            const int slideSpeed = 10;
            //Slide the cursor sprite to the actual tile coordinates for smooth animation
            bool leftOfDestination = renderCoordinates.X - slideSpeed < mapPixelCoordinates.X;
            bool rightOfDestination = renderCoordinates.X + slideSpeed > mapPixelCoordinates.X;
            bool aboveDestination = renderCoordinates.Y - slideSpeed < mapPixelCoordinates.Y;
            bool belowDestionation = renderCoordinates.Y + slideSpeed > mapPixelCoordinates.Y;

            if (leftOfDestination) renderCoordinates.X += slideSpeed;
            if (rightOfDestination) renderCoordinates.X -= slideSpeed;
            if (aboveDestination) renderCoordinates.Y += slideSpeed;
            if (belowDestionation) renderCoordinates.Y -= slideSpeed;

            //Don't slide past the cursor's actual coordinates
            bool slidingRightWouldPassMapCoordinates =
                leftOfDestination && (renderCoordinates.X + slideSpeed) > mapPixelCoordinates.X;
            bool slidingLeftWouldPassMapCoordinates =
                rightOfDestination && (renderCoordinates.X - slideSpeed) < mapPixelCoordinates.X;
            bool slidingDownWouldPassMapCoordinates =
                aboveDestination && (renderCoordinates.Y + slideSpeed) > mapPixelCoordinates.Y;
            bool slidingUpWouldPassMapCoordinates =
                belowDestionation && (renderCoordinates.Y - slideSpeed) < mapPixelCoordinates.Y;

            if (slidingRightWouldPassMapCoordinates) renderCoordinates.X = mapPixelCoordinates.X;
            if (slidingLeftWouldPassMapCoordinates) renderCoordinates.X = mapPixelCoordinates.X;
            if (slidingDownWouldPassMapCoordinates) renderCoordinates.Y = mapPixelCoordinates.Y;
            if (slidingUpWouldPassMapCoordinates) renderCoordinates.Y = mapPixelCoordinates.Y;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            UpdateRenderCoordinates();
            Sprite.Draw(spriteBatch, renderCoordinates, Color.White);
        }
        
        public override void Draw(SpriteBatch spriteBatch, Color color)
        {
            UpdateRenderCoordinates();
            Sprite.Draw(spriteBatch, renderCoordinates, color);
        }

        public override string ToString()
        {
            return "Cursor: {RenderCoordnates:" + renderCoordinates + ", Sprite:{" + Sprite + "}}";
        }
    }
}