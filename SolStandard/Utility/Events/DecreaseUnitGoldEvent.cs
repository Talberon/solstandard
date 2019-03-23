using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class DecreaseUnitGoldEvent : IEvent
    {
        private readonly int gold;

        public DecreaseUnitGoldEvent(int gold)
        {
            this.gold = gold;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.ActiveUnit.CurrentGold -= gold;
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                GameContext.ActiveUnit.Id + " lost " + gold + Currency.CurrencyAbbreviation + "!", 50
            );
            AssetManager.CoinSFX.Play();
            Complete = true;
        }
    }
}