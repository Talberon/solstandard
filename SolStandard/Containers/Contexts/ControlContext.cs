using System;
using SolStandard.Containers.View;
using SolStandard.HUD.Menu;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;

namespace SolStandard.Containers.Contexts
{
    public static class ControlContext
    {
        public static void ListenForInputs(GameControlMapper controlMapper)
        {
            switch (GameContext.CurrentGameState)
            {
                case GameContext.GameState.MainMenu:
                    MainMenuControls(controlMapper, GameContext.MainMenuView.MainMenu);
                    break;
                case GameContext.GameState.ModeSelect:
                    break;
                case GameContext.GameState.ArmyDraft:
                    break;
                case GameContext.GameState.MapSelect:
                    MapSelectControls(controlMapper);
                    break;
                case GameContext.GameState.PauseScreen:
                    PauseMenuControl(controlMapper);
                    break;
                case GameContext.GameState.InGame:
                    MapControls(controlMapper);
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


        private static void MapSelectControls(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                GameContext.MapCursor.MoveCursorInDirection(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                GameContext.MapCursor.MoveCursorInDirection(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                GameContext.MapCursor.MoveCursorInDirection(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                GameContext.MapCursor.MoveCursorInDirection(Direction.Right);
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

        private static void MapControls(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Select, PressType.Single))
            {
                GameContext.CurrentGameState = GameContext.GameState.Results;
            }

            switch (GameContext.GameMapContext.CurrentTurnState)
            {
                case GameMapContext.TurnState.SelectUnit:
                    SelectUnitControl(controlMapper);
                    break;
                case GameMapContext.TurnState.UnitMoving:
                    MoveUnitControl(controlMapper);
                    break;
                case GameMapContext.TurnState.UnitDecidingAction:
                    DecideActionControl(controlMapper);
                    break;
                case GameMapContext.TurnState.UnitTargeting:
                    UnitTargetingControl(controlMapper);
                    break;
                case GameMapContext.TurnState.UnitActing:
                    UnitActingControl(controlMapper);
                    break;
                case GameMapContext.TurnState.ResolvingTurn:
                    ResolvingTurnControl(controlMapper);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void CameraControl(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Y, PressType.Single))
            {
                GameContext.MapCamera.CenterCameraToCursor();
            }

            if (controlMapper.Press(Input.LeftTrigger, PressType.DelayedRepeat))
            {
                GameContext.MapCamera.ZoomOut(0.1f);
            }

            if (controlMapper.Press(Input.RightTrigger, PressType.DelayedRepeat))
            {
                GameContext.MapCamera.ZoomIn(0.1f);
            }

            if (controlMapper.Press(Input.LeftBumper, PressType.Single))
            {
                GameContext.MapCamera.SetZoomLevel(MapCamera.ZoomLevel.Far);
            }

            if (controlMapper.Press(Input.RightBumper, PressType.Single))
            {
                GameContext.MapCamera.SetZoomLevel(MapCamera.ZoomLevel.Default);
            }

            const float cameraPanRateOverride = 5;

            if (controlMapper.Press(Input.RsDown, PressType.InstantRepeat))
            {
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Down, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.RsLeft, PressType.InstantRepeat))
            {
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Left, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.RsRight, PressType.InstantRepeat))
            {
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Right, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.RsUp, PressType.InstantRepeat))
            {
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Up, cameraPanRateOverride);
            }


            if (controlMapper.Released(Input.RsDown))
            {
                GameContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.RsLeft))
            {
                GameContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.RsRight))
            {
                GameContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.RsUp))
            {
                GameContext.MapCamera.StopMovingCamera();
            }
        }

        private static void SelectUnitControl(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Right);
            }

            if (controlMapper.Press(Input.X, PressType.Single))
            {
                GameContext.GameMapContext.ResetCursorToActiveUnit();
            }

            CameraControl(controlMapper);

            if (controlMapper.Press(Input.Start, PressType.DelayedRepeat))
            {
                AssetManager.MenuConfirmSFX.Play();
                GameContext.CurrentGameState = GameContext.GameState.PauseScreen;
            }

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                GameContext.GameMapContext.SelectUnitAndStartMoving();
            }
        }

        private static void MoveUnitControl(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Right);
            }

            CameraControl(controlMapper);

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                GameContext.GameMapContext.FinishMoving();
            }

            if (controlMapper.Press(Input.B, PressType.Single))
            {
                GameContext.GameMapContext.CancelMove();
            }
        }

        private static void DecideActionControl(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.Right, PressType.Single))
            {
                GameContext.GameMapContext.ToggleCombatMenu();
            }

            if (controlMapper.Press(Input.Left, PressType.Single))
            {
                GameContext.GameMapContext.ToggleCombatMenu();
            }

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                GameContext.GameMapContext.SelectActionMenuOption();
            }
            
            if (controlMapper.Press(Input.B, PressType.Single))
            {
                GameContext.GameMapContext.CancelActionMenu();
            }

            if (controlMapper.Press(Input.LeftBumper, PressType.DelayedRepeat))
            {
                //TODO Decrement current action's value slider (Gold trade, etc.)
                GameContext.GameMapContext.DecrementCurrentAdjustableAction(1);
            }
            if (controlMapper.Press(Input.RightBumper, PressType.DelayedRepeat))
            {
                //TODO Increment current action's value slider (Gold trade, etc.)
                GameContext.GameMapContext.IncrementCurrentAdjustableAction(1);
            }
        }

        private static void PauseMenuControl(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MovePauseMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MovePauseMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                GameContext.GameMapContext.SelectPauseMenuOption();
            }

            if (controlMapper.Press(Input.Start, PressType.Single))
            {
                GameContext.GameMapContext.PauseScreenView.ChangeMenu(PauseScreenView.PauseMenus.Primary);
                GameContext.CurrentGameState = GameContext.GameState.InGame;
            }
        }

        private static void UnitTargetingControl(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Up, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Up);
            }

            if (controlMapper.Press(Input.Down, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Down);
            }

            if (controlMapper.Press(Input.Left, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Left);
            }

            if (controlMapper.Press(Input.Right, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Right);
            }

            if (controlMapper.Press(Input.X, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.ResetCursorToActiveUnit();
                GameContext.MapCamera.CenterCameraToCursor();
            }

            CameraControl(controlMapper);

            if (controlMapper.Press(Input.A, PressType.Single))
            {
                GameContext.GameMapContext.ExecuteAction();
            }

            if (controlMapper.Press(Input.B, PressType.Single))
            {
                GameContext.GameMapContext.CancelAction();
            }
        }

        private static void UnitActingControl(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.A, PressType.Single))
            {
                GameContext.BattleContext.ContinueCombat();
            }
        }

        private static void ResolvingTurnControl(GameControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.A, PressType.Single))
            {
                GameContext.GameMapContext.ResolveTurn();
            }
        }
    }
}