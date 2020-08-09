using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Scenario.Objectives
{
    public class RoutArmy : Objective
    {
        private Window objectiveWindow;

        protected override IRenderable VictoryLabelContent => new RenderText(AssetManager.ResultsFont, "ARMY ROUTED");

        public override IRenderable ObjectiveInfo => objectiveWindow ??= BuildObjectiveWindow();

        private static Window BuildObjectiveWindow()
        {
            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            ObjectiveIconProvider.GetObjectiveIcon(
                                VictoryConditions.RoutArmy,
                                GameDriver.CellSizeVector
                            ),
                            new RenderText(AssetManager.WindowFont, "Rout Army")
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
                bool blueTeamRouted = GlobalContext.Units.FindAll(unit => unit.Team == Team.Blue)
                    .TrueForAll(unit => !unit.IsAlive);

                bool redTeamRouted = GlobalContext.Units.FindAll(unit => unit.Team == Team.Red)
                    .TrueForAll(unit => !unit.IsAlive);

                if (blueTeamRouted && redTeamRouted)
                {
                    GameIsADraw = true;
                    return GameIsADraw;
                }

                if (blueTeamRouted)
                {
                    RedTeamWins = true;
                    return RedTeamWins;
                }

                if (redTeamRouted)
                {
                    BlueTeamWins = true;
                    return BlueTeamWins;
                }

                return false;
            }
        }
    }
}