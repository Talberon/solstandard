using SolStandard.Containers;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class PickUpCurrencyEvent : IEvent
    {
        private readonly Currency currency;

        public PickUpCurrencyEvent(Currency currency)
        {
            this.currency = currency;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            if (GlobalContext.ActiveTeam == Team.Creep)
            {
                GlobalContext.ActiveUnit.CurrentBounty += currency.Value;
            }
            else
            {
                GlobalContext.InitiativeContext.AddGoldToTeam(currency.Value, GlobalContext.ActiveTeam);
            }

            RemoveItemFromMap();
            AssetManager.CoinSFX.Play();
            GameMapContext.GameMapView.GenerateObjectiveWindow();

            GlobalContext.GameMapContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.FallingCoins, GameDriver.CellSizeVector),
                GlobalContext.ActiveUnit.UnitEntity.MapCoordinates
            );

            GlobalContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                $"{GlobalContext.ActiveUnit.Id} picked up {currency.Value}{Currency.CurrencyAbbreviation}!", 50);
            Complete = true;
        }

        private void RemoveItemFromMap()
        {
            MapContainer.GameGrid[(int) Layer.Items][(int) currency.MapCoordinates.X, (int) currency.MapCoordinates.Y] =
                null;
        }
    }
}