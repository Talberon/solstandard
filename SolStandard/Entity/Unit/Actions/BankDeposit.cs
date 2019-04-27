using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class BankDeposit : UnitAction, IIncrementableAction
    {
        private readonly Bank bank;
        public int Value { get; private set; }
        private const string DescriptionTag = "Deposit: ";

        public BankDeposit(Bank bank, int value = 0) : base(
            icon: Currency.GoldIcon(new Vector2(GameDriver.CellSize)),
            name: DescriptionTag + value + Currency.CurrencyAbbreviation,
            description: GenerateActionDescription(),
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: true
        )
        {
            this.bank = bank;
            Value = value;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Vector2 bankCoordinates = bank.MapCoordinates;

            MapContainer.GameGrid[(int) mapLayer][(int) bankCoordinates.X, (int) bankCoordinates.Y] =
                new MapDistanceTile(TileSprite, bankCoordinates);

            GameContext.GameMapContext.MapContainer.MapCursor.SnapCursorToCoordinates(bankCoordinates);
        }

        private static WindowContentGrid GenerateActionDescription()
        {
            Vector2 iconSize = new Vector2(GameDriver.CellSize);

            return new WindowContentGrid(new IRenderable[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont, "Deposit"),
                        ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Taxes, iconSize),
                        new RenderText(AssetManager.WindowFont,
                            Currency.CurrencyAbbreviation +
                            " in the bank." + Environment.NewLine +
                            "Value will still count towards your total for Taxes victory."),
                        new RenderBlank(),
                        new RenderBlank(),
                    },
                    {
                        new RenderText(AssetManager.WindowFont, "Adjust value to deposit with "),
                        ButtonIconProvider.GetButton(ButtonIcon.Lb, iconSize),
                        new RenderText(AssetManager.WindowFont, " and "),
                        ButtonIconProvider.GetButton(ButtonIcon.Rb, iconSize),
                        new RenderText(AssetManager.WindowFont, ""),
                    }
                },
                2
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
            Bank selectedBank = targetSlice.TerrainEntity as Bank;

            if (Value > 0)
            {
                if (SelectedBankIsThisBank(selectedBank))
                {
                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new BankDepositEvent(actingUnit, Value));
                    eventQueue.Enqueue(new WaitFramesEvent(50));
                    eventQueue.Enqueue(new AdditionalActionEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Cannot deposit Gold here!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("No Gold specified!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool SelectedBankIsThisBank(Bank selectedBank)
        {
            return selectedBank != null && selectedBank == bank;
        }


        private void UpdateNameAndDescription()
        {
            Name = DescriptionTag + Value + Currency.CurrencyAbbreviation;
            Description = GenerateActionDescription();

            GameContext.GameMapContext.RefreshCurrentActionMenuOption();
        }
    }
}