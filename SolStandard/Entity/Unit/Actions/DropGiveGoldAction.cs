using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.General.Item;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class DropGiveGoldAction : UnitAction, IIncrementableAction
    {
        public int Value { get; private set; }

        public DropGiveGoldAction(int value = 0) : base(
            icon: Currency.GoldIcon(new Vector2(GameDriver.CellSize)),
            name: "Drop/Give: " + value + Currency.CurrencyAbbreviation,
            description: GenerateActionDescription(),
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1},
            freeAction: false
        )
        {
            Value = value;
        }

        private static WindowContentGrid GenerateActionDescription()
        {
            Vector2 iconSize = new Vector2(GameDriver.CellSize);

            return new WindowContentGrid(new [,]
                {
                    {
                        new RenderText(AssetManager.WindowFont,"Drop"),
                        ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Taxes, iconSize),
                        new RenderText(AssetManager.WindowFont,
                            Currency.CurrencyAbbreviation + " on an empty item tile or give it to an ally."),
                        new RenderBlank(),
                        new RenderBlank(),
                    },
                    {
                        new RenderText(AssetManager.WindowFont, "Adjust value to give with "),
                        InputIconProvider.GetInputIcon(Input.LeftBumper, iconSize),
                        new RenderText(AssetManager.WindowFont, " and "),
                        InputIconProvider.GetInputIcon(Input.RightBumper, iconSize),
                        new RenderText(AssetManager.WindowFont, ""),
                    }
                },
                2
            );
        }

        private Spoils GenerateMoneyBag(Vector2 mapCoordinates)
        {
            return new Spoils(
                "Money Bag",
                "Spoils",
                new SpriteAtlas(AssetManager.SpoilsIcon, new Vector2(GameDriver.CellSize)),
                mapCoordinates,
                Value,
                new List<IItem>()
            );
        }

        public void Increment(int amountToIncrement)
        {
            int activeUnitCurrentGold = GameContext.ActiveUnit.CurrentGold;

            if (Value + amountToIncrement > activeUnitCurrentGold)
            {
                Value = activeUnitCurrentGold;
            }
            else
            {
                Value += amountToIncrement;
            }

            UpdateNameAndDescription();
            AssetManager.MenuMoveSFX.Play();
        }

        public void Decrement(int amountToDecrement)
        {
            if (Value - amountToDecrement < 0)
            {
                Value = 0;
            }
            else
            {
                Value -= amountToDecrement;
            }

            UpdateNameAndDescription();
            AssetManager.MenuMoveSFX.Play();
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit actingUnit = GameContext.ActiveUnit;
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (Value > 0)
            {
                if (CanGiveItemToAlly(targetUnit, actingUnit, targetSlice))
                {
                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new TransferUnitGoldEvent(actingUnit, targetUnit, Value, Icon));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new EndTurnEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else if (CanPlaceItemAtSlice(targetSlice))
                {
                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new DecreaseUnitGoldEvent(Value));
                    eventQueue.Enqueue(new PlaceEntityOnMapEvent(
                        GenerateMoneyBag(targetSlice.MapCoordinates), Layer.Items, AssetManager.DropItemSFX)
                    );
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new EndTurnEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Cannot drop/give Gold here!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("No Gold specified!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CanGiveItemToAlly(GameUnit targetUnit, GameUnit actingUnit, MapSlice targetSlice)
        {
            return targetUnit != null && targetUnit.Team == actingUnit.Team && targetUnit != actingUnit &&
                   targetSlice.DynamicEntity != null;
        }

        private static bool CanPlaceItemAtSlice(MapSlice targetSlice)
        {
            return targetSlice.ItemEntity == null && targetSlice.DynamicEntity != null;
        }

        private void UpdateNameAndDescription()
        {
            Name = "Drop/Give: " + Value + Currency.CurrencyAbbreviation;
            Description = GenerateActionDescription();

            GameContext.GameMapContext.RefreshCurrentActionMenuOption();
        }
    }
}