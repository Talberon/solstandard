using System;
using SolStandard.Containers.View;
using SolStandard.HUD.Menu;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
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
                    PauseMenuControl(gameContext, controlMapper);
                    break;
                case GameContext.GameState.InGame:
                    MapControls(gameContext, controlMapper, mapCamera);
                    break;
                case GameContext.GameState.Results:
                    ResultsControls(controlMapper);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ResultsControls(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Select, PressType.Single))
            {
                GameContext.CurrentGameState = GameContext.GameState.InGame;
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
            if (controlMapper.Press(Input.Select, PressType.Single))
            {
                GameContext.CurrentGameState = GameContext.GameState.Results;
            }

            switch (gameContext.GameMapContext.CurrentTurnState)
            {
                case GameMapContext.TurnState.SelectUnit:
                    SelectUnitControl(gameContext, controlMapper, mapCamera);
                    break;
                case GameMapContext.TurnState.UnitMoving:
                    MoveUnitControl(gameContext, controlMapper, mapCamera);
                    break;
                case GameMapContext.TurnState.UnitDecidingAction:
                    DecideActionControl(gameContext, controlMapper);
                    break;
                case GameMapContext.TurnState.UnitTargeting:
                    UnitTargetingControl(gameContext, controlMapper, mapCamera);
                    break;
                case GameMapContext.TurnState.UnitActing:
                    UnitActingControl(gameContext, controlMapper);
                    break;
                case GameMapContext.TurnState.ResolvingTurn:
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
                mapCamera.ZoomOut(0.1f);
            }

            if (controlMapper.Press(Input.RightTrigger, PressType.DelayedRepeat))
            {
                mapCamera.ZoomIn(0.1f);
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
                gameContext.GameMapContext.MoveCursorOnMap(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorOnMap(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorOnMap(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorOnMap(Direction.Right);
            }

            if (controlMapper.Press(Input.X, PressType.Single))
            {
                gameContext.GameMapContext.ResetCursorToActiveUnit();
            }

            CameraControl(controlMapper, mapCamera);

            if (controlMapper.Press(Input.Start, PressType.DelayedRepeat))
            {
                AssetManager.MenuConfirmSFX.Play();
                GameContext.CurrentGameState = GameContext.GameState.PauseScreen;
            }

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
                gameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Right);
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
                gameContext.GameMapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                gameContext.GameMapContext.SelectActionMenuOption();
            }
        }

        private static void PauseMenuControl(GameContext gameContext, GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MovePauseMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MovePauseMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                gameContext.GameMapContext.SelectPauseMenuOption();
            }

            if (controlMapper.Press(Input.Start, PressType.Single))
            {
                gameContext.GameMapContext.PauseMenuUI.ChangeMenu(PauseMenuUI.PauseMenus.Primary);
                GameContext.CurrentGameState = GameContext.GameState.InGame;
            }
        }

        private static void UnitTargetingControl(GameContext gameContext, GameControlMapper controlMapper,
            MapCamera mapCamera)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorOnMap(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorOnMap(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorOnMap(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.MoveCursorOnMap(Direction.Right);
            }

            if (controlMapper.Press(Input.X, PressType.DelayedRepeat))
            {
                gameContext.GameMapContext.ResetCursorToActiveUnit();
                MapCamera.CenterCameraToCursor();
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