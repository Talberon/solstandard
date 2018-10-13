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

namespace SolStandard.Entity.Unit.Skills.Archer
{
    public class Harpoon : UnitAction
    {
        private readonly int skillRange;

        public Harpoon(int skillRange) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Harpoon, new Vector2(GameDriver.CellSize)),
            name: "Harpoon",
            description: "Pull an enemy within range towards you, then attack.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
            this.skillRange = skillRange;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            List<MapDistanceTile> attackTiles = new List<MapDistanceTile>();

            Vector2 northTile = new Vector2(origin.X, origin.Y - skillRange);
            Vector2 southTile = new Vector2(origin.X, origin.Y + skillRange);
            Vector2 eastTile = new Vector2(origin.X + skillRange, origin.Y);
            Vector2 westTile = new Vector2(origin.X - skillRange, origin.Y);

            AddTileWithinMapBounds(attackTiles, northTile, skillRange);
            AddTileWithinMapBounds(attackTiles, southTile, skillRange);
            AddTileWithinMapBounds(attackTiles, eastTile, skillRange);
            AddTileWithinMapBounds(attackTiles, westTile, skillRange);

            AddVisitedTilesToGameGrid(attackTiles, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                if (CanPull(targetSlice, targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new PullEvent(targetUnit));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new StartCombatEvent(targetUnit, gameMapContext, battleContext));

                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    MapContainer.AddNewToastAtMapCursor("Target is obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static bool CanPull(MapSlice targetSlice, GameUnit targetUnit)
        {
            Vector2 actorCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = targetUnit.UnitEntity.MapCoordinates;
            Vector2 pullTileCoordinates = DeterminePullPosition(actorCoordinates, targetCoordinates);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit) &&
                UnitMovingContext.CanMoveAtCoordinates(pullTileCoordinates))
            {
                return true;
            }

            return false;
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
            if (GameMapContext.CoordinatesWithinMapBounds(tileCoordinates))
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