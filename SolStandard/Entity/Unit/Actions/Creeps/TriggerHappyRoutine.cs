using System.Collections.Generic;
using System.Linq;
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
    public class TriggerHappyRoutine : UnitAction, IRoutine
    {
        private const SkillIcon RoutineIcon = SkillIcon.TriggerHappy;

        public TriggerHappyRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2(GameDriver.CellSize)),
                name: "Trigger Happy Routine",
                description: "Interact with switches and openable tiles.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {0},
                freeAction: false
            )
        {
        }

        public IRenderable MapIcon => SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3));

        public bool CanBeReadied(CreepUnit unit)
        {
            return FindTriggerableInRange(unit) != null;
        }

        public bool CanExecute
        {
            get
            {
                GameUnit triggerer = GameContext.Units.Find(creep => creep.Actions.Contains(this));
                return FindTriggerableInRange(triggerer) != null;
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit activeUnit = GameContext.ActiveUnit;
            ITriggerable targetTriggerable = FindTriggerableInRange(activeUnit);

            if (targetTriggerable != null)
            {
                PathToTriggerableAndTrigger(targetTriggerable, activeUnit);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                    activeUnit.UnitEntity,
                    "No targets in range to trigger!",
                    50
                );
                AssetManager.WarningSFX.Play();
            }
        }

        private ITriggerable FindTriggerableInRange(GameUnit creep)
        {
            IThreatRange threatRange = new AdHocThreatRange(new[] {1}, creep.MvRange);

            new UnitTargetingContext(TileSprite).GenerateThreatGrid(creep.UnitEntity.MapCoordinates, threatRange);
            List<MapElement> moveTiles = MapContainer.GetMapElementsFromLayer(Layer.Dynamic);
            moveTiles.AddRange(MapContainer.GetMapElementsFromLayer(Layer.Preview));
            MapContainer.ClearDynamicAndPreviewGrids();

            List<ITriggerable> triggerables = new List<ITriggerable>();

            foreach (MapElement moveTile in moveTiles)
            {
                MapSlice moveSlice = MapContainer.GetMapSliceAtCoordinates(moveTile.MapCoordinates);
                if (moveSlice.TerrainEntity is ITriggerable triggerable && triggerable.CanTrigger)
                {
                    triggerables.Add(triggerable);
                }
            }

            triggerables.Shuffle();
            return triggerables.FirstOrDefault();
        }

        private static void PathToTriggerableAndTrigger(ITriggerable triggerable, GameUnit creep)
        {
            Vector2 roamerMapCoordinates = creep.UnitEntity.MapCoordinates;

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(roamerMapCoordinates, "Triggering " + triggerable.Name + "!", 50)
            );

            //If triggerable can't be targeted while standing on top, don't end move on top of it
            bool ignoreLastStep = !triggerable.InteractRange.Contains(0);

            List<Direction> directionsToDestination = AStarAlgorithm.DirectionsToDestination(
                roamerMapCoordinates,
                triggerable.MapCoordinates,
                false,
                ignoreLastStep
            );

            Queue<IEvent> pathToItemQueue = new Queue<IEvent>();
            foreach (Direction direction in directionsToDestination)
            {
                if (direction == Direction.None) continue;

                pathToItemQueue.Enqueue(new UnitMoveEvent(creep, direction));
                pathToItemQueue.Enqueue(new WaitFramesEvent(15));
            }

            pathToItemQueue.Enqueue(new UnitMoveEvent(creep, Direction.None));
            pathToItemQueue.Enqueue(new CreepTriggerTileEvent(triggerable));
            pathToItemQueue.Enqueue(new WaitFramesEvent(50));
            GlobalEventQueue.QueueEvents(pathToItemQueue);
        }
    }
}