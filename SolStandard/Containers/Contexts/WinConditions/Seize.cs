using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Seize : Objective
    {
        public bool RedSeizedObjective { get; set; }
        public bool BlueSeizedObjective { get; set; }

        protected override IRenderable VictoryLabelContent
        {
            get { return new RenderText(AssetManager.ResultsFont, "Objective Seized!"); }
        }

        public override bool ConditionsMet()
        {
            if (RedSeizedObjective)
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            if (BlueSeizedObjective)
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            return false;
        }
    }
}