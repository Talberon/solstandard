using System;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
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
                case GameContext.GameState.Codex:
                    CodexControls(controlMapper);
                    break;
                case GameContext.GameState.Results:
                    ResultsControls(controlMapper);
                    break;
                case GameContext.GameState.Credits:
                    CreditsControls(controlMapper);
                    break;
                case GameContext.GameState.ItemPreview:
                    ViewInventoryControl(controlMapper);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void CreditsControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GameContext.CreditsContext.OpenBrowser();
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single) ||
                controlMapper.Press(Input.Status, PressType.Single) ||
                controlMapper.Press(Input.Menu, PressType.Single))
            {
                GameContext.CreditsContext.ExitView();
            }
        }

        private static void CodexControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GameContext.CodexContext.SelectUnit();
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single) ||
                controlMapper.Press(Input.PreviewUnit, PressType.Single) ||
                controlMapper.Press(Input.Status, PressType.Single) ||
                controlMapper.Press(Input.Menu, PressType.Single))
            {
                GameContext.CodexContext.CloseMenu();
            }


            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GameContext.CodexContext.MoveMenuCursor(MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GameContext.CodexContext.MoveMenuCursor(MenuCursorDirection.Right);
            }
        }

        private static void MainMenuControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorDown, PressType.Single))
            {
                GameContext.MainMenuView.MainMenu.MoveMenuCursor(MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorUp, PressType.Single))
            {
                GameContext.MainMenuView.MainMenu.MoveMenuCursor(MenuCursorDirection.Up);
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

            if (GameContext.NetworkMenuView.Menu == null) return;

            if (controlMapper.Press(Input.CursorUp, PressType.Single))
            {
                GameContext.NetworkMenuView.Menu.MoveMenuCursor(MenuCursorDirection.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.Single))
            {
                GameContext.NetworkMenuView.Menu.MoveMenuCursor(MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.Single))
            {
                GameContext.NetworkMenuView.Menu.MoveMenuCursor(MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.Single))
            {
                GameContext.NetworkMenuView.Menu.MoveMenuCursor(MenuCursorDirection.Right);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GameContext.NetworkMenuView.Menu.SelectOption();
            }
        }

        private static void DraftMenuControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
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

            if (controlMapper.Press(Input.PreviewUnit, PressType.Single))
            {
                GameContext.CodexContext.OpenMenu();
            }

            CameraControl(controlMapper);
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

            if (controlMapper.Press(Input.TabLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new DeploySelectPreviousUnitEvent());
            }

            if (controlMapper.Press(Input.TabRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new DeploySelectNextUnitEvent());
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new DeployUnitEvent());
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new DeployResetToNextDeploymentTileEvent());
            }

            if (controlMapper.Press(Input.PreviewUnit, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new PreviewUnitSkillsEvent());
            }

            CameraControl(controlMapper);
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

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ResetGameEvent());
            }

            if (controlMapper.Press(Input.TabLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new PreviousMapEvent());
            }

            if (controlMapper.Press(Input.TabRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new NextMapEvent());
            }

            if (controlMapper.Press(Input.PreviewUnit, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ChangePlayerTeamsEvent(Team.Red));
            }

            if (controlMapper.Press(Input.PreviewItem, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ChangePlayerTeamsEvent(Team.Blue));
            }

            CameraControl(controlMapper);
        }

        private static void MapControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Status, PressType.Single))
            {
                AssetManager.MenuConfirmSFX.Play();
                GameContext.CurrentGameState = GameContext.GameState.Results;
            }

            switch (GameContext.GameMapContext.CurrentTurnState)
            {
                case GameMapContext.TurnState.AdHocDraft:
                    AdHocDraftControl(controlMapper);
                    break;
                case GameMapContext.TurnState.TakeItem:
                    StealItemControl(controlMapper);
                    break;
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

        private static void AdHocDraftControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveMenuCursor(MenuCursorDirection.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveMenuCursor(MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveMenuCursor(MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.MoveMenuCursor(MenuCursorDirection.Right);
            }

            if (controlMapper.Press(Input.Confirm, PressType.DelayedRepeat))
            {
                GameContext.GameMapContext.SelectMenuOption();
            }
        }

        private static void StealItemControl(ControlMapper controlMapper)
        {
            AdHocDraftControl(controlMapper);

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GameContext.GameMapContext.CancelStealItemMenu();
            }
        }

        private static void CameraControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.ZoomOut, PressType.DelayedRepeat))
            {
                GameContext.MapCamera.ZoomOut();
            }

            if (controlMapper.Press(Input.ZoomIn, PressType.DelayedRepeat))
            {
                GameContext.MapCamera.ZoomIn();
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

            if (controlMapper.Press(Input.TabLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new ResetCursorToPreviousUnitEvent());
            }

            if (controlMapper.Press(Input.TabRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new ResetCursorToNextUnitEvent());
            }

            CameraControl(controlMapper);

            if (controlMapper.Press(Input.Menu, PressType.DelayedRepeat))
            {
                AssetManager.MenuConfirmSFX.Play();
                PauseScreenView.OpenScreen(PauseScreenView.PauseMenus.Primary);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new SelectUnitEvent());
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new MapPingEvent());
            }

            if (controlMapper.Press(Input.PreviewUnit, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new PreviewUnitSkillsEvent());
            }

            if (controlMapper.Press(Input.PreviewItem, PressType.Single))
            {
                GameContext.GameMapContext.ToggleItemPreview();
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
                GlobalEventQueue.QueueSingleEvent(new MoveActionMenuEvent(MenuCursorDirection.Up));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveActionMenuEvent(MenuCursorDirection.Down));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveActionMenuEvent(MenuCursorDirection.Left));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveActionMenuEvent(MenuCursorDirection.Right));
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new SelectActionMenuOptionEvent());
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new CancelActionMenuEvent());
            }

            if (controlMapper.Press(Input.TabLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new DecrementCurrentAdjustableActionEvent(1));
            }

            if (controlMapper.Press(Input.TabRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new IncrementCurrentAdjustableActionEvent(1));
            }
        }

        private static void PauseMenuControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                PauseScreenView.CurrentMenu.MoveMenuCursor(MenuCursorDirection.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                PauseScreenView.CurrentMenu.MoveMenuCursor(MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                PauseScreenView.CurrentMenu.MoveMenuCursor(MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                PauseScreenView.CurrentMenu.MoveMenuCursor(MenuCursorDirection.Right);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                PauseScreenView.CurrentMenu.SelectOption();
            }

            if (controlMapper.Press(Input.Menu, PressType.Single))
            {
                PauseScreenView.ChangeMenu(PauseScreenView.PauseMenus.Primary);
                GameContext.CurrentGameState = GameContext.GameState.InGame;
            }
        }

        private static void ViewInventoryControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single) ||
                controlMapper.Press(Input.PreviewItem, PressType.Single) ||
                controlMapper.Press(Input.Cancel, PressType.Single) ||
                controlMapper.Press(Input.Menu, PressType.Single))
            {
                GameContext.GameMapContext.ToggleItemPreview();
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

            if (controlMapper.Press(Input.PreviewItem, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new ResetCursorToActiveUnitEvent());
            }

            CameraControl(controlMapper);

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ExecuteActionEvent());
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new CancelActionTargetingEvent());
            }
        }

        private static void UnitActingControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                if (GameContext.BattleContext.CombatCanContinue)
                {
                    GlobalEventQueue.QueueSingleEvent(new ContinueCombatEvent());
                }
            }
        }

        private static void ResolvingTurnControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalEventQueue.QueueSingleEvent(new ResolveNetworkTurnEvent());
            }
        }

        private static void ResultsControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Status, PressType.Single))
            {
                if (GameContext.Scenario.GameIsOver)
                {
                    GameContext.GoToMainMenuIfGameIsOver();
                }
                else
                {
                    AssetManager.MenuConfirmSFX.Play();
                    GameContext.CurrentGameState = GameContext.GameState.InGame;
                }
            }

            if (controlMapper.Press(Input.Menu, PressType.Single))
            {
                GameContext.GoToMainMenuIfGameIsOver();
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GameContext.GoToMainMenuIfGameIsOver();
            }

            CameraControl(controlMapper);
        }
    }
}