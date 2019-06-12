using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class LeapStrike : UnitAction
    {
        private enum ActionPhase
        {
            SelectTarget,
            SelectLandingSpace
        }

        private ActionPhase currentPhase = ActionPhase.SelectTarget;
        private UnitEntity targetUnitEntity;

        public LeapStrike() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Charge, new Vector2(GameDriver.CellSize)),
            name: "Leap Strike",
            description: "Leap towards an enemy to attack them; even across impassible terrain!",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1, 2, 3},
            freeAction: false
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            //TODO Consider the phase when cancelling the action

            switch (currentPhase)
            {
                case ActionPhase.SelectTarget:
                    if (SelectTarget(targetSlice)) currentPhase = ActionPhase.SelectLandingSpace;
                    break;
                case ActionPhase.SelectLandingSpace:
                    if (SelectLandingSpace(targetSlice)) currentPhase = ActionPhase.SelectTarget;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void CancelAction()
        {
            targetUnitEntity = null;
            currentPhase = ActionPhase.SelectTarget;
            base.CancelAction();
        }

        private bool SelectTarget(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                if (!SpaceAroundUnitIsEntirelyObstructed(targetUnit))
                {
                    //TODO Move this to an event so that it can work over network? Might not be necessary
                    targetUnitEntity = targetUnit.UnitEntity;
                    MapContainer.ClearDynamicAndPreviewGrids();
                    CreateLandingSpacesAroundTarget(targetUnit.UnitEntity.MapCoordinates);
                    AssetManager.MenuConfirmSFX.Play();
                    return true;
                }

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("No space to land!", 50);
                AssetManager.WarningSFX.Play();
                return false;
            }

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private bool SelectLandingSpace(MapSlice targetSlice)
        {
            if (targetSlice.DynamicEntity != null && !CoordinatesAreObstructed(targetSlice.MapCoordinates))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new MoveEntityToCoordinatesEvent(GameContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates));
                eventQueue.Enqueue(new PlaySoundEffectEvent(AssetManager.CombatDamageSFX));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new StartCombatEvent(UnitSelector.SelectUnit(targetUnitEntity)));
                GlobalEventQueue.QueueEvents(eventQueue);
                return true;
            }

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid landing space!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }


        private static bool SpaceAroundUnitIsEntirelyObstructed(GameUnit targetUnit)
        {
            Vector2 unitCoordinates = targetUnit.UnitEntity.MapCoordinates;

            return SouthOfTargetIsObstructed(unitCoordinates) && NorthOfTargetIsObstructed(unitCoordinates) &&
                   WestOfTargetIsObstructed(unitCoordinates) && EastOfTargetIsObstructed(unitCoordinates);
        }

        private static bool EastOfTargetIsObstructed(Vector2 targetCoordinates)
        {
            Vector2 rightOfTarget = new Vector2(
                targetCoordinates.X + 1,
                targetCoordinates.Y
            );

            return CoordinatesAreObstructed(rightOfTarget);
        }

        private static bool WestOfTargetIsObstructed(Vector2 targetCoordinates)
        {
            Vector2 leftOfTarget = new Vector2(
                targetCoordinates.X - 1,
                targetCoordinates.Y
            );

            return CoordinatesAreObstructed(leftOfTarget);
        }

        private static bool NorthOfTargetIsObstructed(Vector2 targetCoordinates)
        {
            Vector2 aboveTarget = new Vector2(
                targetCoordinates.X,
                targetCoordinates.Y - 1
            );

            return CoordinatesAreObstructed(aboveTarget);
        }

        private static bool SouthOfTargetIsObstructed(Vector2 targetCoordinates)
        {
            Vector2 belowTarget = new Vector2(
                targetCoordinates.X,
                targetCoordinates.Y + 1
            );

            return CoordinatesAreObstructed(belowTarget);
        }

        private static bool CoordinatesAreObstructed(Vector2 coordinatesToCheck)
        {
            MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);
            return !UnitMovingContext.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates);
        }

        private void CreateLandingSpacesAroundTarget(Vector2 targetCoordinates)
        {
            List<MapDistanceTile> attackTiles = new List<MapDistanceTile>();

            const int distanceFromTarget = 1;

            Vector2 northTile = new Vector2(targetCoordinates.X, targetCoordinates.Y - distanceFromTarget);
            Vector2 southTile = new Vector2(targetCoordinates.X, targetCoordinates.Y + distanceFromTarget);
            Vector2 eastTile = new Vector2(targetCoordinates.X + distanceFromTarget, targetCoordinates.Y);
            Vector2 westTile = new Vector2(targetCoordinates.X - distanceFromTarget, targetCoordinates.Y);

            if (!NorthOfTargetIsObstructed(targetCoordinates))
                AddTileWithinMapBounds(attackTiles, northTile, distanceFromTarget);
            if (!SouthOfTargetIsObstructed(targetCoordinates))
                AddTileWithinMapBounds(attackTiles, southTile, distanceFromTarget);
            if (!EastOfTargetIsObstructed(targetCoordinates))
                AddTileWithinMapBounds(attackTiles, eastTile, distanceFromTarget);
            if (!WestOfTargetIsObstructed(targetCoordinates))
                AddTileWithinMapBounds(attackTiles, westTile, distanceFromTarget);

            AddAttackTilesToGameGrid(attackTiles, Layer.Dynamic);
        }

        private void AddTileWithinMapBounds(ICollection<MapDistanceTile> tiles, Vector2 tileCoordinates, int distance)
        {
            if (GameMapContext.CoordinatesWithinMapBounds(tileCoordinates))
            {
                tiles.Add(new MapDistanceTile(TileSprite, tileCoordinates, distance));
            }
        }

        private static void AddAttackTilesToGameGrid(IEnumerable<MapDistanceTile> visitedTiles, Layer layer)
        {
            foreach (MapDistanceTile tile in visitedTiles)
            {
                MapContainer.GameGrid[(int) layer][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y] = tile;
            }
        }
    }
}