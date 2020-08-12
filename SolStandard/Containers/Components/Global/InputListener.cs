using System;
using SolStandard.Containers.Components.InputRemapping;
using SolStandard.Containers.Components.World;
using SolStandard.Containers.Components.World.SubContext.Pause;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;
using SolStandard.Utility.Inputs;

namespace SolStandard.Containers.Components.Global
{
    public static class InputListener
    {
        public static void ListenForInputs(ControlMapper controlMapper)
        {
            switch (GlobalContext.CurrentGameState)
            {
                case GlobalContext.GameState.SplashScreen:
                    return;
                case GlobalContext.GameState.EULAConfirm:
                    EULAControls(controlMapper);
                    break;
                case GlobalContext.GameState.MainMenu:
                    MainMenuControls(controlMapper);
                    break;
                case GlobalContext.GameState.NetworkMenu:
                    NetworkMenuControls(controlMapper);
                    break;
                case GlobalContext.GameState.Deployment:
                    DeploymentControls(controlMapper);
                    break;
                case GlobalContext.GameState.ArmyDraft:
                    DraftMenuControls(controlMapper);
                    break;
                case GlobalContext.GameState.MapSelect:
                    MapSelectControls(controlMapper);
                    break;
                case GlobalContext.GameState.PauseScreen:
                    PauseMenuControl(controlMapper);
                    break;
                case GlobalContext.GameState.InGame:
                    MapControls(controlMapper);
                    break;
                case GlobalContext.GameState.Codex:
                    CodexControls(controlMapper);
                    break;
                case GlobalContext.GameState.Results:
                    ResultsControls(controlMapper);
                    break;
                case GlobalContext.GameState.Credits:
                    CreditsControls(controlMapper);
                    break;
                case GlobalContext.GameState.ItemPreview:
                    ViewInventoryControl(controlMapper);
                    break;
                case GlobalContext.GameState.ControlConfig:
                    InputConfigControl(controlMapper);
                    break;
                case GlobalContext.GameState.HowToPlay:
                    HowToPlayControls(controlMapper);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void EULAControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalContext.EULAContext.ConfirmEULAPrompt();
            }

            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalContext.EULAContext.ScrollWindow(Direction.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalContext.EULAContext.ScrollWindow(Direction.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalContext.EULAContext.ScrollWindow(Direction.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalContext.EULAContext.ScrollWindow(Direction.Right);
            }
        }

        private static void HowToPlayControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalContext.HowToPlayContext.ExitView();
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalContext.HowToPlayContext.OpenBrowser();
            }

            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalContext.HowToPlayContext.ScrollWindow(Direction.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalContext.HowToPlayContext.ScrollWindow(Direction.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalContext.HowToPlayContext.ScrollWindow(Direction.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalContext.HowToPlayContext.ScrollWindow(Direction.Right);
            }
        }

        private static void CreditsControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalContext.CreditsContext.OpenBrowser();
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single) ||
                controlMapper.Press(Input.Status, PressType.Single) ||
                controlMapper.Press(Input.Menu, PressType.Single))
            {
                GlobalContext.CreditsContext.ExitView();
            }

            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalContext.CreditsContext.ScrollWindow(Direction.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalContext.CreditsContext.ScrollWindow(Direction.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalContext.CreditsContext.ScrollWindow(Direction.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalContext.CreditsContext.ScrollWindow(Direction.Right);
            }
        }

        private static void CodexControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalContext.CodexContext.SelectUnit();
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single) ||
                controlMapper.Press(Input.PreviewUnit, PressType.Single) ||
                controlMapper.Press(Input.Status, PressType.Single) ||
                controlMapper.Press(Input.Menu, PressType.Single))
            {
                GlobalContext.CodexContext.CloseMenu();
            }


            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalContext.CodexContext.MoveMenuCursor(MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalContext.CodexContext.MoveMenuCursor(MenuCursorDirection.Right);
            }
        }

        private static void InputConfigControl(ControlMapper controlMapper)
        {
            if (GlobalContext.ControlConfigContext.CurrentState ==
                ControlConfigContext.ControlMenuState.ListeningForInput) return;

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalContext.ControlConfigContext.SelectCurrentOption();
            }

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalContext.ControlConfigContext.Cancel();
            }

            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalContext.ControlConfigContext.MoveMenuCursor(MenuCursorDirection.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalContext.ControlConfigContext.MoveMenuCursor(MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalContext.ControlConfigContext.MoveMenuCursor(MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalContext.ControlConfigContext.MoveMenuCursor(MenuCursorDirection.Right);
            }
        }

        private static void MainMenuControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorDown, PressType.Single))
            {
                GlobalContext.MainMenuHUD.MainMenu.MoveMenuCursor(MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorUp, PressType.Single))
            {
                GlobalContext.MainMenuHUD.MainMenu.MoveMenuCursor(MenuCursorDirection.Up);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalContext.MainMenuHUD.MainMenu.SelectOption();
            }
        }

        private static void NetworkMenuControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalContext.NetworkHUD.Exit();
            }

            if (GlobalContext.NetworkHUD.Menu == null) return;

            if (controlMapper.Press(Input.CursorUp, PressType.Single))
            {
                GlobalContext.NetworkHUD.Menu.MoveMenuCursor(MenuCursorDirection.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.Single))
            {
                GlobalContext.NetworkHUD.Menu.MoveMenuCursor(MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.Single))
            {
                GlobalContext.NetworkHUD.Menu.MoveMenuCursor(MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.Single))
            {
                GlobalContext.NetworkHUD.Menu.MoveMenuCursor(MenuCursorDirection.Right);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalContext.NetworkHUD.Menu.SelectOption();
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
                GlobalContext.CodexContext.OpenMenu();
            }

            CameraControl(controlMapper);
        }

        private static void DeploymentControls(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Up, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Down, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Left, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Right,
                    GlobalContext.CurrentGameState));
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
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Up, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Down, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Left, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Right,
                    GlobalContext.CurrentGameState));
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
                GlobalContext.CurrentGameState = GlobalContext.GameState.Results;
            }

            switch (GlobalContext.WorldContext.CurrentTurnState)
            {
                case WorldContext.TurnState.AdHocDraft:
                    AdHocDraftControl(controlMapper);
                    break;
                case WorldContext.TurnState.TakeItem:
                    StealItemControl(controlMapper);
                    break;
                case WorldContext.TurnState.SelectUnit:
                    SelectUnitControl(controlMapper);
                    break;
                case WorldContext.TurnState.UnitMoving:
                    MoveUnitControl(controlMapper);
                    break;
                case WorldContext.TurnState.UnitDecidingAction:
                    DecideActionControl(controlMapper);
                    break;
                case WorldContext.TurnState.UnitTargeting:
                    UnitTargetingControl(controlMapper);
                    break;
                case WorldContext.TurnState.UnitActing:
                    UnitActingControl(controlMapper);
                    break;
                case WorldContext.TurnState.ResolvingTurn:
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
                GlobalContext.WorldContext.MoveMenuCursor(MenuCursorDirection.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalContext.WorldContext.MoveMenuCursor(MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalContext.WorldContext.MoveMenuCursor(MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalContext.WorldContext.MoveMenuCursor(MenuCursorDirection.Right);
            }

            if (controlMapper.Press(Input.Confirm, PressType.DelayedRepeat))
            {
                GlobalContext.WorldContext.SelectMenuOption();
            }
        }

        private static void StealItemControl(ControlMapper controlMapper)
        {
            AdHocDraftControl(controlMapper);

            if (controlMapper.Press(Input.Cancel, PressType.Single))
            {
                GlobalContext.WorldContext.CancelStealItemMenu();
            }
        }

        private static void CameraControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.ZoomOut, PressType.DelayedRepeat))
            {
                GlobalContext.MapCamera.ZoomOut();
            }

            if (controlMapper.Press(Input.ZoomIn, PressType.DelayedRepeat))
            {
                GlobalContext.MapCamera.ZoomIn();
            }

            const float cameraPanRateOverride = 5;

            if (controlMapper.Press(Input.CameraDown, PressType.InstantRepeat))
            {
                GlobalContext.MapCamera.MoveCameraInDirection(CameraDirection.Down, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.CameraLeft, PressType.InstantRepeat))
            {
                GlobalContext.MapCamera.MoveCameraInDirection(CameraDirection.Left, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.CameraRight, PressType.InstantRepeat))
            {
                GlobalContext.MapCamera.MoveCameraInDirection(CameraDirection.Right, cameraPanRateOverride);
            }

            if (controlMapper.Press(Input.CameraUp, PressType.InstantRepeat))
            {
                GlobalContext.MapCamera.MoveCameraInDirection(CameraDirection.Up, cameraPanRateOverride);
            }


            if (controlMapper.Released(Input.CameraDown))
            {
                GlobalContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.CameraLeft))
            {
                GlobalContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.CameraRight))
            {
                GlobalContext.MapCamera.StopMovingCamera();
            }

            if (controlMapper.Released(Input.CameraUp))
            {
                GlobalContext.MapCamera.StopMovingCamera();
            }
        }

        private static void SelectUnitControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Up, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Down, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Left, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Right,
                    GlobalContext.CurrentGameState));
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
                PauseScreenUtils.OpenScreen(PauseScreenUtils.PauseMenus.Primary);
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
                GlobalContext.WorldContext.ToggleItemPreview();
            }
        }

        private static void MoveUnitControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorAndUnitEvent(Direction.Up,
                    GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorAndUnitEvent(Direction.Down,
                    GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorAndUnitEvent(Direction.Left,
                    GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorAndUnitEvent(Direction.Right,
                    GlobalContext.CurrentGameState));
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
                PauseScreenUtils.CurrentMenu.MoveMenuCursor(MenuCursorDirection.Up);
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                PauseScreenUtils.CurrentMenu.MoveMenuCursor(MenuCursorDirection.Down);
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                PauseScreenUtils.CurrentMenu.MoveMenuCursor(MenuCursorDirection.Left);
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                PauseScreenUtils.CurrentMenu.MoveMenuCursor(MenuCursorDirection.Right);
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                PauseScreenUtils.CurrentMenu.SelectOption();
            }

            if (controlMapper.Press(Input.Menu, PressType.Single))
            {
                PauseScreenUtils.ChangeMenu(PauseScreenUtils.PauseMenus.Primary);
                GlobalContext.CurrentGameState = GlobalContext.GameState.InGame;
            }
        }

        private static void ViewInventoryControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.Confirm, PressType.Single) ||
                controlMapper.Press(Input.PreviewItem, PressType.Single) ||
                controlMapper.Press(Input.Cancel, PressType.Single) ||
                controlMapper.Press(Input.Menu, PressType.Single))
            {
                GlobalContext.WorldContext.ToggleItemPreview();
            }
        }

        private static void UnitTargetingControl(ControlMapper controlMapper)
        {
            if (controlMapper.Press(Input.CursorUp, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Up, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorDown, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Down, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorLeft, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Left, GlobalContext.CurrentGameState));
            }

            if (controlMapper.Press(Input.CursorRight, PressType.DelayedRepeat))
            {
                GlobalEventQueue.QueueSingleEvent(new MoveMapCursorEvent(Direction.Right,
                    GlobalContext.CurrentGameState));
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
                if (GlobalContext.CombatPhase.CombatCanContinue)
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
                if (GlobalContext.Scenario.GameIsOver)
                {
                    GlobalContext.GoToMainMenuIfGameIsOver();
                }
                else
                {
                    AssetManager.MenuConfirmSFX.Play();
                    GlobalContext.CurrentGameState = GlobalContext.GameState.InGame;
                }
            }

            if (controlMapper.Press(Input.Menu, PressType.Single))
            {
                GlobalContext.GoToMainMenuIfGameIsOver();
            }

            if (controlMapper.Press(Input.Confirm, PressType.Single))
            {
                GlobalContext.GoToMainMenuIfGameIsOver();
            }

            CameraControl(controlMapper);
        }
    }
}