using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity.General.Item;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class IncreaseTeamGoldEvent : IEvent
    {
        private readonly int gold;

        public IncreaseTeamGoldEvent(int gold)
        {
            this.gold = gold;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GlobalContext.InitiativePhase.AddGoldToTeam(gold, GlobalContext.ActiveTeam);
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                $"{GlobalContext.ActiveTeam} team got {gold} {Currency.CurrencyAbbreviation}!", 50
            );
            AssetManager.CoinSFX.Play();
            WorldContext.WorldHUD.GenerateObjectiveWindow();
            Complete = true;
        }
    }
}