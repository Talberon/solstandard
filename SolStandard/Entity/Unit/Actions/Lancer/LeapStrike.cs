using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Containers.Components.World.SubContext.Targeting;
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
        protected enum ActionPhase
        {
            SelectTarget,
            SelectLandingSpace
        }

        private const MapDistanceTile.TileType TileType = MapDistanceTile.TileType.Attack;
        protected ActionPhase CurrentPhase = ActionPhase.SelectTarget;
        private UnitEntity targetUnitEntity;

        public LeapStrike(IRenderable icon = null, string name = null, string description = null,
            bool freeAction = false) : base(
            icon: icon ?? SkillIconProvider.GetSkillIcon(SkillIcon.LeapStrike, GameDriver.CellSizeVector),
            name: name ?? "Leap Strike",
            description: description ?? "Leap towards an enemy to attack them; even across impassible terrain!" +
                         Environment.NewLine +
                         "Select a target, then select a space to land on next to that target." +
                         Environment.NewLine +
                         $"Cannot move further than maximum {UnitStatistics.Abbreviation[Stats.Mv]}.",
            tileSprite: MapDistanceTile.GetTileSprite(TileType),
            range: new[] {1, 2, 3},
            freeAction: freeAction
        )
        {
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            GenerateLimitedActionRange(origin, mapLayer, Range, TileSprite);
        }

        private static void GenerateLimitedActionRange(Vector2 origin, Layer mapLayer, IEnumerable<int> actionRange,
            IRenderable tileSprite)
        {
            int[] adjustedRange = actionRange.Where(range => range <= GlobalContext.ActiveUnit.Stats.Mv).ToArray();
            var unitTargetingContext = new UnitTargetingPhase(tileSprite);

            if (adjustedRange.Length > 0) unitTargetingContext.GenerateTargetingGrid(origin, adjustedRange, mapLayer);

            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(origin);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            switch (CurrentPhase)
            {
                case ActionPhase.SelectTarget:
                    if (SelectTarget(targetSlice)) CurrentPhase = ActionPhase.SelectLandingSpace;
                    break;
                case ActionPhase.SelectLandingSpace:
                    if (SelectLandingSpace(targetSlice)) CurrentPhase = ActionPhase.SelectTarget;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void CancelAction()
        {
            targetUnitEntity = null;
            CurrentPhase = ActionPhase.SelectTarget;
            base.CancelAction();
        }

        private bool SelectTarget(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                if (!SpaceAroundUnitIsEntirelyObstructed(targetUnit))
                {
                    targetUnitEntity = targetUnit.UnitEntity;
                    MapContainer.ClearDynamicAndPreviewGrids();
                    CreateLandingSpacesAroundTarget(TileType, targetUnit.UnitEntity.MapCoordinates);
                    AssetManager.MenuConfirmSFX.Play();
                    return true;
                }

                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("No space to land!", 50);
                AssetManager.WarningSFX.Play();
                return false;
            }

            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private bool SelectLandingSpace(MapSlice targetSlice)
        {
            if (targetSlice.DynamicEntity != null && !CoordinatesAreObstructed(targetSlice.MapCoordinates))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new MoveEntityToCoordinatesEvent(GlobalContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates));
                eventQueue.Enqueue(new PlaySoundEffectEvent(AssetManager.CombatDamageSFX));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new StartCombatEvent(UnitSelector.SelectUnit(targetUnitEntity), FreeAction));
                GlobalEventQueue.QueueEvents(eventQueue);
                return true;
            }

            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Invalid landing space!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }


        public static bool SpaceAroundUnitIsEntirelyObstructed(GameUnit targetUnit)
        {
            Vector2 unitCoordinates = targetUnit.UnitEntity.MapCoordinates;

            return SouthOfTargetIsObstructed(unitCoordinates) && NorthOfTargetIsObstructed(unitCoordinates) &&
                   WestOfTargetIsObstructed(unitCoordinates) && EastOfTargetIsObstructed(unitCoordinates);
        }

        private static bool EastOfTargetIsObstructed(Vector2 targetCoordinates)
        {
            var rightOfTarget = new Vector2(
                targetCoordinates.X + 1,
                targetCoordinates.Y
            );

            return CoordinatesAreObstructed(rightOfTarget);
        }

        private static bool WestOfTargetIsObstructed(Vector2 targetCoordinates)
        {
            var leftOfTarget = new Vector2(
                targetCoordinates.X - 1,
                targetCoordinates.Y
            );

            return CoordinatesAreObstructed(leftOfTarget);
        }

        private static bool NorthOfTargetIsObstructed(Vector2 targetCoordinates)
        {
            var aboveTarget = new Vector2(
                targetCoordinates.X,
                targetCoordinates.Y - 1
            );

            return CoordinatesAreObstructed(aboveTarget);
        }

        private static bool SouthOfTargetIsObstructed(Vector2 targetCoordinates)
        {
            var belowTarget = new Vector2(
                targetCoordinates.X,
                targetCoordinates.Y + 1
            );

            return CoordinatesAreObstructed(belowTarget);
        }

        public static bool CoordinatesAreObstructed(Vector2 coordinatesToCheck)
        {
            MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);
            return !UnitMovingPhase.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates);
        }

        public static void CreateLandingSpacesAroundTarget(MapDistanceTile.TileType tileType, Vector2 targetCoordinates)
        {
            var attackTiles = new List<MapDistanceTile>();

            const int distanceFromTarget = 1;

            var northTile = new Vector2(targetCoordinates.X, targetCoordinates.Y - distanceFromTarget);
            var southTile = new Vector2(targetCoordinates.X, targetCoordinates.Y + distanceFromTarget);
            var eastTile = new Vector2(targetCoordinates.X + distanceFromTarget, targetCoordinates.Y);
            var westTile = new Vector2(targetCoordinates.X - distanceFromTarget, targetCoordinates.Y);

            if (!NorthOfTargetIsObstructed(targetCoordinates))
                AddTileWithinMapBounds(tileType, attackTiles, northTile, distanceFromTarget);
            if (!SouthOfTargetIsObstructed(targetCoordinates))
                AddTileWithinMapBounds(tileType, attackTiles, southTile, distanceFromTarget);
            if (!EastOfTargetIsObstructed(targetCoordinates))
                AddTileWithinMapBounds(tileType, attackTiles, eastTile, distanceFromTarget);
            if (!WestOfTargetIsObstructed(targetCoordinates))
                AddTileWithinMapBounds(tileType, attackTiles, westTile, distanceFromTarget);

            AddAttackTilesToGameGrid(attackTiles, Layer.Dynamic);
        }

        private static void AddTileWithinMapBounds(MapDistanceTile.TileType tileType,
            ICollection<MapDistanceTile> tiles, Vector2 tileCoordinates, int distance)
        {
            if (WorldContext.CoordinatesWithinMapBounds(tileCoordinates))
            {
                tiles.Add(new MapDistanceTile(MapDistanceTile.GetTileSprite(tileType), tileCoordinates, distance));
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