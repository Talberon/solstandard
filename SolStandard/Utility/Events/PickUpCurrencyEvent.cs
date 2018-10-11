using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
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
            GameContext.ActiveUnit.CurrentGold += currency.Value;
            RemoveItemFromMap();
            AssetManager.CoinSFX.Play();
            Complete = true;
        }

        private void RemoveItemFromMap()
        {
            MapContainer.GameGrid[(int) Layer.Items][(int) currency.MapCoordinates.X, (int) currency.MapCoordinates.Y] =
                null;
        }
    }
}