using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class IncreaseUnitGoldEvent : IEvent
    {
        private int gold;

        public IncreaseUnitGoldEvent(int gold)
        {
            this.gold = gold;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.ActiveUnit.CurrentGold += gold;
            AssetManager.CoinSFX.Play();
            Complete = true;
        }
    }
}