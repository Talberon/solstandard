using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class IncreaseUnitGoldEvent : IEvent
    {
        private readonly int gold;

        public IncreaseUnitGoldEvent(int gold)
        {
            this.gold = gold;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.ActiveUnit.CurrentGold += gold;
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                GameContext.ActiveUnit.Id + " got " + gold + Currency.CurrencyAbbreviation + "!", 50
            );
            AssetManager.CoinSFX.Play();
            GameMapContext.GameMapView.GenerateObjectiveWindow();
            Complete = true;
        }
    }
}