using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class Sprint : UnitAction
    {
        private readonly int maxDistance;

        public Sprint(int maxDistance) : base(
            icon: UnitStatistics.GetSpriteAtlas(Stats.Mv, GameDriver.CellSizeVector),
            name: $"Sprint [{maxDistance}]",
            description: "Move up to [" + maxDistance + "] additional spaces this turn." + Environment.NewLine +
                         "Can not move further than maximum " + UnitStatistics.Abbreviation[Stats.Mv] + ".",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: false
        )
        {
            this.maxDistance = maxDistance;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            GenerateSprintGrid(origin, GlobalContext.ActiveUnit, maxDistance, mapLayer);
        }

        public static void GenerateSprintGrid(Vector2 origin, GameUnit sprintingUnit, int maxDistance,
            Layer mapLayer = Layer.Dynamic)
        {
            int lowerMv = sprintingUnit.Stats.Mv < maxDistance ? sprintingUnit.Stats.Mv : maxDistance;

            var unitMovingContext =
                new UnitMovingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement));
            unitMovingContext.GenerateMoveGrid(origin, lowerMv, sprintingUnit.Team);

            //Delete the origin space to prevent players standing still and wasting action.
            MapContainer.GameGrid[(int) mapLayer][(int) origin.X, (int) origin.Y] = null;

            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(origin);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanMove(GlobalContext.ActiveUnit))
            {
                if (CanMoveToTargetTile(targetSlice))
                {
                    MoveUnitToTargetPosition(GlobalContext.ActiveUnit, targetSlice.MapCoordinates);
                    GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a valid tile!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't move!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static void MoveUnitToTargetPosition(GameUnit movingUnit, Vector2 mapCoordinates)
        {
            const bool walkThroughAllies = true;

            Queue<IEvent> pathingEventQueue =
                PathingUtil.MoveToCoordinates(movingUnit, mapCoordinates, false, walkThroughAllies, 10);

            pathingEventQueue.Enqueue(new CameraCursorPositionEvent(mapCoordinates));
            GlobalEventQueue.QueueEvents(pathingEventQueue);
        }

        public static bool CanMove(GameUnit unit)
        {
            return unit.Stats.Mv > 0;
        }
    }
}