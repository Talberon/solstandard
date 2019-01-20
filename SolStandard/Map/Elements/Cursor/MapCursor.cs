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
        private static Vector2 _currentPixelCoordinates;
        private const int BaseSlideSpeed = 10;
        public int SlideSpeed { get; private set; }
        private static Vector2 _cursorSize;

        private enum CursorColor
        {
            White,
            Blue,
            Red
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public MapCursor(SpriteAtlas sprite, Vector2 mapCoordinates, Vector2 mapSize)
        {
            Sprite = sprite;
            MapCoordinates = mapCoordinates;
            _currentPixelCoordinates = mapCoordinates * GameDriver.CellSize;
            SlideSpeed = BaseSlideSpeed;
            this.mapSize = mapSize;
            _cursorSize = new Vector2(sprite.Width, sprite.Height);
        }

        public static Vector2 ScreenCoordinates
        {
            get
            {
                return (_currentPixelCoordinates + GameContext.MapCamera.TargetPosition) *
                       GameContext.MapCamera.TargetZoom;
            }
        }

        public static Vector2 CenterCursorScreenCoordinates
        {
            get
            {
                return (_currentPixelCoordinates + (_cursorSize / 2) + GameContext.MapCamera.TargetPosition) *
                       GameContext.MapCamera.TargetZoom;
            }
        }

        public static Vector2 CurrentPixelCoordinates
        {
            get { return _currentPixelCoordinates; }
        }

        public Vector2 CenterPixelPoint
        {
            get { return CurrentPixelCoordinates + (new Vector2(Sprite.Width, Sprite.Height) / 2); }
        }

        private SpriteAtlas SpriteAtlas
        {
            get { return (SpriteAtlas) Sprite; }
        }

        private void SlideCursorToCoordinates(Vector2 coordinates)
        {
            MapCoordinates = coordinates;
        }

        public void SnapCursorToCoordinates(Vector2 coordinates)
        {
            MapCoordinates = coordinates;
            _currentPixelCoordinates = MapCoordinates * GameDriver.CellSize;
            GameContext.MapCamera.StartMovingCameraToCursor();
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
            GameContext.MapCamera.StartMovingCameraToCursor();
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

            //Slide the cursor sprite to the actual tile coordinates for smooth animation
            bool leftOfDestination = _currentPixelCoordinates.X - SlideSpeed < mapPixelCoordinates.X;
            bool rightOfDestination = _currentPixelCoordinates.X + SlideSpeed > mapPixelCoordinates.X;
            bool aboveDestination = _currentPixelCoordinates.Y - SlideSpeed < mapPixelCoordinates.Y;
            bool belowDestionation = _currentPixelCoordinates.Y + SlideSpeed > mapPixelCoordinates.Y;

            if (leftOfDestination) _currentPixelCoordinates.X += SlideSpeed;
            if (rightOfDestination) _currentPixelCoordinates.X -= SlideSpeed;
            if (aboveDestination) _currentPixelCoordinates.Y += SlideSpeed;
            if (belowDestionation) _currentPixelCoordinates.Y -= SlideSpeed;

            //Don't slide past the cursor's actual coordinates
            bool slidingRightWouldPassMapCoordinates =
                leftOfDestination && (_currentPixelCoordinates.X + SlideSpeed) > mapPixelCoordinates.X;
            bool slidingLeftWouldPassMapCoordinates =
                rightOfDestination && (_currentPixelCoordinates.X - SlideSpeed) < mapPixelCoordinates.X;
            bool slidingDownWouldPassMapCoordinates =
                aboveDestination && (_currentPixelCoordinates.Y + SlideSpeed) > mapPixelCoordinates.Y;
            bool slidingUpWouldPassMapCoordinates =
                belowDestionation && (_currentPixelCoordinates.Y - SlideSpeed) < mapPixelCoordinates.Y;

            if (slidingRightWouldPassMapCoordinates) _currentPixelCoordinates.X = mapPixelCoordinates.X;
            if (slidingLeftWouldPassMapCoordinates) _currentPixelCoordinates.X = mapPixelCoordinates.X;
            if (slidingDownWouldPassMapCoordinates) _currentPixelCoordinates.Y = mapPixelCoordinates.Y;
            if (slidingUpWouldPassMapCoordinates) _currentPixelCoordinates.Y = mapPixelCoordinates.Y;
        }

        private void UpdateCursorTeam()
        {
            switch (GameContext.ActiveUnit.Team)
            {
                case Team.Red:
                    SpriteAtlas.SetCellIndex((int) CursorColor.Red);
                    break;
                case Team.Blue:
                    SpriteAtlas.SetCellIndex((int) CursorColor.Blue);
                    break;
                default:
                    SpriteAtlas.SetCellIndex((int) CursorColor.White);
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
            Sprite.Draw(spriteBatch, _currentPixelCoordinates, colorOverride);
        }

        public override string ToString()
        {
            return "Cursor: {RenderCoordnates:" + _currentPixelCoordinates + ", Sprite:{" + Sprite + "}}";
        }
    }
}