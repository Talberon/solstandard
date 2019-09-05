using System;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Taxes : Objective
    {
        private readonly int targetGold;

        public Taxes(int targetGold)
        {
            this.targetGold = targetGold;
        }

        protected override IRenderable VictoryLabelContent => BuildObjectiveWindow(AssetManager.ResultsFont);
        public override IRenderable ObjectiveInfo => BuildObjectiveWindow(AssetManager.WindowFont);

        private Window BuildObjectiveWindow(ISpriteFont font)
        {
            Window blueGoldWindow = new Window(
                new RenderText(font, "Blue: " + BankedGoldForTeam(Team.Blue) + "/" + targetGold + "G"),
                TeamUtility.DetermineTeamColor(Team.Blue));

            Window redGoldWindow = new Window(
                new RenderText(font, "Red: " + BankedGoldForTeam(Team.Red) + "/" + targetGold + "G"),
                TeamUtility.DetermineTeamColor(Team.Red));

            WindowContentGrid teamGoldWindowContentGrid = new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        blueGoldWindow,
                        ObjectiveIconProvider.GetObjectiveIcon(
                            VictoryConditions.Taxes,
                            GameDriver.CellSizeVector
                        ),
                        new RenderText(font, "Deposit"),
                        redGoldWindow
                    }
                },
                2,
                HorizontalAlignment.Centered
            );
            return new Window(teamGoldWindowContentGrid, ObjectiveWindowColor);
        }

        public override bool ConditionsMet()
        {
            if (TeamHasBankedTargetGold(Team.Blue) && TeamHasBankedTargetGold(Team.Red))
            {
                GameIsADraw = true;
                return GameIsADraw;
            }

            if (TeamHasBankedTargetGold(Team.Blue))
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            if (TeamHasBankedTargetGold(Team.Red))
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            return false;
        }

        private static int BankedGoldForTeam(Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    return Bank.BlueMoney;
                case Team.Red:
                    return Bank.RedMoney;
                case Team.Creep:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(team), team, null);
            }
        }

        private bool TeamHasBankedTargetGold(Team team)
        {
            return BankedGoldForTeam(team) >= targetGold;
        }
    }
}