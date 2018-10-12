using System;
using System.Diagnostics;
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
            if (controlMapper.Press(Input.Up, PressType.Repeating))
            {
                mapCursor.MoveCursorInDirection(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.Repeating))
            {
                mapCursor.MoveCursorInDirection(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.Repeating))
            {
                mapCursor.MoveCursorInDirection(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.Repeating))
            {
                mapCursor.MoveCursorInDirection(Direction.Right);
            }

            if (controlMapper.Press(Input.A, PressType.Repeating))
            {
                GameContext.MapSelectContext.SelectMap();
            }
        }

        private static void MainMenuControls(GameControlMapper controlMapper, VerticalMenu verticalMenu)
        {
            if (controlMapper.Press(Input.Down, PressType.Repeating))
            {
                verticalMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.Up, PressType.Repeating))
            {
                verticalMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.A, PressType.Repeating))
            {
                verticalMenu.SelectOption();
            }
        }

        private static void MapControls(GameContext gameContext, GameControlMapper controlMapper, MapCamera mapCamera)
        {
            if (controlMapper.Press(Input.Start, PressType.Repeating))
            {
                gameContext.MapContext.GameMapUI.ToggleVisible();
            }

            if (controlMapper.Press(Input.Down, PressType.Repeating))
            {
                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        gameContext.MapContext.MoveCursorOnMap(Direction.Down);
                        return;
                    case MapContext.TurnState.UnitMoving:
                        gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Down);
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        gameContext.MapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
                        break;
                    case MapContext.TurnState.UnitTargeting:
                        gameContext.MapContext.MoveCursorOnMap(Direction.Down);
                        return;
                    case MapContext.TurnState.UnitActing:
                        break;
                    case MapContext.TurnState.ResolvingTurn:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.Press(Input.Left, PressType.Repeating))
            {
                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        gameContext.MapContext.MoveCursorOnMap(Direction.Left);
                        return;
                    case MapContext.TurnState.UnitMoving:
                        gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Left);
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        break;
                    case MapContext.TurnState.UnitTargeting:
                        gameContext.MapContext.MoveCursorOnMap(Direction.Left);
                        return;
                    case MapContext.TurnState.UnitActing:
                        break;
                    case MapContext.TurnState.ResolvingTurn:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.Press(Input.Right, PressType.Repeating))
            {
                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        gameContext.MapContext.MoveCursorOnMap(Direction.Right);
                        return;
                    case MapContext.TurnState.UnitMoving:
                        gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Right);
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        break;
                    case MapContext.TurnState.UnitTargeting:
                        gameContext.MapContext.MoveCursorOnMap(Direction.Right);
                        return;
                    case MapContext.TurnState.UnitActing:
                        break;
                    case MapContext.TurnState.ResolvingTurn:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.Press(Input.Up, PressType.Repeating))
            {
                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        gameContext.MapContext.MoveCursorOnMap(Direction.Up);
                        return;
                    case MapContext.TurnState.UnitMoving:
                        gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Up);
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        gameContext.MapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
                        break;
                    case MapContext.TurnState.UnitTargeting:
                        gameContext.MapContext.MoveCursorOnMap(Direction.Up);
                        return;
                    case MapContext.TurnState.UnitActing:
                        break;
                    case MapContext.TurnState.ResolvingTurn:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.Press(Input.A, PressType.Repeating))
            {
                Trace.WriteLine("Current Turn State: " + gameContext.MapContext.CurrentTurnState);

                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        gameContext.SelectUnitAndStartMoving();
                        return;

                    case MapContext.TurnState.UnitMoving:
                        gameContext.FinishMoving();
                        return;

                    case MapContext.TurnState.UnitDecidingAction:
                        gameContext.DecideAction();
                        return;

                    case MapContext.TurnState.UnitTargeting:
                        gameContext.ExecuteAction();
                        return;

                    case MapContext.TurnState.UnitActing:
                        gameContext.ContinueCombat();
                        return;

                    case MapContext.TurnState.ResolvingTurn:
                        gameContext.ResolveTurn();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.Press(Input.B, PressType.Repeating))
            {
                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        return;
                    case MapContext.TurnState.UnitMoving:
                        gameContext.CancelMove();
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        return;
                    case MapContext.TurnState.UnitTargeting:
                        gameContext.CancelAction();
                        return;
                    case MapContext.TurnState.UnitActing:
                        return;
                    case MapContext.TurnState.ResolvingTurn:
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.Press(Input.Y, PressType.Repeating))
            {
                MapCamera.CenterCameraToCursor();
            }

            if (controlMapper.Press(Input.X, PressType.Repeating))
            {
                gameContext.MapContext.SlideCursorToActiveUnit();
            }


            if (controlMapper.Press(Input.LeftTrigger, PressType.Repeating))
            {
                //Zoom out
                mapCamera.DecrementZoom(0.1f);
            }

            if (controlMapper.Press(Input.RightTrigger, PressType.Repeating))
            {
                //Zoom in
                mapCamera.IncrementZoom(0.1f);
            }

            if (controlMapper.Press(Input.LeftBumper, PressType.Repeating))
            {
                mapCamera.SetZoomLevel(MapCamera.ZoomLevel.Far);
            }

            if (controlMapper.Press(Input.RightBumper, PressType.Repeating))
            {
                mapCamera.SetZoomLevel(MapCamera.ZoomLevel.Medium);
            }

            const float cameraPanRateOverride = 64;

            if (controlMapper.Press(Input.RsDown, PressType.Repeating))
            {
                MapCamera.MoveCameraInDirection(CameraDirection.Down, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.RsLeft, PressType.Repeating))
            {
                MapCamera.MoveCameraInDirection(CameraDirection.Left, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.RsRight, PressType.Repeating))
            {
                MapCamera.MoveCameraInDirection(CameraDirection.Right, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.RsUp, PressType.Repeating))
            {
                MapCamera.MoveCameraInDirection(CameraDirection.Up, cameraPanRateOverride);
            }
        }
    }
}