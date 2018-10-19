using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Surrender : Objective
    {
        public bool BlueConcedes { private get; set; }
        public bool RedConcedes { private get; set; }

        public Surrender() : base(
            new RenderText(AssetManager.ResultsFont, "SURRENDER")
        )
        {
        }

        public override bool ConditionsMet(GameContext gameContext)
        {
            if (BlueConcedes)
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            if (RedConcedes)
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            return false;
        }
    }
}