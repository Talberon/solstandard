using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Scenario.Objectives
{
    public class Surrender : Objective
    {
        public bool BlueConcedes { private get; set; }
        public bool RedConcedes { private get; set; }


        protected override IRenderable VictoryLabelContent =>
            new RenderText(AssetManager.ResultsFont, "PLAYER SURRENDERED");

        public override bool ConditionsMet
        {
            get
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
}