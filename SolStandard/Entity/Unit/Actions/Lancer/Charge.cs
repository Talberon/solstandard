using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Containers.Components.World.SubContext.Movement;
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

        protected Charge(IRenderable icon, string skillName, int chargeDistance) : base(
            icon: icon,
            name: skillName,
            description: "Dash towards a target and attack! Cannot move through obstacles or other units." +
                         Environment.NewLine +
                         $"Cannot move further than maximum {UnitStatistics.Abbreviation[Stats.Mv]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {chargeDistance},
            freeAction: false
        )
        {
            this.chargeDistance = chargeDistance;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            var attackTiles = new List<MapDistanceTile>();

            int limitedGallopDistance = Math.Min(chargeDistance, GlobalContext.ActiveUnit.Stats.Mv);

            for (int i = limitedGallopDistance; i > 1; i--)
            {
                (float originX, float originY) = origin;
                var northTile = new Vector2(originX, originY - i);
                var southTile = new Vector2(originX, originY + i);
                var eastTile = new Vector2(originX + i, originY);
                var westTile = new Vector2(originX - i, originY);
                AddTileWithinMapBounds(attackTiles, northTile, i, TileSprite);
                AddTileWithinMapBounds(attackTiles, southTile, i, TileSprite);
                AddTileWithinMapBounds(attackTiles, eastTile, i, TileSprite);
                AddTileWithinMapBounds(attackTiles, westTile, i, TileSprite);
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
                    Queue<IEvent> eventQueue = PathingUtil.MoveToCoordinates(
                        GlobalContext.ActiveUnit,
                        targetUnit.UnitEntity.MapCoordinates,
                        true,
                        false,
                        8
                    );
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new StartCombatEvent(targetUnit));
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Target is obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }


        public static bool PathIsObstructed(MapSlice targetSlice, GameUnit targetUnit)
        {
            if (TargetIsNorth(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.Y - targetSlice.MapCoordinates.Y
                );

                for (int northDistance = 1; northDistance < distanceToTarget; northDistance++)
                {
                    var coordinatesToCheck = new Vector2(
                        GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.X,
                        GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.Y - northDistance
                    );
                    MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);

                    if (!UnitMovingPhase.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates)) return true;
                    if (SliceIsAtTargetUnit(sliceToCheck, targetUnit)) break;
                }
            }

            if (TargetIsSouth(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    targetSlice.MapCoordinates.Y - GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.Y
                );

                for (int southDistance = 1; southDistance < distanceToTarget; southDistance++)
                {
                    var coordinatesToCheck = new Vector2(
                        GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.X,
                        GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.Y + southDistance
                    );
                    MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);

                    if (!UnitMovingPhase.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates)) return true;
                    if (SliceIsAtTargetUnit(sliceToCheck, targetUnit)) break;
                }
            }

            if (TargetIsEast(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    targetSlice.MapCoordinates.X - GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.X
                );

                for (int eastDistance = 1; eastDistance < distanceToTarget; eastDistance++)
                {
                    var coordinatesToCheck = new Vector2(
                        GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.X + eastDistance,
                        GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.Y
                    );
                    MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);

                    if (!UnitMovingPhase.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates)) return true;
                    if (SliceIsAtTargetUnit(sliceToCheck, targetUnit)) break;
                }
            }

            if (TargetIsWest(targetSlice))
            {
                int distanceToTarget = Convert.ToInt32(
                    GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.X - targetSlice.MapCoordinates.X
                );

                for (int westDistance = 1; westDistance < distanceToTarget; westDistance++)
                {
                    var coordinatesToCheck = new Vector2(
                        GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.X - westDistance,
                        GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.Y
                    );
                    MapSlice sliceToCheck = MapContainer.GetMapSliceAtCoordinates(coordinatesToCheck);

                    if (!UnitMovingPhase.CanEndMoveAtCoordinates(sliceToCheck.MapCoordinates)) return true;
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
            return targetSlice.MapCoordinates.Y < GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.Y;
        }

        private static bool TargetIsSouth(MapSlice targetSlice)
        {
            return targetSlice.MapCoordinates.Y > GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.Y;
        }

        private static bool TargetIsEast(MapSlice targetSlice)
        {
            return targetSlice.MapCoordinates.X > GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.X;
        }

        private static bool TargetIsWest(MapSlice targetSlice)
        {
            return targetSlice.MapCoordinates.X < GlobalContext.ActiveUnit.UnitEntity.MapCoordinates.X;
        }


        public static void AddTileWithinMapBounds(ICollection<MapDistanceTile> tiles, Vector2 tileCoordinates,
            int distance, IRenderable tileSprite)
        {
            if (WorldContext.CoordinatesWithinMapBounds(tileCoordinates))
            {
                tiles.Add(new MapDistanceTile(tileSprite, tileCoordinates, distance));
            }
        }

        public static void AddAttackTilesToGameGrid(IEnumerable<MapDistanceTile> visitedTiles, Layer layer)
        {
            foreach (MapDistanceTile tile in visitedTiles)
            {
                MapContainer.GameGrid[(int) layer][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y] = tile;
            }
        }
    }
}