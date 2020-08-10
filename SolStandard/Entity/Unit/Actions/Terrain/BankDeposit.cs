using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Inputs;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class BankDeposit : UnitAction, IIncrementableAction
    {
        private readonly Bank bank;
        public int Value { get; private set; }
        private const string DescriptionTag = "Deposit: ";

        public BankDeposit(Bank bank, int value = 0) : base(
            icon: Currency.GoldIcon(GameDriver.CellSizeVector),
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

            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(bankCoordinates);
        }

        private static WindowContentGrid GenerateActionDescription()
        {
            const int iconSize = GameDriver.CellSize;

            return new WindowContentGrid(new[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont, "Deposit"),
                        ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Taxes, new Vector2(iconSize)),
                        new RenderText(AssetManager.WindowFont, Currency.CurrencyAbbreviation + " in the bank."),
                        RenderBlank.Blank
                    },
                    {
                        new RenderText(AssetManager.WindowFont,
                            "This will count towards the Deposit victory condition."),
                        RenderBlank.Blank, RenderBlank.Blank, RenderBlank.Blank
                    },
                    {
                        new RenderText(AssetManager.WindowFont, "Adjust value to deposit with "),
                        InputIconProvider.GetInputIcon(Input.TabLeft, iconSize),
                        new RenderText(AssetManager.WindowFont, " and "),
                        InputIconProvider.GetInputIcon(Input.TabRight, iconSize)
                    }
                },
                2
            );
        }

        public void Increment(int amountToIncrement)
        {
            int maxGold = GlobalContext.InitiativePhase.GetGoldForTeam(GlobalContext.ActiveTeam);

            if (Value + amountToIncrement > maxGold)
            {
                Value = maxGold;
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
            GameUnit actingUnit = GlobalContext.ActiveUnit;
            var selectedBank = targetSlice.TerrainEntity as Bank;

            if (Value > 0)
            {
                if (SelectedBankIsThisBank(selectedBank))
                {
                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(
                        new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                    );
                    eventQueue.Enqueue(new BankDepositEvent(actingUnit, Value));
                    eventQueue.Enqueue(new WaitFramesEvent(50));
                    eventQueue.Enqueue(new AdditionalActionEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Cannot deposit Gold here!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("No Gold specified!", 50);
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

            GlobalContext.WorldContext.RefreshCurrentActionMenuOption();
        }
    }
}