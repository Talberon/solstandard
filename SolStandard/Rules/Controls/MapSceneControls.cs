using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
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
        public static void ListenForInputs(MapContext mapContext, BattleContext battleContext,
            GameControlMapper controlMapper, MapCamera mapCamera, MapCursor mapCursor, MapUI mapUi)
        {
            if (controlMapper.Start())
            {
                mapCamera.SetTargetCameraPosition(new Vector2(0));
            }

            if (controlMapper.Down())
            {
                switch (mapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        mapCursor.MoveCursorInDirection((Direction.Down));
                        return;
                    case MapContext.TurnState.UnitMoving:
                        mapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Down);
                        return;
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Down));
                        return;
                }
            }

            if (controlMapper.Left())
            {
                switch (mapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        mapCursor.MoveCursorInDirection((Direction.Left));
                        return;
                    case MapContext.TurnState.UnitMoving:
                        mapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Left);
                        return;
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Left));
                        return;
                }
            }

            if (controlMapper.Right())
            {
                switch (mapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        mapCursor.MoveCursorInDirection((Direction.Right));
                        return;
                    case MapContext.TurnState.UnitMoving:
                        mapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Right);
                        return;
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Right));
                        return;
                }
            }

            if (controlMapper.Up())
            {
                switch (mapContext.CurrentTurnState)
                {
                    case MapContext.TurnState.SelectUnit:
                        mapCursor.MoveCursorInDirection((Direction.Up));
                        return;
                    case MapContext.TurnState.UnitMoving:
                        mapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Up);
                        return;
                    case MapContext.TurnState.UnitTargeting:
                        mapCursor.MoveCursorInDirection((Direction.Up));
                        return;
                }
            }

            if (controlMapper.A())
            {
                Trace.WriteLine("Current Turn State: " + mapContext.CurrentTurnState);

                switch (mapContext.CurrentTurnState)
                {
                    //TODO If the cursor is currently hovering over a VALID unit.
                    //TODO A VALID unit is a unit that is first in the initiative list 
                    case MapContext.TurnState.SelectUnit:
                        //Select the unit. Store it somewhere.

                        mapContext.SelectedUnit =
                            UnitSelector.SelectUnit(mapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);

                        if (mapContext.SelectedUnit != null)
                        {
                            Trace.WriteLine("Selecting unit: " + mapContext.SelectedUnit.UnitTeam + " " +
                                            mapContext.SelectedUnit.UnitJobClass);
                            mapContext.ProceedToNextState();
                            mapContext.GenerateMoveGrid(mapContext.MapContainer.MapCursor.MapCoordinates,
                                mapContext.SelectedUnit.Stats.MaxMv,
                                new TextureCell(new Texture2DWrapper(GameDriver.TerrainTextures.MonoGameTexture),
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
                        if (mapContext.OtherUnitExistsAtCursor()) return;
                        mapContext.MapContainer.ClearDynamicGrid();

                        //TODO Open the menu
                        mapContext.ProceedToNextState();
                        return;

                    case MapContext.TurnState.UnitDecidingAction:
                        //TODO Select option in the menu


                        //If the selection is Basic Attack
                        //Open the targeting grid
                        mapContext.SelectedUnit =
                            UnitSelector.SelectUnit(mapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);
                        mapContext.GenerateTargetingGridAtUnit(new TextureCell(
                            new Texture2DWrapper(GameDriver.TerrainTextures.MonoGameTexture), GameDriver.CellSize, 68));
                        mapContext.ProceedToNextState();
                        return;

                    case MapContext.TurnState.UnitTargeting:
                        //TODO Start Combat
                        GameUnit targetUnit =
                            UnitSelector.SelectUnit(mapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);
                        battleContext.SetupCombatUI(mapContext.SelectedUnit,
                            mapContext.MapContainer.GetMapSliceAtCoordinates(mapContext.SelectedUnit.MapEntity
                                .MapCoordinates),
                            targetUnit, mapContext.MapContainer.GetMapSliceAtCoordinates(targetUnit.MapEntity
                                .MapCoordinates));

                        mapContext.MapContainer.ClearDynamicGrid();
                        mapContext.ProceedToNextState();
                        return;

                    case MapContext.TurnState.UnitActing:
                        //TODO Resolve Combat
                        mapContext.ProceedToNextState();
                        return;

                    case MapContext.TurnState.UnitFinishedActing:
                        //TODO Resolve any additional actions
                        mapContext.ProceedToNextState();
                        return;

                    case MapContext.TurnState.ResolvingTurn:
                        //TODO Do various turn check resolution (win state, etc.)
                        mapContext.ProceedToNextState();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (controlMapper.LeftTrigger())
            {
                mapUi.ToggleVisible();
            }

            if (controlMapper.RightTrigger())
            {
                //FIXME Remove this after debugging use is no longer needed
                foreach (GameUnit unit in GameContext.Units)
                {
                    unit.DamageUnit(1);
                }
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