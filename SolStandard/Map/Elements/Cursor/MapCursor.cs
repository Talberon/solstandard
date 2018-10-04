using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Map.Elements.Cursor
{
    public class MapCursor : MapElement
    {
        private readonly Vector2 mapSize;
        private Vector2 currentPixelCoordinates;
        private const int BaseSlideSpeed = 10;
        public int SlideSpeed { get; private set; }

        private enum CursorColor
        {
            None,
            White,
            Blue,
            Red
        }

        public MapCursor(SpriteAtlas sprite, Vector2 mapCoordinates, Vector2 mapSize)
        {
            Sprite = sprite;
            MapCoordinates = mapCoordinates;
            currentPixelCoordinates = mapCoordinates * GameDriver.CellSize;
            SlideSpeed = BaseSlideSpeed;
            this.mapSize = mapSize;
        }

        public Vector2 CurrentPixelCoordinates
        {
            get { return currentPixelCoordinates; }
        }

        public Vector2 CenterPixelPoint
        {
            get { return CurrentPixelCoordinates + (new Vector2(Sprite.Width, Sprite.Height) / 2); }
        }

        private SpriteAtlas SpriteAtlas
        {
            get { return (SpriteAtlas) Sprite; }
        }


        public void SlideCursorToCoordinates(Vector2 coordinates)
        {
            MapCoordinates = coordinates;
        }

        public void SnapCursorToCoordinates(Vector2 coordinates)
        {
            MapCoordinates = coordinates;
            currentPixelCoordinates = MapCoordinates * GameDriver.CellSize;
        }

        public void MoveCursorInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    SlideCursorToCoordinates(new Vector2(MapCoordinates.X, MapCoordinates.Y + 1));
                    break;
                case Direction.Right:
                    SlideCursorToCoordinates(new Vector2(MapCoordinates.X + 1, MapCoordinates.Y));
                    break;
                case Direction.Up:
                    SlideCursorToCoordinates(new Vector2(MapCoordinates.X, MapCoordinates.Y - 1));
                    break;
                case Direction.Left:
                    SlideCursorToCoordinates(new Vector2(MapCoordinates.X - 1, MapCoordinates.Y));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            PreventCursorLeavingMapBounds();
            AssetManager.MapCursorMoveSFX.Play();
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

            //TODO Slide faster the further the cursor is away from its destination

            //Slide the cursor sprite to the actual tile coordinates for smooth animation
            bool leftOfDestination = currentPixelCoordinates.X - SlideSpeed < mapPixelCoordinates.X;
            bool rightOfDestination = currentPixelCoordinates.X + SlideSpeed > mapPixelCoordinates.X;
            bool aboveDestination = currentPixelCoordinates.Y - SlideSpeed < mapPixelCoordinates.Y;
            bool belowDestionation = currentPixelCoordinates.Y + SlideSpeed > mapPixelCoordinates.Y;

            if (leftOfDestination) currentPixelCoordinates.X += SlideSpeed;
            if (rightOfDestination) currentPixelCoordinates.X -= SlideSpeed;
            if (aboveDestination) currentPixelCoordinates.Y += SlideSpeed;
            if (belowDestionation) currentPixelCoordinates.Y -= SlideSpeed;

            //Don't slide past the cursor's actual coordinates
            bool slidingRightWouldPassMapCoordinates =
                leftOfDestination && (currentPixelCoordinates.X + SlideSpeed) > mapPixelCoordinates.X;
            bool slidingLeftWouldPassMapCoordinates =
                rightOfDestination && (currentPixelCoordinates.X - SlideSpeed) < mapPixelCoordinates.X;
            bool slidingDownWouldPassMapCoordinates =
                aboveDestination && (currentPixelCoordinates.Y + SlideSpeed) > mapPixelCoordinates.Y;
            bool slidingUpWouldPassMapCoordinates =
                belowDestionation && (currentPixelCoordinates.Y - SlideSpeed) < mapPixelCoordinates.Y;

            if (slidingRightWouldPassMapCoordinates) currentPixelCoordinates.X = mapPixelCoordinates.X;
            if (slidingLeftWouldPassMapCoordinates) currentPixelCoordinates.X = mapPixelCoordinates.X;
            if (slidingDownWouldPassMapCoordinates) currentPixelCoordinates.Y = mapPixelCoordinates.Y;
            if (slidingUpWouldPassMapCoordinates) currentPixelCoordinates.Y = mapPixelCoordinates.Y;
        }

        private void UpdateCursorTeam()
        {
            switch (GameContext.ActiveUnit.Team)
            {
                case Team.Red:
                    SpriteAtlas.CellIndex = (int) CursorColor.Red;
                    break;
                case Team.Blue:
                    SpriteAtlas.CellIndex = (int) CursorColor.Blue;
                    break;
                default:
                    SpriteAtlas.CellIndex = (int) CursorColor.White;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Color.White);
        }

        public override void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            UpdateCursorTeam();
            UpdateRenderCoordinates();
            Sprite.Draw(spriteBatch, currentPixelCoordinates, colorOverride);
        }

        public override string ToString()
        {
            return "Cursor: {RenderCoordnates:" + currentPixelCoordinates + ", Sprite:{" + Sprite + "}}";
        }
    }
}