using System;
using SolStandard.Containers.View;
using SolStandard.HUD.Menu;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;

namespace SolStandard.Containers.Contexts
{
    public static class ControlContext
    {
        public static void ListenForInputs(ControlMapper controlMapper)
        {
            switch (GameContext.CurrentGameState)
            {
                case GameContext.GameState.MainMenu:
                    MainMenuControls(controlMapper);
                    break;
                case GameContext.GameState.NetworkMenu:
                    NetworkMenuControls(controlMapper);
                    break;
                case GameContext.GameState.Deployment:
                    DeploymentControls(controlMapper);
                    break;
                case GameContext.GameState.ArmyDraft:
                    DraftMenuControls(controlMapper);
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

        private static void MainMenuControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorDown, PressType.Single))
            {
                GameContext.MainMenuView.MainMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.CursorUp, PressType.Single))
            {
                GameContext.MainMenuView.MainMenu.MoveMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GameContext.MainMenuView.MainMenu.SelectOption();
            }
        }

        private static void NetworkMenuControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GameContext.NetworkMenuView.Exit();
            }

            if (GameContext.NetworkMenuView.DialMenu == null) return;

            if (controlMapper.Press(Input.CursorUp, PressType.Single))
            {
                GameContext.NetworkMenuView.DialMenu.MoveMenuCursor(TwoDimensionalMenu.MenuCursorDirection.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.Single))
            {
                GameContext.NetworkMenuView.DialMenu.MoveMenuCursor(TwoDimensionalMenu.MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.Single))
            {
                GameContext.NetworkMenuView.DialMenu.MoveMenuCursor(TwoDimensionalMenu.MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.Single))
            {
                GameContext.NetworkMenuView.DialMenu.MoveMenuCursor(TwoDimensionalMenu.MenuCursorDirection.Right);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GameContext.NetworkMenuView.DialMenu.SelectOption();
            }
        }

        private static void DraftMenuControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GameContext.DraftContext.MoveCursor(Direction.Up);
                GlobalEventQueue.QueueSingleEvent(new DraftMenuMoveEvent(Direction.Up));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new DraftMenuMoveEvent(Direction.Down));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new DraftMenuMoveEvent(Direction.Left));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new DraftMenuMoveEvent(Direction.Right));
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new DraftConfirmSelectionEvent());
            }
        }

        private static void DeploymentControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Up, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Down, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Left, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Right,
                    GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.LeftBumper, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new DeploySelectPreviousUnitEvent());
            }

            if (controlMapper.Press(Input.RightBumper, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new DeploySelectNextUnitEvent());
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new DeployUnitEvent());
            }

            if (controlMapper.Press(Input.SelectNextUnit, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new DeployResetToNextDeploymentTileEvent());
            }
        }


        private static void MapSelectControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Up, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Down, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Left, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Right,
                    GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new SelectMapEvent());
            }
        }

        private static void MapControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Status, PressType.Single))
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

        private static void CameraControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CenterCamera, PressType.Single))
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

            if (controlMapper.Press(Input.CameraDown, PressType.InstantRepeat))
            {
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Down, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.CameraLeft, PressType.InstantRepeat))
            {
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Left, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.CameraRight, PressType.InstantRepeat))
            {
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Right, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.CameraUp, PressType.InstantRepeat))
            {
                GameContext.MapCamera.MoveCameraInDirection(CameraDirection.Up, cameraPanRateOverride);
            }


            if (controlMapper.Released(Input.CameraDown))
            {
                GameContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.CameraLeft))
            {
                GameContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.CameraRight))
            {
                GameContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.CameraUp))
            {
                GameContext.MapCamera.StopMovingCamera();
            }
        }

        private static void SelectUnitControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Up, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Down, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Left, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Right,
                    GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.SelectNextUnit, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ResetCursorToNextUnitEvent());
            }

            CameraControl(controlMapper);

            if (controlMapper.Press(Input.Menu, PressType.DelayedRepeat))
            {
                AssetManager.MenuConfirmSFX.Play();
                GameContext.CurrentGameState = GameContext.GameState.PauseScreen;
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new SelectUnitEvent());
            }
        }

        private static void MoveUnitControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorAndUnitEvent(Direction.Up,
                    GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorAndUnitEvent(Direction.Down,
                    GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorAndUnitEvent(Direction.Left,
                    GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorAndUnitEvent(Direction.Right,
                    GameContext.CurrentGameState));
            }

            CameraControl(controlMapper);

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new FinishMovingEvent());
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new CancelMoveEvent());
            }
        }

        private static void DecideActionControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveActionMenuEvent(VerticalMenu.MenuCursorDirection.Backward));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveActionMenuEvent(VerticalMenu.MenuCursorDirection.Forward));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ToggleCombatMenuEvent());
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ToggleCombatMenuEvent());
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new SelectActionMenuOptionEvent());
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new CancelActionMenuEvent());
            }

            if (controlMapper.Press(Input.LeftBumper, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new DecrementCurrentAdjustableActionEvent(1));
            }

            if (controlMapper.Press(Input.RightBumper, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new IncrementCurrentAdjustableActionEvent(1));
            }
        }

        private static void PauseMenuControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MovePauseMenuCursor(VerticalMenu.MenuCursorDirection.Backward);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MovePauseMenuCursor(VerticalMenu.MenuCursorDirection.Forward);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GameContext.GameMapContext.SelectPauseMenuOption();
            }

            if (controlMapper.Press(Input.Menu, PressType.Single))
            {
                GameContext.GameMapContext.PauseScreenView.ChangeMenu(PauseScreenView.PauseMenus.Primary);
                GameContext.CurrentGameState = GameContext.GameState.InGame;
            }
        }

        private static void UnitTargetingControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Up, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Down, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Left, GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Right,
                    GameContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.SelectNextUnit, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new ResetCursorToNextUnitEvent());
            }

            CameraControl(controlMapper);

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ExecuteActionEvent());
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new CancelActionEvent());
            }
        }

        private static void UnitActingControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ContinueCombatEvent());
            }
        }

        private static void ResolvingTurnControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ResolveTurnEvent());
            }
        }

        private static void ResultsControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Status, PressType.Single))
            {
                GameContext.CurrentGameState = GameContext.GameState.InGame;
            }

            if (controlMapper.Press(Input.Menu, PressType.Single))
            {
                GameContext.GoToMainMenuIfGameIsOver();
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GameContext.GoToMainMenuIfGameIsOver();
            }
        }
    }
}