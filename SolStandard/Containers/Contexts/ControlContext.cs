using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Entity.Unit;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts
{
    public static class ControlContext
    {
        public static void ListenForInputs(GameContext gameContext, GameControlMapper controlMapper,
            MapCamera mapCamera, MapCursor mapCursor)
        {
            if (controlMapper.Start())
            {
                gameContext.MapContext.MapUI.ToggleVisible();
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
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Down));
                        return;
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
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Left));
                        return;
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
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Right));
                        return;
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
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Up));
                        return;
                }
            }

            if (controlMapper.A())
            {
                Trace.WriteLine("Current Turn State: " + gameContext.MapContext.CurrentTurnState);

                switch (gameContext.MapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        //Select the unit. Store it somewhere.
                        gameContext.MapContext.SelectedUnit =
                            UnitSelector.SelectUnit(
                                gameContext.MapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);

                        if (gameContext.MapContext.SelectedUnit != GameContext.ActiveUnit)
                        {
                            gameContext.MapContext.SelectedUnit = null;
                            //TODO Notify the player the selected unit is not legal
                            return;
                        }
                        
                        if (gameContext.MapContext.SelectedUnit != null)
                        {
                            Trace.WriteLine("Selecting unit: " + gameContext.MapContext.SelectedUnit.UnitTeam + " " +
                                            gameContext.MapContext.SelectedUnit.UnitJobClass);
                            gameContext.MapContext.ProceedToNextState();
                            gameContext.MapContext.GenerateMoveGrid(
                                gameContext.MapContext.MapContainer.MapCursor.MapCoordinates,
                                gameContext.MapContext.SelectedUnit.Stats.MaxMv,
                                new SpriteAtlas(new Texture2DWrapper(GameDriver.TerrainTextures.MonoGameTexture),
                                    new Vector2(GameDriver.CellSize), 69));
                        }
                        else
                        {
                            Trace.WriteLine("No unit to select.");
                        }

                        return;

                    case MapContext.TurnState.UnitMoving:
                        if (gameContext.MapContext.OtherUnitExistsAtCursor()) return;
                        gameContext.MapContext.ProceedToNextState();

                        gameContext.MapContext.MapContainer.ClearDynamicGrid();
                        gameContext.MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);

                        //TODO Open the action menu
                        return;

                    case MapContext.TurnState.UnitDecidingAction:
                        gameContext.MapContext.ProceedToNextState();
                        gameContext.MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
                        //TODO Select option in the action menu

                        //If the selection is Basic Attack
                        //Open the targeting grid
                        gameContext.MapContext.SelectedUnit =
                            UnitSelector.SelectUnit(
                                gameContext.MapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);
                        gameContext.MapContext.GenerateTargetingGridAtUnit(new SpriteAtlas(
                            new Texture2DWrapper(GameDriver.TerrainTextures.MonoGameTexture),
                            new Vector2(GameDriver.CellSize), 68));
                        return;

                    case MapContext.TurnState.UnitTargeting:
                        //Start Combat
                        GameUnit attackingUnit = gameContext.MapContext.SelectedUnit;
                        GameUnit defendingUnit = UnitSelector.SelectUnit(
                            gameContext.MapContext.MapContainer.GetMapSliceAtCursor().UnitEntity
                        );

                        if (gameContext.MapContext.TargetUnitIsLegal(defendingUnit))
                        {
                            gameContext.MapContext.MapContainer.ClearDynamicGrid();
                            gameContext.BattleContext.StartNewCombat(attackingUnit,
                                gameContext.MapContext.MapContainer.GetMapSliceAtCoordinates(attackingUnit.UnitEntity
                                    .MapCoordinates),
                                defendingUnit, gameContext.MapContext.MapContainer.GetMapSliceAtCoordinates(
                                    defendingUnit.UnitEntity.MapCoordinates));
                            
                            gameContext.MapContext.SetPromptWindowText("Confirm End Turn");
                            gameContext.MapContext.ProceedToNextState();
                        }
                        else if (attackingUnit == defendingUnit)
                        {
                            //Skip the combat state if player selects the same unit
                            gameContext.MapContext.MapContainer.ClearDynamicGrid();
                            gameContext.MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                            gameContext.MapContext.ProceedToNextState();
                            gameContext.MapContext.SetPromptWindowText("Confirm End Turn");
                            gameContext.MapContext.ProceedToNextState();
                        }

                        return;

                    case MapContext.TurnState.UnitActing:

                        switch (gameContext.BattleContext.CurrentState)
                        {
                            case BattleContext.BattleState.Start:
                                if (gameContext.BattleContext.TryProceedToNextState())
                                    gameContext.BattleContext.StartRollingDice();
                                break;
                            case BattleContext.BattleState.RollDice:
                                if (gameContext.BattleContext.TryProceedToNextState())
                                    gameContext.BattleContext.StartResolvingBlocks();
                                break;
                            case BattleContext.BattleState.CountDice:
                                if (gameContext.BattleContext.TryProceedToNextState())
                                    gameContext.BattleContext.StartResolvingDamage();
                                break;
                            case BattleContext.BattleState.ResolveCombat:
                                if (gameContext.BattleContext.TryProceedToNextState())
                                {
                                    gameContext.MapContext.ProceedToNextState();
                                }

                                break;
                            default:
                                gameContext.MapContext.ProceedToNextState();
                                return;
                        }

                        return;

                    case MapContext.TurnState.ResolvingTurn:
                        //TODO Do various turn check resolution (win state, etc.)
                        gameContext.MapContext.ConfirmPromptWindow();
                        gameContext.InitiativeContext.PassTurnToNextUnit();
                        gameContext.MapContext.ResetCursorToActiveUnit();
                        //TODO Animate unit if currently-active
                        gameContext.MapContext.ProceedToNextState();
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
                        gameContext.MapContext.CancelMovement();
                        return;
                    case MapContext.TurnState.UnitDecidingAction:
                        return;
                    case MapContext.TurnState.UnitTargeting:
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
                gameContext.MapContext.ResetCursorToActiveUnit();
            }

            if (controlMapper.X())
            {
                //FIXME Remove this eventually after debugging is done
                gameContext.BattleContext.StartRollingDice();
                gameContext.MapContext.MapContainer.ClearDynamicGrid();
            }
            

            //TODO Figure out how to handle the free camera or decide if this is only for debugging
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Down);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Left);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Right);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Up);
            }
        }
    }
}