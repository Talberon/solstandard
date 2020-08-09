using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity.General.Item;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class DecreaseTeamGoldEvent : IEvent
    {
        private readonly int gold;

        public DecreaseTeamGoldEvent(int gold)
        {
            this.gold = gold;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.InitiativeContext.DeductGoldFromTeam(gold, GameContext.ActiveTeam);
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                $"{GameContext.ActiveTeam} team lost {gold} {Currency.CurrencyAbbreviation}!", 50
            );
            AssetManager.CoinSFX.Play();
            GameMapContext.GameMapView.GenerateObjectiveWindow();
            Complete = true;
        }
    }
}