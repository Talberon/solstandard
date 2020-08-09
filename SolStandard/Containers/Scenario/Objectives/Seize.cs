using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Scenario.Objectives
{
    public class Seize : Objective
    {
        public bool RedSeizedObjective { get; set; }
        public bool BlueSeizedObjective { get; set; }
        private Window objectiveWindow;

        protected override IRenderable VictoryLabelContent =>
            new RenderText(AssetManager.ResultsFont, "Objective Seized!");

        public override IRenderable ObjectiveInfo => objectiveWindow ??= BuildObjectiveWindow();

        private static Window BuildObjectiveWindow()
        {
            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            ObjectiveIconProvider.GetObjectiveIcon(
                                VictoryConditions.Seize,
                                GameDriver.CellSizeVector
                            ),
                            new RenderText(AssetManager.WindowFont, "Seize Objective")
                        }
                    },
                    2,
                    HorizontalAlignment.Centered
                ),
                ObjectiveWindowColor,
                HorizontalAlignment.Centered
            );
        }

        public override bool ConditionsMet
        {
            get
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
}