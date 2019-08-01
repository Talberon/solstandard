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
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class BasicAttackRoutine : UnitAction, IRoutine
    {
        protected readonly bool Independent;
        private readonly SkillIcon routineIcon;

        public BasicAttackRoutine(bool independent, string name = null, string description = null,
            SkillIcon routineIcon = SkillIcon.BasicAttack)
            : base(
                icon: SkillIconProvider.GetSkillIcon(routineIcon, GameDriver.CellSizeVector),
                name: name ?? "Basic Attack Routine",
                description: description ?? "Attacks a random enemy in range.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
                range: new[] {0},
                freeAction: false
            )
        {
            Independent = independent;
            this.routineIcon = routineIcon;
        }

        public IRenderable MapIcon => SkillIconProvider.GetSkillIcon(routineIcon, new Vector2((float) GameDriver.CellSize / 3));

        public virtual bool CanBeReadied(CreepUnit creepUnit)
        {
            return true;
        }

        public bool CanExecute => TilesWithinThreatRangeForUnit(GameContext.ActiveUnit, Independent).Count > 0;

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit attacker = GameContext.ActiveUnit;

            List<KeyValuePair<GameUnit, Vector2>> targetsInRange = TilesWithinThreatRangeForUnit(attacker, Independent);

            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
            if (targetsInRange.Count > 0)
            {
                PathToTargetAndAttack(targetsInRange, attacker);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates("No valid targets in range!",
                    targetSlice.MapCoordinates, 50);
                AssetManager.WarningSFX.Play();
            }
        }

        protected static void PathToTargetAndAttack(IReadOnlyList<KeyValuePair<GameUnit, Vector2>> targetsInRange,
            GameUnit roamer)
        {
            KeyValuePair<GameUnit, Vector2> targetUnitCoordinatePair =
                targetsInRange[GameDriver.Random.Next(targetsInRange.Count)];
            Vector2 roamerMapCoordinates = roamer.UnitEntity.MapCoordinates;

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(roamerMapCoordinates, "Targeting " + targetUnitCoordinatePair.Key.Id + "!",
                    50)
            );

            List<Direction> directionsToDestination =
                AStarAlgorithm.DirectionsToDestination(roamerMapCoordinates, targetUnitCoordinatePair.Value, false,
                    false);

            Queue<IEvent> pathAndAttackQueue = new Queue<IEvent>();
            foreach (Direction direction in directionsToDestination)
            {
                if (direction == Direction.None) continue;

                pathAndAttackQueue.Enqueue(new CreepMoveEvent(roamer, direction));
                pathAndAttackQueue.Enqueue(new WaitFramesEvent(15));
            }

            pathAndAttackQueue.Enqueue(new CreepMoveEvent(roamer, Direction.None));
            pathAndAttackQueue.Enqueue(new StartCombatEvent(targetUnitCoordinatePair.Key));
            GlobalEventQueue.QueueEvents(pathAndAttackQueue);
        }

        protected static List<KeyValuePair<GameUnit, Vector2>> TilesWithinThreatRangeForUnit(GameUnit creep,
            bool isIndependent)
        {
            MapContainer.ClearDynamicAndPreviewGrids();
        
            //Check movement range
            UnitMovingContext unitMovingContext =
                new UnitMovingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Dark));
            unitMovingContext.GenerateMoveGrid(creep.UnitEntity.MapCoordinates, creep.MvRange, creep.Team);

            //Generate a range ring around each unit on the map using this creep's AtkRange
            //Keep a tally of KV-Pairs tracking the tiles that overlap with the move range and the unit associated with the check

            List<KeyValuePair<GameUnit, Vector2>> attackPositionsInRange = new List<KeyValuePair<GameUnit, Vector2>>();
            UnitTargetingContext unitTargetingContext =
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Dark));

            foreach (GameUnit targetUnit in GameContext.Units)
            {
                if (targetUnit.UnitEntity == null) continue;
                if (targetUnit.UnitEntity == GameContext.ActiveUnit.UnitEntity) continue;
                if (!isIndependent && targetUnit.Team == GameContext.ActiveUnit.Team) continue;

                unitTargetingContext.GenerateTargetingGrid(targetUnit.UnitEntity.MapCoordinates, creep.AtkRange,
                    Layer.Preview);
                foreach (MapElement previewTile in MapContainer.GetMapElementsFromLayer(Layer.Preview))
                {
                    MapSlice currentPreviewSlice = MapContainer.GetMapSliceAtCoordinates(previewTile.MapCoordinates);

                    if (TargetAndMoveTilesOverlap(currentPreviewSlice))
                    {
                        attackPositionsInRange.Add(
                            new KeyValuePair<GameUnit, Vector2>(targetUnit, previewTile.MapCoordinates));
                    }
                }
                
                MapContainer.ClearPreviewGrid();
            }
            

            return attackPositionsInRange;

            //TODO In the future, consider whether the target unit is a ranged/melee unit to make an optimal move
        }

        private static bool TargetAndMoveTilesOverlap(MapSlice currentPreviewSlice)
        {
            return currentPreviewSlice.DynamicEntity != null && currentPreviewSlice.PreviewEntity != null;
        }
    }
}