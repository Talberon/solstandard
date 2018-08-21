using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Logic;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Buttons;

namespace SolStandard.Rules.Controls
{
    public static class MapSceneControls
    {
        public static void ListenForInputs(GameControlMapper controlMapper, MapCamera mapCamera, MapCursor mapCursor,
            MapUI mapUi, MapSlice mapSlice, List<GameUnit> units)
        {
            if (controlMapper.Start())
            {
                mapCamera.SetTargetCameraPosition(new Vector2(0));
            }

            if (controlMapper.Down())
            {
                mapCursor.MoveCursorInDirection((MapCursor.CursorDirection.Down));
            }

            if (controlMapper.Left())
            {
                mapCursor.MoveCursorInDirection((MapCursor.CursorDirection.Left));
            }

            if (controlMapper.Right())
            {
                mapCursor.MoveCursorInDirection((MapCursor.CursorDirection.Right));
            }

            if (controlMapper.Up())
            {
                mapCursor.MoveCursorInDirection((MapCursor.CursorDirection.Up));
            }

            if (controlMapper.A())
            {
                //TODO If the cursor is currently hovering over a VALID unit.
                if (UnitSelector.SelectUnit(units, mapSlice.UnitEntity) != null)
                {
                    //TODO Select the unit. Store it somewhere.
                    //TODO Pin the Left Portrait + Info to the HUD
                    //TODO Generate the movement grid
                    //TODO Remember where the unit originated
                    //TODO Allow the unit to move within the movement grid
                    //TODO Set the current GameState to UNIT_MOVEMENT
                    //TODO On a B-press, remove the movement grid and return the unit to its original position; revert the GameState.
                    //TODO On a second A-press, remove the movement grid and prevent unit from moving again.
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