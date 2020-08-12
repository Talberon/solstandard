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

namespace SolStandard.Entity.Unit.Actions.Champion
{
    public class Challenge : UnitAction
    {
        private readonly int skillRange;

        public Challenge(int skillRange) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Challenge, GameDriver.CellSizeVector),
            name: "Challenge",
            description: "Taunt an enemy within range towards you, then attack.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null,
            freeAction: false
        )
        {
            this.skillRange = skillRange;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            var attackTiles = new List<MapDistanceTile>();

            var northTile = new Vector2(origin.X, origin.Y - skillRange);
            var southTile = new Vector2(origin.X, origin.Y + skillRange);
            var eastTile = new Vector2(origin.X + skillRange, origin.Y);
            var westTile = new Vector2(origin.X - skillRange, origin.Y);

            AddTileWithinMapBounds(attackTiles, northTile, skillRange);
            AddTileWithinMapBounds(attackTiles, southTile, skillRange);
            AddTileWithinMapBounds(attackTiles, eastTile, skillRange);
            AddTileWithinMapBounds(attackTiles, westTile, skillRange);

            AddVisitedTilesToGameGrid(attackTiles, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                if (CanPull(targetSlice, targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new PullEvent(targetUnit));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new StartCombatEvent(targetUnit));

                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Obstructed/Immovable!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static bool CanPull(MapSlice targetSlice, GameUnit targetUnit)
        {
            Vector2 actorCoordinates = GlobalContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = targetUnit.UnitEntity.MapCoordinates;
            Vector2 pullTileCoordinates = DeterminePullPosition(actorCoordinates, targetCoordinates);

            return TargetIsUnitInRange(targetSlice, targetUnit) &&
                   UnitMovingPhase.CanEndMoveAtCoordinates(pullTileCoordinates) &&
                   targetUnit.IsMovable;
        }

        public static Vector2 DeterminePullPosition(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            Vector2 closerCoordinates = targetCoordinates;

            if (SourceNorthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move Target North
                closerCoordinates.Y--;
            }

            if (SourceSouthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move Target South
                closerCoordinates.Y++;
            }

            if (SourceEastOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move Target East
                closerCoordinates.X++;
            }

            if (SourceWestOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move Target West
                closerCoordinates.X--;
            }

            return closerCoordinates;
        }

        private void AddTileWithinMapBounds(ICollection<MapDistanceTile> tiles, Vector2 tileCoordinates, int distance)
        {
            if (WorldContext.CoordinatesWithinMapBounds(tileCoordinates))
            {
                tiles.Add(new MapDistanceTile(TileSprite, tileCoordinates, distance));
            }
        }

        private static void AddVisitedTilesToGameGrid(IEnumerable<MapDistanceTile> visitedTiles, Layer layer)
        {
            foreach (MapDistanceTile tile in visitedTiles)
            {
                MapContainer.GameGrid[(int) layer][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y] = tile;
            }
        }
    }
}