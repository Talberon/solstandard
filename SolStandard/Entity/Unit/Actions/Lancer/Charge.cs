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
    public class Charge : UnitAction
    {
        private readonly int chargeDistance;

        public Charge(int chargeDistance) : base(
            //TODO Add Charge Skill Icon
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, new Vector2(GameDriver.CellSize)),
            name: "Charge",
            description: "Dash towards a target and attack!",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
            this.chargeDistance = chargeDistance;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            List<MapDistanceTile> attackTiles = new List<MapDistanceTile>();


            for (int i = chargeDistance; i > 0; i--)
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
                if (!PathIsObstructed(targetSlice, targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue =
                        MoveToTarget(eventQueue, GameContext.ActiveUnit.UnitEntity.MapCoordinates, targetSlice, 10);
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new StartCombatEvent(targetUnit));
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Target is obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static Queue<IEvent> MoveToTarget(Queue<IEvent> eventQueue, Vector2 origin, MapSlice targetSlice,
            int frameDelay)
        {
            if (TargetIsNorth(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    origin.Y - targetSlice.MapCoordinates.Y
                );

                for (int northDistance = 1; northDistance < distanceToTarget; northDistance++)
                {
                    Vector2 coordinatesToMove = new Vector2(origin.X, origin.Y - northDistance);

                    eventQueue.Enqueue(
                        new MoveEntityToCoordinatesEvent(GameContext.ActiveUnit.UnitEntity, coordinatesToMove)
                    );
                    eventQueue.Enqueue(new WaitFramesEvent(frameDelay));
                }
            }

            if (TargetIsSouth(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    targetSlice.MapCoordinates.Y - GameContext.ActiveUnit.UnitEntity.MapCoordinates.Y
                );

                for (int southDistance = 1; southDistance < distanceToTarget; southDistance++)
                {
                    Vector2 coordinatesToMove = new Vector2(origin.X, origin.Y + southDistance);
                    eventQueue.Enqueue(
                        new MoveEntityToCoordinatesEvent(GameContext.ActiveUnit.UnitEntity, coordinatesToMove)
                    );
                    eventQueue.Enqueue(new WaitFramesEvent(frameDelay));
                }
            }

            if (TargetIsEast(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    targetSlice.MapCoordinates.X - GameContext.ActiveUnit.UnitEntity.MapCoordinates.X
                );

                for (int eastDistance = 1; eastDistance < distanceToTarget; eastDistance++)
                {
                    Vector2 coordinatesToMove = new Vector2(origin.X + eastDistance, origin.Y);
                    eventQueue.Enqueue(
                        new MoveEntityToCoordinatesEvent(GameContext.ActiveUnit.UnitEntity, coordinatesToMove)
                    );
                    eventQueue.Enqueue(new WaitFramesEvent(frameDelay));
                }
            }

            if (TargetIsWest(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    GameContext.ActiveUnit.UnitEntity.MapCoordinates.X - targetSlice.MapCoordinates.X
                );

                for (int westDistance = 1; westDistance < distanceToTarget; westDistance++)
                {
                    Vector2 coordinatesToMove = new Vector2(origin.X - westDistance, origin.Y);
                    eventQueue.Enqueue(
                        new MoveEntityToCoordinatesEvent(GameContext.ActiveUnit.UnitEntity, coordinatesToMove)
                    );
                    eventQueue.Enqueue(new WaitFramesEvent(frameDelay));
                }
            }

            return eventQueue;
        }


        private static bool PathIsObstructed(MapSlice targetSlice, GameUnit targetUnit)
        {
            if (TargetIsNorth(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    GameContext.ActiveUnit.UnitEntity.MapCoordinates.Y - targetSlice.MapCoordinates.Y
                );

                for (int northDistance = 1; northDistance < distanceToTarget; northDistance++)
                {
                    Vector2 coordinatesToCheck = new Vector2(
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates.X,
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates.Y - northDistance
                    );
                    MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);

                    if (sliceToCheck.DynamicEntity == null) return true;
                    if (!UnitMovingContext.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates)) return true;
                    if (SliceIsAtTargetUnit(sliceToCheck, targetUnit)) break;
                }
            }

            if (TargetIsSouth(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    targetSlice.MapCoordinates.Y - GameContext.ActiveUnit.UnitEntity.MapCoordinates.Y
                );

                for (int southDistance = 1; southDistance < distanceToTarget; southDistance++)
                {
                    Vector2 coordinatesToCheck = new Vector2(
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates.X,
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates.Y + southDistance
                    );
                    MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);

                    if (sliceToCheck.DynamicEntity == null) return true;
                    if (!UnitMovingContext.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates)) return true;
                    if (SliceIsAtTargetUnit(sliceToCheck, targetUnit)) break;
                }
            }

            if (TargetIsEast(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    targetSlice.MapCoordinates.X - GameContext.ActiveUnit.UnitEntity.MapCoordinates.X
                );

                for (int eastDistance = 1; eastDistance < distanceToTarget; eastDistance++)
                {
                    Vector2 coordinatesToCheck = new Vector2(
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates.X + eastDistance,
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates.Y
                    );
                    MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);

                    if (sliceToCheck.DynamicEntity == null) return true;
                    if (!UnitMovingContext.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates)) return true;
                    if (SliceIsAtTargetUnit(sliceToCheck, targetUnit)) break;
                }
            }

            if (TargetIsWest(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    GameContext.ActiveUnit.UnitEntity.MapCoordinates.X - targetSlice.MapCoordinates.X
                );

                for (int westDistance = 1; westDistance < distanceToTarget; westDistance++)
                {
                    Vector2 coordinatesToCheck = new Vector2(
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates.X - westDistance,
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates.Y
                    );
                    MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);

                    if (sliceToCheck.DynamicEntity == null) return true;
                    if (!UnitMovingContext.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates)) return true;
                    if (SliceIsAtTargetUnit(sliceToCheck, targetUnit)) break;
                }
            }

            return false;
        }

        private static bool SliceIsAtTargetUnit(MapSlice sliceToCheck, GameUnit targetUnit)
        {
            return sliceToCheck.UnitEntity != null && sliceToCheck.UnitEntity == targetUnit.UnitEntity;
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