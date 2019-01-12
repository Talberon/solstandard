using System;
using SolStandard.Containers.View;
using SolStandard.HUD.Menu;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.Network;

namespace SolStandard.Containers.Contexts
{
    public static class ControlContext
    {
        public static NetworkController ListenForInputs(ControlMapper controlMapper)
        {
            NetworkController networkController = new NetworkController();

            switch (GameContext.CurrentGameState)
            {
                case GameContext.GameState.MainMenu:
                    MainMenuControls(controlMapper, networkController);
                    break;
                case GameContext.GameState.ModeSelect:
                    break;
                case GameContext.GameState.ArmyDraft:
                    break;
                case GameContext.GameState.MapSelect:
                    MapSelectControls(controlMapper, networkController);
                    break;
                case GameContext.GameState.PauseScreen:
                    PauseMenuControl(controlMapper, networkController);
                    break;
                case GameContext.GameState.InGame:
                    MapControls(controlMapper, networkController);
                    break;
                case GameContext.GameState.Results:
                    ResultsControls(controlMapper, networkController);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return networkController;
        }

        private static void ResultsControls(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.Status, PressType.Single))
            {
                networkController.Press(Input.Status);
                GameContext.CurrentGameState = GameContext.GameState.InGame;
            }
        }


        private static void MapSelectControls(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorUp);
                GameContext.MapCursor.MoveCursorInDirection(Direction.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorDown);
                GameContext.MapCursor.MoveCursorInDirection(Direction.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorLeft);
                GameContext.MapCursor.MoveCursorInDirection(Direction.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorRight);
                GameContext.MapCursor.MoveCursorInDirection(Direction.Right);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                networkController.Press(Input.Confirm);
                GameContext.MapSelectContext.SelectMap();
            }
        }

        private static void MainMenuControls(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.CursorDown, PressType.Single))
            {
                networkController.Press(Input.CursorDown);
                GameContext.MainMenuView.MainMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.CursorUp, PressType.Single))
            {
                networkController.Press(Input.CursorUp);
                GameContext.MainMenuView.MainMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                networkController.Press(Input.Confirm);
                GameContext.MainMenuView.MainMenu.SelectOption();
            }
        }

        private static void MapControls(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.Status, PressType.Single))
            {
                networkController.Press(Input.Status);
                GameContext.CurrentGameState = GameContext.GameState.Results;
            }

            switch (GameContext.GameMapContext.CurrentTurnState)
            {
                case GameMapContext.TurnState.SelectUnit:
                    SelectUnitControl(controlMapper, networkController);
                    break;
                case GameMapContext.TurnState.UnitMoving:
                    MoveUnitControl(controlMapper, networkController);
                    break;
                case GameMapContext.TurnState.UnitDecidingAction:
                    DecideActionControl(controlMapper, networkController);
                    break;
                case GameMapContext.TurnState.UnitTargeting:
                    UnitTargetingControl(controlMapper, networkController);
                    break;
                case GameMapContext.TurnState.UnitActing:
                    UnitActingControl(controlMapper, networkController);
                    break;
                case GameMapContext.TurnState.ResolvingTurn:
                    ResolvingTurnControl(controlMapper, networkController);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void CameraControl(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.CenterCamera, PressType.Single))
            {
                networkController.Press(Input.CenterCamera);
                GameContext.MapCamera.CenterCameraToCursor();
            }

            if (controlMapper.Press(Input.LeftTrigger, PressType.DelayedRepeat))
            {
                networkController.Press(Input.LeftTrigger);
                GameContext.MapCamera.ZoomOut(0.1f);
            }

            if (controlMapper.Press(Input.RightTrigger, PressType.DelayedRepeat))
            {
                networkController.Press(Input.RightTrigger);
                GameContext.MapCamera.ZoomIn(0.1f);
            }

            if (controlMapper.Press(Input.LeftBumper, PressType.Single))
            {
                networkController.Press(Input.LeftBumper);
                GameContext.MapCamera.SetZoomLevel(MapCamera.ZoomLevel.Far);
            }

            if (controlMapper.Press(Input.RightBumper, PressType.Single))
            {
                networkController.Press(Input.RightBumper);
                GameContext.MapCamera.SetZoomLevel(MapCamera.ZoomLevel.Default);
            }

            const float cameraPanRateOverride = 5;

            if (controlMapper.Press(Input.CameraDown, PressType.InstantRepeat))
            {
                networkController.Press(Input.CameraDown);
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Down, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.CameraLeft, PressType.InstantRepeat))
            {
                networkController.Press(Input.CameraLeft);
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Left, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.CameraRight, PressType.InstantRepeat))
            {
                networkController.Press(Input.CameraRight);
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Right, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.CameraUp, PressType.InstantRepeat))
            {
                networkController.Press(Input.CameraUp);
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Up, cameraPanRateOverride);
            }


            if (controlMapper.Released(Input.CameraDown))
            {
                networkController.Press(Input.CameraDown);
                GameContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.CameraLeft))
            {
                networkController.Press(Input.CameraLeft);
                GameContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.CameraRight))
            {
                networkController.Press(Input.CameraRight);
                GameContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.CameraUp))
            {
                networkController.Press(Input.CursorUp);
                GameContext.MapCamera.StopMovingCamera();
            }
        }

        private static void SelectUnitControl(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorUp);
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorDown);
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorLeft);
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorRight);
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Right);
            }

            if (controlMapper.Press(Input.ResetToUnit, PressType.Single))
            {
                networkController.Press(Input.ResetToUnit);
                GameContext.GameMapContext.ResetCursorToActiveUnit();
                AssetManager.MapUnitCancelSFX.Play();
            }

            CameraControl(controlMapper, networkController);

            if (controlMapper.Press(Input.Menu, PressType.DelayedRepeat))
            {
                networkController.Press(Input.Menu);
                AssetManager.MenuConfirmSFX.Play();
                GameContext.CurrentGameState = GameContext.GameState.PauseScreen;
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                networkController.Press(Input.Confirm);
                GameContext.GameMapContext.SelectUnitAndStartMoving();
            }
        }

        private static void MoveUnitControl(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorUp);
                GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorDown);
                GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorLeft);
                GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorRight);
                GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Right);
            }

            CameraControl(controlMapper, networkController);

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                networkController.Press(Input.Confirm);
                GameContext.GameMapContext.FinishMoving();
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                networkController.Press(Input.Cancel);
                GameContext.GameMapContext.CancelMove();
            }
        }

        private static void DecideActionControl(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorUp);
                GameContext.GameMapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorDown);
                GameContext.GameMapContext.MoveActionMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.Single))
            {
                networkController.Press(Input.CursorRight);
                GameContext.GameMapContext.ToggleCombatMenu();
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.Single))
            {
                networkController.Press(Input.CursorLeft);
                GameContext.GameMapContext.ToggleCombatMenu();
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                networkController.Press(Input.Confirm);
                GameContext.GameMapContext.SelectActionMenuOption();
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                networkController.Press(Input.Cancel);
                GameContext.GameMapContext.CancelActionMenu();
            }

            if (controlMapper.Press(Input.LeftBumper, PressType.DelayedRepeat))
            {
                networkController.Press(Input.LeftBumper);
                //TODO Decrement current action's value slider (Gold trade, etc.)
                GameContext.GameMapContext.DecrementCurrentAdjustableAction(1);
            }

            if (controlMapper.Press(Input.RightBumper, PressType.DelayedRepeat))
            {
                networkController.Press(Input.RightBumper);
                //TODO Increment current action's value slider (Gold trade, etc.)
                GameContext.GameMapContext.IncrementCurrentAdjustableAction(1);
            }
        }

        private static void PauseMenuControl(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorUp);
                GameContext.GameMapContext.MovePauseMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorDown);
                GameContext.GameMapContext.MovePauseMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                networkController.Press(Input.Confirm);
                GameContext.GameMapContext.SelectPauseMenuOption();
            }

            if (controlMapper.Press(Input.Menu, PressType.Single))
            {
                networkController.Press(Input.Menu);
                GameContext.GameMapContext.PauseScreenView.ChangeMenu(PauseScreenView.PauseMenus.Primary);
                GameContext.CurrentGameState = GameContext.GameState.InGame;
            }
        }

        private static void UnitTargetingControl(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorUp);
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorDown);
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorLeft);
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                networkController.Press(Input.CursorRight);
                GameContext.GameMapContext.MoveCursorOnMap(Direction.Right);
            }

            if (controlMapper.Press(Input.ResetToUnit, PressType.DelayedRepeat))
            {
                networkController.Press(Input.ResetToUnit);
                GameContext.GameMapContext.ResetCursorToActiveUnit();
                GameContext.MapCamera.CenterCameraToCursor();
                AssetManager.MapUnitCancelSFX.Play();
            }

            CameraControl(controlMapper, networkController);

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                networkController.Press(Input.Confirm);
                GameContext.GameMapContext.ExecuteAction();
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                networkController.Press(Input.Cancel);
                GameContext.GameMapContext.CancelAction();
            }
        }

        private static void UnitActingControl(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                networkController.Press(Input.Confirm);
                GameContext.BattleContext.ContinueCombat();
            }
        }

        private static void ResolvingTurnControl(ControlMapper controlMapper, NetworkController networkController)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                networkController.Press(Input.Confirm);
                GameContext.GameMapContext.ResolveTurn();
                AssetManager.MapUnitSelectSFX.Play();
            }
        }
    }
}