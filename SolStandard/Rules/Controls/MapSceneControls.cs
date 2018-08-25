using System.Collections.Generic;
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
        public static void ListenForInputs(MapContext mapContext, GameControlMapper controlMapper, MapCamera mapCamera,
            MapCursor mapCursor,
            MapUI mapUi, MapSlice mapSlice, List<GameUnit> units, ITexture2D terrainTextures, ISpriteFont mapFont)
        {
            if (controlMapper.Start())
            {
                mapCamera.SetTargetCameraPosition(new Vector2(0));
            }

            if (controlMapper.Down())
            {
                if (mapContext.CurrentTurnState == MapContext.TurnState.SelectUnit)
                {
                    mapCursor.MoveCursorInDirection((Direction.Down));
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitMoving)
                {
                    //TODO Restrict movement to move grid
                    mapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Down);
                    //TODO Move the unit with the cursor
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitTargetting)
                {
                    mapCursor.MoveCursorInDirection((Direction.Down));
                    return;
                }
            }

            if (controlMapper.Left())
            {
                if (mapContext.CurrentTurnState == MapContext.TurnState.SelectUnit)
                {
                    mapCursor.MoveCursorInDirection((Direction.Left));
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitMoving)
                {
                    //TODO Restrict movement to move grid
                    mapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Left);
                    //TODO Move the unit with the cursor
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitTargetting)
                {
                    mapCursor.MoveCursorInDirection((Direction.Left));
                    return;
                }
            }

            if (controlMapper.Right())
            {
                if (mapContext.CurrentTurnState == MapContext.TurnState.SelectUnit)
                {
                    mapCursor.MoveCursorInDirection((Direction.Right));
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitMoving)
                {
                    //TODO Restrict movement to move grid
                    mapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Right);
                    //TODO Move the unit with the cursor
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitTargetting)
                {
                    mapCursor.MoveCursorInDirection((Direction.Right));
                    return;
                }
            }

            if (controlMapper.Up())
            {
                if (mapContext.CurrentTurnState == MapContext.TurnState.SelectUnit)
                {
                    mapCursor.MoveCursorInDirection((Direction.Up));
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitMoving)
                {
                    //TODO Restrict movement to move grid
                    mapContext.MoveCursorAndSelectedUnitWithinMoveGrid(Direction.Up);
                    //TODO Move the unit with the cursor
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitTargetting)
                {
                    mapCursor.MoveCursorInDirection((Direction.Up));
                    return;
                }
            }

            if (controlMapper.A())
            {
                Trace.WriteLine("Current Turn State: " + mapContext.CurrentTurnState);

                //TODO If the cursor is currently hovering over a VALID unit.
                //TODO A VALID unit is a unit that is first in the initiative list 
                if (mapContext.CurrentTurnState == MapContext.TurnState.SelectUnit)
                {
                    //Select the unit. Store it somewhere.
                    mapContext.SelectedUnit = UnitSelector.SelectUnit(units, mapSlice.UnitEntity);

                    if (mapContext.SelectedUnit != null)
                    {
                        Trace.WriteLine("Selecting unit: " + mapContext.SelectedUnit.UnitTeam + " " +
                                        mapContext.SelectedUnit.UnitJobClass);
                        mapContext.ProceedToNextState();
                        mapContext.GenerateMoveGrid(mapContext.MapLayer.MapCursor.MapCoordinates,
                            mapContext.SelectedUnit.Stats.MaxMv,
                            new TextureCell(new Texture2DWrapper(terrainTextures.MonoGameTexture), GameDriver.CellSize,
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
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitMoving)
                {
                    if (!mapContext.OtherUnitExistsAtCursor())
                    {
                        mapContext.MapLayer.ClearDynamicGrid();
                        mapContext.MoveUnitOnMapGrid();
                        mapContext.ProceedToNextState();
                    }

                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitDecidingAction)
                {
                    mapContext.ProceedToNextState();
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitTargetting)
                {
                    mapContext.ProceedToNextState();
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitActing)
                {
                    mapContext.ProceedToNextState();
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.UnitFinishedActing)
                {
                    mapContext.ProceedToNextState();
                    return;
                }

                if (mapContext.CurrentTurnState == MapContext.TurnState.ResolvingTurn)
                {
                    mapContext.ProceedToNextState();
                    return;
                }
            }

            if (controlMapper.LeftTrigger())
            {
                mapUi.ToggleVisible();
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