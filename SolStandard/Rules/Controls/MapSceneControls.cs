using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Logic;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Monogame;

namespace SolStandard.Rules.Controls
{
    public static class MapSceneControls
    {
        public static void ListenForInputs(GameContext gameContext, GameControlMapper controlMapper,
            MapCamera mapCamera, MapCursor mapCursor)
        {
            if (controlMapper.Start())
            {
                mapCamera.SetTargetCameraPosition(new Vector2(0));
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
                    //TODO If the cursor is currently hovering over a VALID unit.
                    //TODO A VALID unit is a unit that is first in the initiative list 
                    case MapContext.TurnState.SelectUnit:
                        //Select the unit. Store it somewhere.

                        gameContext.MapContext.SelectedUnit =
                            UnitSelector.SelectUnit(
                                gameContext.MapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);

                        if (gameContext.MapContext.SelectedUnit != null)
                        {
                            Trace.WriteLine("Selecting unit: " + gameContext.MapContext.SelectedUnit.UnitTeam + " " +
                                            gameContext.MapContext.SelectedUnit.UnitJobClass);
                            gameContext.MapContext.ProceedToNextState();
                            gameContext.MapContext.GenerateMoveGrid(
                                gameContext.MapContext.MapContainer.MapCursor.MapCoordinates,
                                gameContext.MapContext.SelectedUnit.Stats.MaxMv,
                                new SpriteAtlas(new Texture2DWrapper(GameDriver.TerrainTextures.MonoGameTexture),
                                    GameDriver.CellSize,
                                    69));

                            //TODO Remember where the unit originated
                            //TODO Allow the unit to move within the movement grid
                            //TODO Set the current GameState to UNIT_MOVEMENT
                            //TODO On a B-press, remove the movement grid and return the unit to its original position; revert the GameState.
                            //TODO On a second A-press, remove the movement grid and prevent unit from moving again.
                        }
                        else
                        {
                            Trace.WriteLine("No unit to select.");
                        }

                        return;

                    case MapContext.TurnState.UnitMoving:
                        gameContext.MapContext.ProceedToNextState();

                        if (gameContext.MapContext.OtherUnitExistsAtCursor()) return;
                        gameContext.MapContext.MapContainer.ClearDynamicGrid();

                        //TODO Open the menu
                        return;

                    case MapContext.TurnState.UnitDecidingAction:
                        gameContext.MapContext.ProceedToNextState();
                        //TODO Select option in the menu


                        //If the selection is Basic Attack
                        //Open the targeting grid
                        gameContext.MapContext.SelectedUnit =
                            UnitSelector.SelectUnit(
                                gameContext.MapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);
                        gameContext.MapContext.GenerateTargetingGridAtUnit(new SpriteAtlas(
                            new Texture2DWrapper(GameDriver.TerrainTextures.MonoGameTexture), GameDriver.CellSize, 68));
                        return;

                    case MapContext.TurnState.UnitTargeting:
                        //Start Combat
                        GameUnit targetUnit =
                            UnitSelector.SelectUnit(
                                gameContext.MapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);

                        //TODO clean up this gigantic if statement
                        if (targetUnit != null && gameContext.MapContext.SelectedUnit != targetUnit &&
                            BattleContext.CoordinatesAreInRange(
                                gameContext.MapContext.SelectedUnit.MapEntity.MapCoordinates,
                                targetUnit.MapEntity.MapCoordinates,
                                gameContext.MapContext.SelectedUnit.Stats.AtkRange) &&
                            gameContext.MapContext.SelectedUnit.UnitTeam != targetUnit.UnitTeam)
                        {
                            gameContext.MapContext.ProceedToNextState();

                            gameContext.MapContext.MapContainer.ClearDynamicGrid();
                            gameContext.BattleContext.StartNewCombat(gameContext.MapContext.SelectedUnit,
                                gameContext.MapContext.MapContainer.GetMapSliceAtCoordinates(gameContext.MapContext
                                    .SelectedUnit.MapEntity
                                    .MapCoordinates),
                                targetUnit, gameContext.MapContext.MapContainer.GetMapSliceAtCoordinates(targetUnit
                                    .MapEntity
                                    .MapCoordinates));
                        }
                        else if (gameContext.MapContext.SelectedUnit == targetUnit)
                        {
                            //Skip the combat state if player selects the same unit
                            gameContext.MapContext.MapContainer.ClearDynamicGrid();
                            gameContext.MapContext.ProceedToNextState();
                            gameContext.MapContext.ProceedToNextState();
                        }

                        gameContext.MapContext.SetPromptWindowText("Confirm End Turn");
                        return;

                    case MapContext.TurnState.UnitActing:

                        switch (gameContext.BattleContext.CurrentState)
                        {
                            case BattleContext.BattleState.Start:
                                gameContext.BattleContext.ProceedToNextState();
                                gameContext.BattleContext.StartRollingDice();
                                break;
                            case BattleContext.BattleState.RollDice:
                                gameContext.BattleContext.ProceedToNextState();
                                gameContext.BattleContext.StartCountingDice();
                                break;
                            case BattleContext.BattleState.CountDice:
                                gameContext.BattleContext.ProceedToNextState();
                                gameContext.BattleContext.StartResolvingDamage();
                                break;
                            case BattleContext.BattleState.ResolveCombat:
                                gameContext.BattleContext.ProceedToNextState();
                                gameContext.MapContext.ProceedToNextState();
                                break;
                            default:
                                gameContext.MapContext.ProceedToNextState();
                                return;
                        }

                        return;

                    case MapContext.TurnState.ResolvingTurn:
                        //TODO Do various turn check resolution (win state, etc.)
                        //TODO Confirm turn end
                        gameContext.MapContext.ConfirmPromptWindow();
                        //TODO Disable unit
                        gameContext.MapContext.ProceedToNextState();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.LeftTrigger())
            {
                gameContext.MapContext.MapUI.ToggleVisible();
            }

            if (controlMapper.RightTrigger())
            {
                //FIXME Remove this after debugging use is no longer needed
                foreach (GameUnit unit in GameContext.Units)
                {
                    unit.DamageUnit(1);
                }
            }

            if (controlMapper.X())
            {
                //FIXME Remove this eventually after debugging is done
                gameContext.BattleContext.StartRollingDice();
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