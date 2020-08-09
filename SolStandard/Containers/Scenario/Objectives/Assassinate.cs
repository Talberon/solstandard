using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Scenario.Objectives
{
    public class Assassinate : Objective
    {
        private Window objectiveWindow;

        protected override IRenderable VictoryLabelContent =>
            new RenderText(AssetManager.ResultsFont, "COMMANDER DEFEATED");

        public override IRenderable ObjectiveInfo => objectiveWindow ??= BuildObjectiveWindow();

        private static Window BuildObjectiveWindow()
        {
            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            ObjectiveIconProvider.GetObjectiveIcon(
                                VictoryConditions.Assassinate,
                                GameDriver.CellSizeVector
                            ),
                            new RenderText(AssetManager.WindowFont, "Assassinate")
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
                List<GameUnit> blueTeam = GlobalContext.Units.FindAll(unit => unit.Team == Team.Blue);
                List<GameUnit> redTeam = GlobalContext.Units.FindAll(unit => unit.Team == Team.Red);

                if (TeamCommandersAreAllDead(blueTeam) && TeamCommandersAreAllDead(redTeam))
                {
                    GameIsADraw = true;
                    return GameIsADraw;
                }

                if (TeamCommandersAreAllDead(blueTeam))
                {
                    RedTeamWins = true;
                    return RedTeamWins;
                }

                if (TeamCommandersAreAllDead(redTeam))
                {
                    BlueTeamWins = true;
                    return BlueTeamWins;
                }

                return false;
            }
        }


        private static bool TeamCommandersAreAllDead(List<GameUnit> team)
        {
            List<GameUnit> teamCommanders = team.FindAll(unit => unit.IsCommander);
            return !teamCommanders.Any(commander => commander.IsAlive);
        }
    }
}