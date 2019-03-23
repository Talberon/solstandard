using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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
        private static int TargetGold { get; set; }

        public Taxes(int targetGold)
        {
            TargetGold = targetGold;
        }

        protected override IRenderable VictoryLabelContent
        {
            get { return BuildObjectiveWindow(AssetManager.ResultsFont); }
        }


        public override IRenderable ObjectiveInfo
        {
            get { return BuildObjectiveWindow(AssetManager.WindowFont); }
        }

        private static Window BuildObjectiveWindow(ISpriteFont font)
        {
            Window blueGoldWindow = new Window(
                new RenderText(font, "Blue: " + CollectedGold(Team.Blue) + "/" + TargetGold + "G"),
                TeamUtility.DetermineTeamColor(Team.Blue));

            Window redGoldWindow = new Window(
                new RenderText(font, "Red: " + CollectedGold(Team.Red) + "/" + TargetGold + "G"),
                TeamUtility.DetermineTeamColor(Team.Red));

            WindowContentGrid teamGoldWindowContentGrid = new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        blueGoldWindow,
                        ObjectiveIconProvider.GetObjectiveIcon(
                            VictoryConditions.Taxes,
                            new Vector2(GameDriver.CellSize)
                        ),
                        redGoldWindow,
                    }
                },
                2,
                HorizontalAlignment.Centered
            );
            return new Window(teamGoldWindowContentGrid, ObjectiveWindowColor);
        }

        public override bool ConditionsMet()
        {
            if (TeamHasCollectedTargetGold(Team.Blue) && TeamHasCollectedTargetGold(Team.Red))
            {
                GameIsADraw = true;
                return GameIsADraw;
            }

            if (TeamHasCollectedTargetGold(Team.Blue))
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            if (TeamHasCollectedTargetGold(Team.Red))
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            return false;
        }

        public static int CollectedGold(Team team)
        {
            List<GameUnit> teamUnitList = GameContext.Units.FindAll(unit => unit.Team == team);

            return teamUnitList.Sum(unit => unit.CurrentGold);
        }

        private bool TeamHasCollectedTargetGold(Team team)
        {
            return CollectedGold(team) >= TargetGold;
        }
    }
}