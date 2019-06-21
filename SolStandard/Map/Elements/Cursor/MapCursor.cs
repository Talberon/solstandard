using System;
using System.Diagnostics;
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
        private static Vector2 _cursorSize;

        private enum CursorColor
        {
            White,
            Blue,
            Red
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public MapCursor(SpriteAtlas sprite, Vector2 mapCoordinates, Vector2 mapSize) : base(sprite, mapCoordinates)
        {
            this.mapSize = mapSize;
            _cursorSize = new Vector2(sprite.Width, sprite.Height);
        }

        public Vector2 CenterCursorScreenCoordinates =>
            (CurrentDrawCoordinates + (_cursorSize / 2) + GameContext.MapCamera.TargetPosition) *
            GameContext.MapCamera.TargetZoom;

        public Vector2 CenterPixelPoint => CurrentDrawCoordinates + (new Vector2(Sprite.Width, Sprite.Height) / 2);

        private SpriteAtlas SpriteAtlas => (SpriteAtlas) Sprite;

        public void SnapCursorToCoordinates(Vector2 coordinates)
        {
            SnapToCoordinates(coordinates);
            if (GameContext.GameMapContext != null) GameContext.GameMapContext.UpdateHoverContextWindows();
            GameContext.MapCamera.StartMovingCameraToCursor();
        }

        public void MoveCursorInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    SlideToCoordinates(new Vector2(MapCoordinates.X, MapCoordinates.Y + 1));
                    break;
                case Direction.Right:
                    SlideToCoordinates(new Vector2(MapCoordinates.X + 1, MapCoordinates.Y));
                    break;
                case Direction.Up:
                    SlideToCoordinates(new Vector2(MapCoordinates.X, MapCoordinates.Y - 1));
                    break;
                case Direction.Left:
                    SlideToCoordinates(new Vector2(MapCoordinates.X - 1, MapCoordinates.Y));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            if (GameContext.GameMapContext != null) GameContext.GameMapContext.UpdateHoverContextWindows();
            PreventCursorLeavingMapBounds();
            AssetManager.MapCursorMoveSFX.Play();
            GameContext.MapCamera.StartMovingCameraToCursor();
        }

        public bool IsOnScreen
        {
            get
            {
                Vector2 cursorCoordinates = MapCoordinates * GameDriver.CellSize * GameContext.MapCamera.TargetZoom;
                Vector2 screenPosition = -GameContext.MapCamera.CurrentPosition;
                Vector2 screenBounds = screenPosition + GameDriver.ScreenSize;

                bool isOnScreen = cursorCoordinates.X > screenPosition.X &&
                                  cursorCoordinates.Y > screenPosition.Y &&
                                  cursorCoordinates.X < screenBounds.X &&
                                  cursorCoordinates.Y < screenBounds.Y;

                Trace.WriteLine(
                    $"[isOnScreen={isOnScreen}] Cursor: {cursorCoordinates}, Screen NW: {screenPosition}, Screen SE: {screenBounds}");
                return isOnScreen;
            }
        }

        private void PreventCursorLeavingMapBounds()
        {
            if (MapCoordinates.X < 0)
            {
                SnapToCoordinates(new Vector2(0, MapCoordinates.Y));
            }

            if (MapCoordinates.X >= mapSize.X)
            {
                SnapToCoordinates(new Vector2(mapSize.X - 1, MapCoordinates.Y));
            }

            if (MapCoordinates.Y < 0)
            {
                SnapToCoordinates(new Vector2(MapCoordinates.X, 0));
            }

            if (MapCoordinates.Y >= mapSize.Y)
            {
                SnapToCoordinates(new Vector2(MapCoordinates.X, mapSize.Y - 1));
            }
        }


        private void UpdateCursorTeam()
        {
            switch (GameContext.ActivePlayer)
            {
                case PlayerIndex.One:
                    switch (GameContext.P1Team)
                    {
                        case Team.Blue:
                            SpriteAtlas.SetCellIndex((int) CursorColor.Blue);
                            break;
                        case Team.Red:
                            SpriteAtlas.SetCellIndex((int) CursorColor.Red);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case PlayerIndex.Two:
                    switch (GameContext.P2Team)
                    {
                        case Team.Blue:
                            SpriteAtlas.SetCellIndex((int) CursorColor.Blue);
                            break;
                        case Team.Red:
                            SpriteAtlas.SetCellIndex((int) CursorColor.Red);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

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

        protected override void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            UpdateCursorTeam();
            UpdateRenderCoordinates();
            Sprite.Draw(spriteBatch, CurrentDrawCoordinates, colorOverride);
        }

        public override string ToString()
        {
            return "Cursor: {RenderCoordnates:" + CurrentDrawCoordinates + ", Sprite:{" + Sprite + "}}";
        }
    }
}