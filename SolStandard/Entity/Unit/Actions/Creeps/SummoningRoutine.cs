using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Containers.Components.World.SubContext.Targeting;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;
using SolStandard.Utility.Model;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class SummoningRoutine : UnitAction, IRoutine
    {
        private readonly CreepRoutineModel creepModel;

        private const SkillIcon RoutineIcon = SkillIcon.Summon;

        public SummoningRoutine(CreepRoutineModel creepModel)
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, GameDriver.CellSizeVector),
                name: "Summoning Routine",
                description: "Summon an ally within range.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {1},
                freeAction: false
            )
        {
            this.creepModel = creepModel;
        }

        public IRenderable MapIcon =>
            SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3));

        public bool CanBeReadied(CreepUnit creepUnit)
        {
            return CanPlaceUnitOnAdjacentTile(creepUnit);
        }

        public bool CanExecute
        {
            get
            {
                GameUnit summoner = GlobalContext.Units.Find(creep => creep.Actions.Contains(this));
                return CanPlaceUnitOnAdjacentTile(summoner);
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(targetSlice.MapCoordinates, "Summoning " + creepModel.CreepClass + "!", 50)
            );

            if (CanPlaceUnitOnAdjacentTile(GlobalContext.ActiveUnit))
            {
                PlaceUnitOnRandomValidAdjacentTile();
                GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
            }
            else
            {
                EndTurnWithToastMessage("Can't place a unit nearby!");
            }
        }

        private bool CanPlaceUnitOnAdjacentTile(GameUnit unitPlacingCreep)
        {
            //This side-effect of altering the map could be icky later; keep this in mind if trying to
            //manipulate the preview layer elsewhere.
            MapContainer.ClearDynamicAndPreviewGrids();
            List<MapElement> tilesInRange = GetTilesInRange(unitPlacingCreep.UnitEntity.MapCoordinates);
            MapContainer.ClearDynamicAndPreviewGrids();

            foreach (MapElement element in tilesInRange)
            {
                if (UnitMovingPhase.CanEndMoveAtCoordinates(element.MapCoordinates)) return true;
            }

            return false;
        }

        private void PlaceUnitOnRandomValidAdjacentTile()
        {
            List<MapElement> shuffledTilesInRange = GetTilesInRange(GlobalContext.ActiveUnit.UnitEntity.MapCoordinates);
            shuffledTilesInRange.Shuffle();

            foreach (MapElement element in shuffledTilesInRange)
            {
                if (!UnitMovingPhase.CanEndMoveAtCoordinates(element.MapCoordinates)) continue;

                SpawnCreep(MapContainer.GetMapSliceAtCoordinates(element.MapCoordinates));
                MapContainer.ClearDynamicAndPreviewGrids();
                return;
            }

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCursorEvent("Failed to place " + creepModel.CreepClass + "!", 50)
            );
        }

        private void SpawnCreep(MapSlice targetSlice)
        {
            if (TargetIsUnoccupiedTileInRange(targetSlice))
            {
                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                );
                eventQueue.Enqueue(
                    new SpawnCreepEvent(
                        creepModel.CreepClass,
                        targetSlice.MapCoordinates,
                        creepModel.EntityProperties
                    )
                );
                eventQueue.Enqueue(new SkippableWaitFramesEvent(50));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static void PlaceCreepInTile(Role role, Vector2 mapCoordinates,
            Dictionary<string, string> entityProperties)
        {
            CreepUnit creepToSpawn = UnitGenerator.GenerateAdHocCreep(role, entityProperties);
            creepToSpawn.UnitEntity.SnapToCoordinates(mapCoordinates);
            creepToSpawn.ExhaustAndDisableUnit();
            creepToSpawn.ReadyNextRoutine();
            GlobalContext.Units.Add(creepToSpawn);
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Spawned new " + role + "!", 50);
            AssetManager.SkillBuffSFX.Play();
        }

        private static bool TargetIsUnoccupiedTileInRange(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null && targetSlice.UnitEntity == null &&
                   UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates);
        }

        private List<MapElement> GetTilesInRange(Vector2 origin)
        {
            var unitTargetingContext = new UnitTargetingPhase(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
            return MapContainer.GetMapElementsFromLayer(Layer.Dynamic);
        }

        private static void EndTurnWithToastMessage(string message)
        {
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(
                    GlobalContext.ActiveUnit.UnitEntity.MapCoordinates,
                    message,
                    AssetManager.WarningSFX
                )
            );
            GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(50));
        }
    }
}