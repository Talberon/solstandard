using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class StationaryAttackRoutine : BasicAttackRoutine
    {
        public StationaryAttackRoutine(bool independent) : base(
            independent,
            "Stationary Attack Routine",
            "Attack a unit within range. Will not move to find target."
        )
        {
        }

        public override bool CanBeReadied(CreepUnit creepUnit)
        {
            return true;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            List<GameUnit> enemiesInRange = GetEnemiesInRange();

            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
            if (enemiesInRange.Count > 0)
            {
                GameUnit target = enemiesInRange[GameDriver.Random.Next(enemiesInRange.Count)];

                GlobalEventQueue.QueueSingleEvent(
                    new ToastAtCursorEvent(
                        $"Targeting {target.Id}!", 50
                    )
                );
                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
                GlobalEventQueue.QueueSingleEvent(new StartCombatEvent(target));
            }
            else
            {
                GlobalEventQueue.QueueSingleEvent(
                    new ToastAtCursorEvent(
                        "No enemies in range! ", 50
                    )
                );
                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
                base.ExecuteAction(targetSlice);
            }
        }

        private static List<GameUnit> GetEnemiesInRange()
        {
            GameUnit actor = GameContext.ActiveUnit;

            MapContainer.ClearDynamicAndPreviewGrids();

            new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Dark))
                .GenerateTargetingGrid(actor.UnitEntity.MapCoordinates,
                    actor.AtkRange);

            List<MapElement> targetingTiles = MapContainer.GetMapElementsFromLayer(Layer.Dynamic);
            List<GameUnit> enemiesInRange = new List<GameUnit>();
            foreach (MapElement tile in targetingTiles)
            {
                MapSlice slice = MapContainer.GetMapSliceAtCoordinates(tile.MapCoordinates);
                GameUnit unit = UnitSelector.SelectUnit(slice.UnitEntity);

                if (unit != null && unit.Team != actor.Team)
                {
                    enemiesInRange.Add(unit);
                }
            }

            MapContainer.ClearDynamicAndPreviewGrids();
            return enemiesInRange;
        }
    }
}