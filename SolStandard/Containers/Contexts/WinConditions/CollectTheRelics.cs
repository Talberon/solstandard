using System.Linq;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class CollectTheRelics : Objective
    {
        private Window objectiveWindow;
        private readonly int relicsToCollect;

        public CollectTheRelics(int relicsToCollect)
        {
            this.relicsToCollect = relicsToCollect;
        }

        protected override IRenderable VictoryLabelContent => new RenderText(AssetManager.ResultsFont, "COLLECTED TARGET RELICS");


        public override IRenderable ObjectiveInfo => objectiveWindow ?? (objectiveWindow = BuildObjectiveWindow());

        private Window BuildObjectiveWindow()
        {
            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            ObjectiveIconProvider.GetObjectiveIcon(
                                VictoryConditions.CollectTheRelicsVS,
                                GameDriver.CellSizeVector
                            ),
                            new RenderText(AssetManager.WindowFont, "Collect [" + relicsToCollect + "] Relics (VS)")
                        }
                    },
                    2,
                    HorizontalAlignment.Centered
                ),
                ObjectiveWindowColor,
                HorizontalAlignment.Centered
            );
        }

        public override bool ConditionsMet()
        {
            if (TeamHasCollectedTargetNumberOfRelics(Team.Red))
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            if (TeamHasCollectedTargetNumberOfRelics(Team.Blue))
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            if (TeamIsWipedOut(Team.Red) && TeamIsWipedOut(Team.Blue))
            {
                GameIsADraw = true;
                return GameIsADraw;
            }

            return false;
        }

        private bool TeamHasCollectedTargetNumberOfRelics(Team team)
        {
            return GetRelicCountForTeam(team) >= relicsToCollect;
        }

        private static int GetRelicCountForTeam(Team team)
        {
            return GameContext.Units.Where(unit => unit.Team == team)
                .Sum(unit => unit.Inventory.Count(item => item is Relic));
        }

        private static bool TeamIsWipedOut(Team team)
        {
            return GameContext.Units.Where(unit => unit.Team == team).ToList().TrueForAll(unit => !unit.IsAlive);
        }
    }
}