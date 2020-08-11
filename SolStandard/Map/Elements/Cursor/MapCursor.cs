using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;

namespace SolStandard.Map.Elements.Cursor
{
    public class MapCursor : MapElement
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private enum CursorColor
        {
            White,
            Blue,
            Red
        }

        private const int ButtonIconSize = 16;
        private readonly Vector2 mapSize;
        private static Vector2 _cursorSize;
        public Vector2 CenterPixelPoint => CurrentDrawCoordinates + (new Vector2(Sprite.Width, Sprite.Height) / 2);
        public Vector2 CenterTargetPixelPoint => MapPixelCoordinates + (new Vector2(Sprite.Width, Sprite.Height) / 2);
        private SpriteAtlas SpriteAtlas => (SpriteAtlas) Sprite;

        public Vector2 CenterCursorScreenCoordinates =>
            (CurrentDrawCoordinates + (_cursorSize / 2) + GlobalContext.MapCamera.TargetPosition) *
            GlobalContext.MapCamera.TargetZoom;

        // ReSharper disable once SuggestBaseTypeForParameter
        public MapCursor(SpriteAtlas sprite, Vector2 mapCoordinates, Vector2 mapSize) : base(sprite, mapCoordinates)
        {
            this.mapSize = mapSize;
            _cursorSize = new Vector2(sprite.Width, sprite.Height);
        }

        private static IRenderable ConfirmButton =>
            InputIconProvider.GetInputIcon(Input.Confirm, ButtonIconSize);

        private static IRenderable CancelButton =>
            InputIconProvider.GetInputIcon(Input.Cancel, ButtonIconSize);

        private static IRenderable ItemButton =>
            InputIconProvider.GetInputIcon(Input.PreviewItem, ButtonIconSize);

        private static IRenderable CodexButton =>
            InputIconProvider.GetInputIcon(Input.PreviewUnit, ButtonIconSize);

        public bool CursorIntersectsWindow(IRenderable window, Vector2 windowPixelPosition)
        {
            (float windowLeft, float windowTop) = windowPixelPosition;
            float windowRight = windowLeft + window.Width;
            float windowBottom = windowTop + window.Height;
            (float cursorX, float cursorY) = CenterCursorScreenCoordinates;

            bool cursorWithinWindowBounds = (cursorX >= windowLeft && cursorX <= windowRight) &&
                                            (cursorY >= windowTop && cursorY <= windowBottom);
            return cursorWithinWindowBounds;
        }

        public void SnapCameraAndCursorToCoordinates(Vector2 coordinates)
        {
            SnapToCoordinates(coordinates);
            GlobalContext.MapCamera.SnapCameraCenterToCursor();
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

            PreventCursorLeavingMapBounds();
            AssetManager.MapCursorMoveSFX.Play();
            GlobalContext.MapCamera.StartMovingCameraToCursor();
        }

        public bool IsOnScreen
        {
            get
            {
                Vector2 cursorCoordinates = MapCoordinates * GameDriver.CellSize * GlobalContext.MapCamera.TargetZoom;
                Vector2 screenPosition = -GlobalContext.MapCamera.CurrentPosition;
                Vector2 screenBounds = screenPosition + GameDriver.ScreenSize;

                bool isOnScreen = cursorCoordinates.X > screenPosition.X &&
                                  cursorCoordinates.Y > screenPosition.Y &&
                                  cursorCoordinates.X < screenBounds.X &&
                                  cursorCoordinates.Y < screenBounds.Y;

                Logger.Debug(
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
            switch (GlobalContext.ActivePlayer)
            {
                case PlayerIndex.One:
                    switch (GlobalContext.P1Team)
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
                    switch (GlobalContext.P2Team)
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

            switch (GlobalContext.CurrentGameState)
            {
                case GlobalContext.GameState.EULAConfirm:
                    break;
                case GlobalContext.GameState.MainMenu:
                    break;
                case GlobalContext.GameState.NetworkMenu:
                    break;
                case GlobalContext.GameState.ArmyDraft:
                    break;
                case GlobalContext.GameState.Deployment:
                    DrawDeploymentButtonPrompts(spriteBatch);
                    break;
                case GlobalContext.GameState.MapSelect:
                    DrawMapSelectButtonPrompts(spriteBatch);
                    break;
                case GlobalContext.GameState.PauseScreen:
                    break;
                case GlobalContext.GameState.InGame:
                    DrawInGameButtonPrompts(spriteBatch);
                    break;
                case GlobalContext.GameState.Results:
                    break;
                case GlobalContext.GameState.Codex:
                    break;
                case GlobalContext.GameState.ItemPreview:
                    break;
                case GlobalContext.GameState.Credits:
                    break;
                case GlobalContext.GameState.ControlConfig:
                    break;
                case GlobalContext.GameState.HowToPlay:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void DrawMapSelectButtonPrompts(SpriteBatch spriteBatch)
        {
            if (GlobalContext.MapSelectContext.CanPressConfirm) DrawConfirmButtonPrompt(spriteBatch);
        }

        private void DrawDeploymentButtonPrompts(SpriteBatch spriteBatch)
        {
            if (GlobalContext.DeploymentContext.CanPressConfirm) DrawConfirmButtonPrompt(spriteBatch);
        }

        private void DrawInGameButtonPrompts(SpriteBatch spriteBatch)
        {
            if (GlobalContext.WorldContext.CanPressConfirm) DrawConfirmButtonPrompt(spriteBatch);
            if (GlobalContext.WorldContext.CanPressCancel) DrawCancelButtonPrompt(spriteBatch);
            if (GlobalContext.WorldContext.CanPressPreviewUnit) DrawPreviewUnitButtonPrompt(spriteBatch);
            if (GlobalContext.WorldContext.CanPressPreviewItem) DrawPreviewItemButtonPrompt(spriteBatch);
        }

        private void DrawConfirmButtonPrompt(SpriteBatch spriteBatch)
        {
            ConfirmButton.Draw(spriteBatch,
                CurrentDrawCoordinates + _cursorSize - (new Vector2(ButtonIconSize) / 2));
        }

        private void DrawCancelButtonPrompt(SpriteBatch spriteBatch)
        {
            CancelButton.Draw(spriteBatch,
                CurrentDrawCoordinates + new Vector2(0, _cursorSize.Y) - (new Vector2(ButtonIconSize) / 2));
        }

        private void DrawPreviewUnitButtonPrompt(SpriteBatch spriteBatch)
        {
            CodexButton.Draw(spriteBatch,
                CurrentDrawCoordinates - (new Vector2(ButtonIconSize) / 2));
        }

        private void DrawPreviewItemButtonPrompt(SpriteBatch spriteBatch)
        {
            ItemButton.Draw(spriteBatch,
                CurrentDrawCoordinates + new Vector2(_cursorSize.X, 0) - (new Vector2(ButtonIconSize) / 2));
        }

        public override string ToString()
        {
            return "Cursor: {RenderCoordnates:" + CurrentDrawCoordinates + ", Sprite:{" + Sprite + "}}";
        }
    }
}