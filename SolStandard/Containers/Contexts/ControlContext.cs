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
                    MapControls(gameContext, controlMapper, mapCamera, mapCursor);
                    break;
                case GameContext.GameState.Results:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private static void MapSelectControls(GameControlMapper controlMapper, MapCursor mapCursor)
        {
            if (controlMapper.Up())
            {
                mapCursor.MoveCursorInDirection((Direction.Up));
            }

            if (controlMapper.Down())
            {
                mapCursor.MoveCursorInDirection((Direction.Down));
            }

            if (controlMapper.Left())
            {
                mapCursor.MoveCursorInDirection((Direction.Left));
            }

            if (controlMapper.Right())
            {
                mapCursor.MoveCursorInDirection((Direction.Right));
            }

            if (controlMapper.A())
            {
                GameContext.MapSelectContext.SelectMap();
            }
        }

        private static void MainMenuControls(GameControlMapper controlMapper, VerticalMenu verticalMenu)
        {
            if (controlMapper.Down())
            {
                verticalMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Up())
            {
                verticalMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.A())
            {
                verticalMenu.SelectOption();
            }
        }

        private static void MapControls(GameContext gameContext, GameControlMapper controlMapper, MapCamera mapCamera,
            MapCursor mapCursor)
        {
            if (controlMapper.Start())
            {
                gameContext.MapContext.GameMapUI.ToggleVisible();
            }

            if (controlMapper.Down())
            {
                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        mapCursor.MoveCursorInDirection((Direction.Down));
                        return;
                    case MapContext.TurnState.UnitMoving:
                        gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Down);
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        gameContext.MapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
                        break;
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Down));
                        return;
                    case MapContext.TurnState.UnitActing:
                        break;
                    case MapContext.TurnState.ResolvingTurn:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.Left())
            {
                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        mapCursor.MoveCursorInDirection((Direction.Left));
                        return;
                    case MapContext.TurnState.UnitMoving:
                        gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Left);
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        break;
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Left));
                        return;
                    case MapContext.TurnState.UnitActing:
                        break;
                    case MapContext.TurnState.ResolvingTurn:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.Right())
            {
                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        mapCursor.MoveCursorInDirection((Direction.Right));
                        return;
                    case MapContext.TurnState.UnitMoving:
                        gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Right);
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        break;
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Right));
                        return;
                    case MapContext.TurnState.UnitActing:
                        break;
                    case MapContext.TurnState.ResolvingTurn:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.Up())
            {
                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        mapCursor.MoveCursorInDirection((Direction.Up));
                        return;
                    case MapContext.TurnState.UnitMoving:
                        gameContext.MapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Up);
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        gameContext.MapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
                        break;
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Up));
                        return;
                    case MapContext.TurnState.UnitActing:
                        break;
                    case MapContext.TurnState.ResolvingTurn:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.A())
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

            if (controlMapper.B())
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

            if (controlMapper.LeftTrigger())
            {
                mapCamera.DecreaseZoom(0.1f);
            }

            if (controlMapper.RightTrigger())
            {
                mapCamera.IncreaseZoom(0.1f);
            }

            if (controlMapper.Y())
            {
                mapCamera.CenterCameraToCursor();
            }

            if (controlMapper.X())
            {
                gameContext.MapContext.SlideCursorToActiveUnit();
                
                //FIXME Remove this eventually after debugging is done
                //GameContext.Units.ForEach(unit => unit.DamageUnit(1));
            }


            const float cameraPanRateOverride = 64;

            if (controlMapper.RightStickDown())
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Down, cameraPanRateOverride);
            }

            if (controlMapper.RightStickLeft())
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Left, cameraPanRateOverride);
            }

            if (controlMapper.RightStickRight())
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Right, cameraPanRateOverride);
            }

            if (controlMapper.RightStickUp())
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Up, cameraPanRateOverride);
            }


            if (controlMapper.RightBumper())
            {
                //Zoom in
                mapCamera.ZoomToCursor(4);
            }

            if (controlMapper.LeftBumper())
            {
                //Zoom out
                mapCamera.ZoomToCursor(2);
            }
        }
    }
}