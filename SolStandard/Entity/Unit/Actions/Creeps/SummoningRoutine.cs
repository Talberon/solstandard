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
    public class SummoningRoutine : UnitAction, IRoutine
    {
        private readonly Role unitTypeToSpawn;
        private readonly int refreshTimeInTurns;
        private int turnTicker;

        public SummoningRoutine(Role unitTypeToSpawn, int refreshTimeInTurns = 1)
            : base(
                icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, new Vector2(GameDriver.CellSize)),
                name: "Summoning Routine",
                description: "Summon an ally within range.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {1},
                freeAction: false
            )
        {
            this.unitTypeToSpawn = unitTypeToSpawn;
            this.refreshTimeInTurns = refreshTimeInTurns;
            turnTicker = 0;
        }

        private void StartTickerTimer()
        {
            turnTicker = refreshTimeInTurns;
        }

        //TODO Implement ICooldown interface to handle abilities with a cooldown period
        public void TickRefreshTime()
        {
            turnTicker--;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(targetSlice.MapCoordinates, "Summoning " + unitTypeToSpawn + "!", 50)
            );

            if (SkillIsAvailable)
            {
                if (CanPlaceUnitOnAdjacentTile)
                {
                    //TODO Enable this once cooldowns are implemented
                    //StartTickerTimer();

                    PlaceUnitOnRandomValidAdjacentTile();
                    GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
                }
                else
                {
                    EndTurnWithToastMessage("Can't place a unit nearby!");
                }
            }
            else
            {
                EndTurnWithToastMessage("Skill is on cooldown!");
            }
        }

        private bool SkillIsAvailable
        {
            get { return turnTicker <= 0; }
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

                new SpawnUnitAction(unitTypeToSpawn).ExecuteAction(
                    MapContainer.GetMapSliceAtCoordinates(element.MapCoordinates)
                );
                MapContainer.ClearDynamicAndPreviewGrids();
                return;
            }

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCursorEvent("Failed to place " + unitTypeToSpawn + "!", 50)
            );
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