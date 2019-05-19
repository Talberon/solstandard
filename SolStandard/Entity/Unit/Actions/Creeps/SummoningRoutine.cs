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
using SolStandard.Utility.Events.Network;
using SolStandard.Utility.Model;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class SummoningRoutine : UnitAction, IRoutine
    {
        private readonly Role unitTypeToSpawn;

        //TODO Add unique icon
        private const SkillIcon RoutineIcon = SkillIcon.Focus;

        public SummoningRoutine(Role unitTypeToSpawn)
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2(GameDriver.CellSize)),
                name: "Summoning Routine",
                description: "Summon an ally within range.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {1},
                freeAction: false
            )
        {
            this.unitTypeToSpawn = unitTypeToSpawn;
        }

        public IRenderable MapIcon
        {
            get { return SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3)); }
        }

        public bool CanExecute
        {
            get { return CanPlaceUnitOnAdjacentTile; }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(targetSlice.MapCoordinates, "Summoning " + unitTypeToSpawn + "!", 50)
            );

            if (CanPlaceUnitOnAdjacentTile)
            {
                PlaceUnitOnRandomValidAdjacentTile();
                GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
            }
            else
            {
                EndTurnWithToastMessage("Can't place a unit nearby!");
            }
        }

        private bool CanPlaceUnitOnAdjacentTile
        {
            get
            {
                //This side-effect of altering the map could be icky later; keep this in mind if trying to
                //manipulate the preview layer elsewhere.
                List<MapElement> tilesInRange = GetTilesInRange(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
                MapContainer.ClearDynamicAndPreviewGrids();

                foreach (MapElement element in tilesInRange)
                {
                    if (UnitMovingContext.CanEndMoveAtCoordinates(element.MapCoordinates)) return true;
                }

                return false;
            }
        }

        private void PlaceUnitOnRandomValidAdjacentTile()
        {
            List<MapElement> shuffledTilesInRange = GetTilesInRange(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            shuffledTilesInRange.Shuffle();

            foreach (MapElement element in shuffledTilesInRange)
            {
                if (!UnitMovingContext.CanEndMoveAtCoordinates(element.MapCoordinates)) continue;

                SpawnCreep(MapContainer.GetMapSliceAtCoordinates(element.MapCoordinates), unitTypeToSpawn);
                MapContainer.ClearDynamicAndPreviewGrids();
                return;
            }

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCursorEvent("Failed to place " + unitTypeToSpawn + "!", 50)
            );
        }

        private static void SpawnCreep(MapSlice targetSlice, Role unitType)
        {
            if (TargetIsUnoccupiedTileInRange(targetSlice))
            {
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new SpawnCreepEvent(
                        unitType,
                        targetSlice.MapCoordinates,
                        GenerateCreepProperties(unitType)
                    )
                );
                eventQueue.Enqueue(new WaitFramesEvent(50));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static Dictionary<string, string> GenerateCreepProperties(Role unitType)
        {
            //TODO Possibly consider how creeps can be configured with different routines per summoner
            CreepModel basicCreepModel = new CreepModel(
                unitType,
                false,
                false,
                "",
                Team.Creep,
                Routine.Wander,
                true,
                false,
                Role.Silhouette,
                false
            );

            return basicCreepModel.EntityProperties;
        }

        public static void PlaceUnitInTile(Role role, Vector2 mapCoordinates,
            Dictionary<string, string> entityProperties)
        {
            GameUnit unitToSpawn = UnitGenerator.GenerateAdHocCreep(role, entityProperties);
            unitToSpawn.UnitEntity.MapCoordinates = mapCoordinates;
            GameContext.Units.Add(unitToSpawn);
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Spawned new " + role + "!", 50);
            AssetManager.SkillBuffSFX.Play();
        }

        private static bool TargetIsUnoccupiedTileInRange(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null && targetSlice.UnitEntity == null &&
                   UnitMovingContext.CanEndMoveAtCoordinates(targetSlice.MapCoordinates);
        }

        private List<MapElement> GetTilesInRange(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
            return MapContainer.GetMapElementsFromLayer(Layer.Dynamic);
        }

        private static void EndTurnWithToastMessage(string message)
        {
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(
                    GameContext.ActiveUnit.UnitEntity.MapCoordinates,
                    message,
                    AssetManager.WarningSFX
                )
            );
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }
    }
}