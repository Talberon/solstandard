using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Entity.Unit;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Buttons;

namespace SolStandard.Containers.Contexts
{
    public static class ControlContext
    {
        private static float _oldZoom;
        
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
                        gameContext.SelectUnitAndStartMoving();
                        return;

                    case MapContext.TurnState.UnitMoving:
                        gameContext.FinishMoving();
                        return;

                    case MapContext.TurnState.UnitDecidingAction:
                        gameContext.StartAction();
                        return;

                    case MapContext.TurnState.UnitTargeting:
                        _oldZoom = mapCamera.CurrentZoom;
                        const float combatZoom = 4;
                        mapCamera.ZoomToCursor(combatZoom);
                        gameContext.StartCombat();
                        return;

                    case MapContext.TurnState.UnitActing:
                        gameContext.ContinueCombat();
                        return;

                    case MapContext.TurnState.ResolvingTurn:
                        mapCamera.SetCameraZoom(_oldZoom);
                        gameContext.ResolveTurn();
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
                        gameContext.CancelMove();
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

                foreach (GameUnit unit in GameContext.Units)
                {
                    unit.DamageUnit(1);
                }

                //gameContext.BattleContext.StartRollingDice();
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