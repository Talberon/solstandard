using System;
using Microsoft.Xna.Framework;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Scenario.Objectives
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
            var blueGoldWindow = new Window(
                new RenderText(font, "Blue: " + BankedGoldForTeam(Team.Blue) + "/" + targetGold + "G"),
                TeamUtility.DetermineTeamWindowColor(Team.Blue));

            var redGoldWindow = new Window(
                new RenderText(font, "Red: " + BankedGoldForTeam(Team.Red) + "/" + targetGold + "G"),
                TeamUtility.DetermineTeamWindowColor(Team.Red));

            var teamGoldWindowContentGrid = new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        blueGoldWindow,
                        ObjectiveIconProvider.GetObjectiveIcon(
                            VictoryConditions.Taxes,
                            new Vector2(font.MeasureString("A").Y)
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

        public override bool ConditionsMet
        {
            get
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
        }

        private static int BankedGoldForTeam(Team team)
        {
            return team switch
            {
                Team.Blue => Bank.BlueMoney,
                Team.Red => Bank.RedMoney,
                Team.Creep => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };
        }

        private bool TeamHasBankedTargetGold(Team team)
        {
            return BankedGoldForTeam(team) >= targetGold;
        }
    }
}