using System;
using SolStandard.HUD.Menu;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Buttons;

namespace SolStandard.Containers.Contexts
{
    public static class ControlContext
    {
        public static void ListenForInputs(GameContext gameContext, GameControlMapper controlMapper,
            MapCamera mapCamera, MapCursor mapCursor)
        {
            switch (GameContext.CurrentGameState)
            {
                case GameContext.GameState.MainMenu:
                    MainMenuControls(controlMapper, gameContext.MainMenuUI.MainMenu);
                    break;
                case GameContext.GameState.ModeSelect:
                    break;
                case GameContext.GameState.ArmyDraft:
                    break;
                case GameContext.GameState.MapSelect:
                    MapSelectControls(controlMapper, mapCursor);
                    break;
                case GameContext.GameState.PauseScreen:
                    break;
                case GameContext.GameState.InGame:
                    MapControls(gameContext, controlMapper, mapCamera);
                    break;
                case GameContext.GameState.Results:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private static void MapSelectControls(GameControlMapper controlMapper, MapCursor mapCursor)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                mapCursor.MoveCursorInDirection(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                mapCursor.MoveCursorInDirection(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                mapCursor.MoveCursorInDirection(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                mapCursor.MoveCursorInDirection(Direction.Right);
            }

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                GameContext.MapSelectContext.SelectMap();
            }
        }

        private static void MainMenuControls(GameControlMapper controlMapper, VerticalMenu verticalMenu)
        {
            if (controlMapper.Press(Input.Down, PressType.Single))
            {
                verticalMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.Up, PressType.Single))
            {
                verticalMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                verticalMenu.SelectOption();
            }
        }

        private static void MapControls(GameContext gameContext, GameControlMapper controlMapper, MapCamera mapCamera)
        {
            if (controlMapper.Press(Input.Start, PressType.DelayedRepeat))
            {
                gameContext.MapContext.GameMapUI.ToggleVisible();
            }

            switch (gameContext.MapContext.CurrentTurnState)
            {
                case MapContext.TurnState.SelectUnit:
                    SelectUnitControl(gameContext, controlMapper, mapCamera);
                    break;
                case MapContext.TurnState.UnitMoving:
                    MoveUnitControl(gameContext, controlMapper, mapCamera);
                    break;
                case MapContext.TurnState.UnitDecidingAction:
                    DecideActionControl(gameContext, controlMapper);
                    break;
                case MapContext.TurnState.UnitTargeting:
                    UnitTargetingControl(gameContext, controlMapper, mapCamera);
                    break;
                case MapContext.TurnState.UnitActing:
                    UnitActingControl(gameContext, controlMapper);
                    break;
                case MapContext.TurnState.ResolvingTurn:
                    ResolvingTurnControl(gameContext, controlMapper);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void CameraControl(GameControlMapper controlMapper, MapCamera mapCamera)
        {
            if (controlMapper.Press(Input.Y, PressType.Single))
            {
                MapCamera.CenterCameraToCursor();
            }

            if (controlMapper.Press(Input.LeftTrigger, PressType.DelayedRepeat))
            {
                //Zoom out
                mapCamera.DecrementZoom(0.1f);
            }

            if (controlMapper.Press(Input.RightTrigger, PressType.DelayedRepeat))
            {
                //Zoom in
                mapCamera.IncrementZoom(0.1f);
            }

            if (controlMapper.Press(Input.LeftBumper, PressType.Single))
            {
                mapCamera.SetZoomLevel(MapCamera.ZoomLevel.Far);
            }

            if (controlMapper.Press(Input.RightBumper, PressType.Single))
            {
                mapCamera.SetZoomLevel(MapCamera.ZoomLevel.Medium);
            }

            const float cameraPanRateOverride = 5;

            if (controlMapper.Press(Input.RsDown, PressType.InstantRepeat))
            {
                MapCamera.MoveCameraInDirection(CameraDirection.Down, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.RsLeft, PressType.InstantRepeat))
            {
                MapCamera.MoveCameraInDirection(CameraDirection.Left, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.RsRight, PressType.InstantRepeat))
            {
                MapCamera.MoveCameraInDirection(CameraDirection.Right, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.RsUp, PressType.InstantRepeat))
            {
                MapCamera.MoveCameraInDirection(CameraDirection.Up, cameraPanRateOverride);
            }
            
            
            if (controlMapper.Released(Input.RsDown))
            {
                MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.RsLeft))
            {
                MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.RsRight))
            {
                MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.RsUp))
            {
                MapCamera.StopMovingCamera();
            }
        }

        private static void SelectUnitControl(GameContext gameContext, GameControlMapper controlMapper,
            MapCamera mapCamera)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorOnMap(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorOnMap(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorOnMap(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorOnMap(Direction.Right);
            }

            if (controlMapper.Press(Input.X, PressType.Single))
            {
                gameContext.MapContext.SlideCursorToActiveUnit();
            }

            CameraControl(controlMapper, mapCamera);

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                gameContext.SelectUnitAndStartMoving();
            }
        }

        private static void MoveUnitControl(GameContext gameContext, GameControlMapper controlMapper,
            MapCamera mapCamera)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Right);
            }

            CameraControl(controlMapper, mapCamera);

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                gameContext.FinishMoving();
            }

            if (controlMapper.Press(Input.B, PressType.Single))
            {
                gameContext.CancelMove();
            }
        }

        private static void DecideActionControl(GameContext gameContext, GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                gameContext.DecideAction();
            }
        }

        private static void UnitTargetingControl(GameContext gameContext, GameControlMapper controlMapper,
            MapCamera mapCamera)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorOnMap(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorOnMap(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorOnMap(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                gameContext.MapContext.MoveCursorOnMap(Direction.Right);
            }

            if (controlMapper.Press(Input.X, PressType.DelayedRepeat))
            {
                gameContext.MapContext.SlideCursorToActiveUnit();
            }

            CameraControl(controlMapper, mapCamera);

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                gameContext.ExecuteAction();
            }

            if (controlMapper.Press(Input.B, PressType.Single))
            {
                gameContext.CancelAction();
            }
        }

        private static void UnitActingControl(GameContext gameContext, GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.A, PressType.Single))
            {
                gameContext.ContinueCombat();
            }
        }

        private static void ResolvingTurnControl(GameContext gameContext, GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.A, PressType.Single))
            {
                gameContext.ResolveTurn();
            }
        }
    }
}