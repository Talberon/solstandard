using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class TreasureHunterRoutine : UnitAction, IRoutine
    {
        private const SkillIcon RoutineIcon = SkillIcon.TreasureHunter;

        public TreasureHunterRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, GameDriver.CellSizeVector),
                name: "Treasure Hunter Routine",
                description: "Collects items and gold.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {0},
                freeAction: false
            )
        {
        }

        public IRenderable MapIcon =>
            SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3));

        public bool CanBeReadied(CreepUnit creepUnit)
        {
            return UnobstructedTreasureInRange(creepUnit);
        }

        public bool CanExecute
        {
            get
            {
                GameUnit hunter = GlobalContext.Units.Find(creep => creep.Actions.Contains(this));
                return UnobstructedTreasureInRange(hunter);
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            SearchForTreasure();
        }

        private void SearchForTreasure()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            GameUnit activeUnit = GlobalContext.ActiveUnit;

            if (activeUnit.UnitEntity != null && UnobstructedTreasureInRange(activeUnit))
            {
                KeyValuePair<Currency, Vector2>? currencyToPickUp =
                    FindUnobstructedCurrencyInRange(activeUnit.UnitEntity.MapCoordinates, activeUnit.MvRange);

                if (currencyToPickUp != null)
                {
                    PathToCurrencyAndPickUp(currencyToPickUp.Value.Key, currencyToPickUp.Value.Value, activeUnit);
                }
                else
                {
                    KeyValuePair<IItem, Vector2>? itemToPickUp =
                        FindUnobstructedItemInRange(activeUnit.UnitEntity.MapCoordinates, activeUnit.MvRange);

                    if (itemToPickUp != null)
                    {
                        PathToItemAndPickUp(itemToPickUp.Value.Key, itemToPickUp.Value.Value, activeUnit);
                    }
                }

                GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't find any items in range!", 50);
                AssetManager.WarningSFX.Play();
                GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(50));
            }
        }

        private static void PathToItemAndPickUp(IItem itemToPickUp, Vector2 itemCoordinates, GameUnit creep)
        {
            Vector2 roamerMapCoordinates = creep.UnitEntity.MapCoordinates;

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(roamerMapCoordinates, "Picking up " + itemToPickUp.Name + "!", 50)
            );

            Queue<IEvent> pathToItemQueue = PathingUtil.MoveToCoordinates(creep, itemCoordinates, false, false, 15);

            pathToItemQueue.Enqueue(new PickUpItemEvent(itemToPickUp, itemCoordinates));
            pathToItemQueue.Enqueue(new SkippableWaitFramesEvent(50));
            GlobalEventQueue.QueueEvents(pathToItemQueue);
        }

        private static void PathToCurrencyAndPickUp(Currency currencyToPickUp, Vector2 itemCoordinates, GameUnit creep)
        {
            Vector2 roamerMapCoordinates = creep.UnitEntity.MapCoordinates;

            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(roamerMapCoordinates, "Picking up " + currencyToPickUp.Name + "!", 50)
            );

            Queue<IEvent> pathToCurrencyQueue = PathingUtil.MoveToCoordinates(creep, itemCoordinates, false, false, 15);
            pathToCurrencyQueue.Enqueue(new PickUpCurrencyEvent(currencyToPickUp));
            pathToCurrencyQueue.Enqueue(new SkippableWaitFramesEvent(50));
            GlobalEventQueue.QueueEvents(pathToCurrencyQueue);
        }

        private KeyValuePair<IItem, Vector2>? FindUnobstructedItemInRange(Vector2 origin, int mvRange)
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            new UnitMovingPhase(TileSprite).GenerateMoveGrid(origin, mvRange, Team.Creep);
            List<MapElement> movementTiles = MapContainer.GetMapElementsFromLayer(Layer.Dynamic);

            var itemsInRange = new List<KeyValuePair<IItem, Vector2>>();

            foreach (MapElement element in movementTiles)
            {
                MapSlice slice = MapContainer.GetMapSliceAtCoordinates(element.MapCoordinates);

                TerrainEntity targetEntity = slice.ItemEntity;

                if (targetEntity is IActionTile && targetEntity is IItem item)
                {
                    if (UnitMovingPhase.CanEndMoveAtCoordinates(element.MapCoordinates))
                    {
                        itemsInRange.Add(new KeyValuePair<IItem, Vector2>(item, slice.MapCoordinates));
                    }
                }
            }

            MapContainer.ClearDynamicAndPreviewGrids();

            if (itemsInRange.Count <= 0) return null;

            //Return a random item among those found
            itemsInRange.Shuffle();
            return itemsInRange.First();
        }

        private KeyValuePair<Currency, Vector2>? FindUnobstructedCurrencyInRange(Vector2 origin, int mvRange)
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            new UnitMovingPhase(TileSprite).GenerateMoveGrid(origin, mvRange, Team.Creep);
            List<MapElement> movementTiles = MapContainer.GetMapElementsFromLayer(Layer.Dynamic);

            var itemsInRange = new List<KeyValuePair<Currency, Vector2>>();

            foreach (MapElement element in movementTiles)
            {
                MapSlice slice = MapContainer.GetMapSliceAtCoordinates(element.MapCoordinates);

                TerrainEntity targetEntity = slice.ItemEntity;

                if (!(targetEntity is IActionTile) || !(targetEntity is Currency entity)) continue;

                if (UnitMovingPhase.CanEndMoveAtCoordinates(element.MapCoordinates))
                {
                    itemsInRange.Add(new KeyValuePair<Currency, Vector2>(entity, slice.MapCoordinates));
                }
            }

            MapContainer.ClearDynamicAndPreviewGrids();

            if (itemsInRange.Count <= 0) return null;

            //Return a random item among those found
            itemsInRange.Shuffle();
            return itemsInRange.First();
        }

        private bool UnobstructedTreasureInRange(GameUnit unitSearching)
        {
            if (unitSearching.UnitEntity == null) return false;

            KeyValuePair<IItem,Vector2>? itemInRange = FindUnobstructedItemInRange(unitSearching.UnitEntity.MapCoordinates, unitSearching.MvRange);
            KeyValuePair<Currency,Vector2>? moneyInRange = FindUnobstructedCurrencyInRange(unitSearching.UnitEntity.MapCoordinates, unitSearching.MvRange);
            
            return itemInRange != null || moneyInRange != null;
        }
    }
}