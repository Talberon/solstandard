using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
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
            GameMapContext.GameMapView.GenerateObjectiveWindow();

            GameContext.GameMapContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.Interact, new Vector2(GameDriver.CellSize)),
                currency.MapCoordinates
            );
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                $"{GameContext.ActiveUnit.Id} picked up {currency.Value}{Currency.CurrencyAbbreviation}!", 50);
            Complete = true;
        }

        private void RemoveItemFromMap()
        {
            MapContainer.GameGrid[(int) Layer.Items][(int) currency.MapCoordinates.X, (int) currency.MapCoordinates.Y] =
                null;
        }
    }
}