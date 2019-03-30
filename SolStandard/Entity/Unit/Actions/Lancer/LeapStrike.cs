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
        private readonly int leapDistance;

        public LeapStrike(int leapDistance) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Charge, new Vector2(GameDriver.CellSize)),
            name: "Leap Strike",
            description: "Leap towards an enemy to attack them; even across impassible terrain!",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null,
            freeAction: false
        )
        {
            this.leapDistance = leapDistance;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            List<MapDistanceTile> attackTiles = new List<MapDistanceTile>();


            for (int i = leapDistance; i > 1; i--)
            {
                Vector2 northTile = new Vector2(origin.X, origin.Y - i);
                Vector2 southTile = new Vector2(origin.X, origin.Y + i);
                Vector2 eastTile = new Vector2(origin.X + i, origin.Y);
                Vector2 westTile = new Vector2(origin.X - i, origin.Y);
                AddTileWithinMapBounds(attackTiles, northTile, i);
                AddTileWithinMapBounds(attackTiles, southTile, i);
                AddTileWithinMapBounds(attackTiles, eastTile, i);
                AddTileWithinMapBounds(attackTiles, westTile, i);
            }

            AddAttackTilesToGameGrid(attackTiles, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                if (!SpaceNextToUnitIsObstructed(targetSlice, targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue = MoveToTarget(eventQueue, targetSlice);
                    eventQueue.Enqueue(new PlaySoundEffectEvent(AssetManager.CombatDamageSFX));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new StartCombatEvent(targetUnit));
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("No space to land!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static Queue<IEvent> MoveToTarget(Queue<IEvent> eventQueue, MapSlice targetSlice)
        {
            Vector2 targetCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;

            if (TargetIsNorth(targetSlice))
            {
                targetCoordinates = new Vector2(
                    targetSlice.MapCoordinates.X,
                    targetSlice.MapCoordinates.Y + 1
                );
            }
            else if (TargetIsSouth(targetSlice))
            {
                targetCoordinates = new Vector2(
                    targetSlice.MapCoordinates.X,
                    targetSlice.MapCoordinates.Y - 1
                );
            }
            else if (TargetIsEast(targetSlice))
            {
                targetCoordinates = new Vector2(
                    targetSlice.MapCoordinates.X - 1,
                    targetSlice.MapCoordinates.Y
                );
            }
            else if (TargetIsWest(targetSlice))
            {
                targetCoordinates = new Vector2(
                    targetSlice.MapCoordinates.X + 1,
                    targetSlice.MapCoordinates.Y
                );
            }

            eventQueue.Enqueue(new MoveEntityToCoordinatesEvent(GameContext.ActiveUnit.UnitEntity, targetCoordinates));

            return eventQueue;
        }


        private static bool SpaceNextToUnitIsObstructed(MapSlice targetSlice, GameUnit targetUnit)
        {
            //Check if the tile in front of the target is movable

            if (TargetIsNorth(targetSlice))
            {
                Vector2 belowTarget = new Vector2(
                    targetUnit.UnitEntity.MapCoordinates.X,
                    targetUnit.UnitEntity.MapCoordinates.Y + 1
                );

                if (CoordinatesAreObstructed(belowTarget)) return true;
            }

            if (TargetIsSouth(targetSlice))
            {
                Vector2 aboveTarget = new Vector2(
                    targetUnit.UnitEntity.MapCoordinates.X,
                    targetUnit.UnitEntity.MapCoordinates.Y - 1
                );

                if (CoordinatesAreObstructed(aboveTarget)) return true;
            }

            if (TargetIsEast(targetSlice))
            {
                Vector2 leftOfTarget = new Vector2(
                    targetUnit.UnitEntity.MapCoordinates.X - 1,
                    targetUnit.UnitEntity.MapCoordinates.Y
                );

                if (CoordinatesAreObstructed(leftOfTarget)) return true;
            }

            if (TargetIsWest(targetSlice))
            {
                Vector2 rightOfTarget = new Vector2(
                    targetUnit.UnitEntity.MapCoordinates.X + 1,
                    targetUnit.UnitEntity.MapCoordinates.Y
                );

                if (CoordinatesAreObstructed(rightOfTarget)) return true;
            }

            return false;
        }

        private static bool CoordinatesAreObstructed(Vector2 coordinatesToCheck)
        {
            MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);
            return !UnitMovingContext.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates);
        }

        private static bool TargetIsNorth(MapSlice targetSlice)
        {
            return targetSlice.MapCoordinates.Y < GameContext.ActiveUnit.UnitEntity.MapCoordinates.Y;
        }

        private static bool TargetIsSouth(MapSlice targetSlice)
        {
            return targetSlice.MapCoordinates.Y > GameContext.ActiveUnit.UnitEntity.MapCoordinates.Y;
        }

        private static bool TargetIsEast(MapSlice targetSlice)
        {
            return targetSlice.MapCoordinates.X > GameContext.ActiveUnit.UnitEntity.MapCoordinates.X;
        }

        private static bool TargetIsWest(MapSlice targetSlice)
        {
            return targetSlice.MapCoordinates.X < GameContext.ActiveUnit.UnitEntity.MapCoordinates.X;
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