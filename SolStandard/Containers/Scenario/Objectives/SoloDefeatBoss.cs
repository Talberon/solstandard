using System;
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
    public class SoloDefeatBoss : Objective
    {
        private Window objectiveWindow;
        private readonly Team playerTeam;
        private string resultText;

        public SoloDefeatBoss(Team playerTeam)
        {
            this.playerTeam = playerTeam;
            resultText = "Defeat the Boss!";
        }

        protected override IRenderable VictoryLabelContent => new RenderText(AssetManager.ResultsFont, resultText);

        public override IRenderable ObjectiveInfo => objectiveWindow ??= BuildObjectiveWindow();

        private Window BuildObjectiveWindow()
        {
            return new Window(new WindowContentGrid(new IRenderable[,]
                {
                    {
                        ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.SoloDefeatBoss,
                            GameDriver.CellSizeVector),
                        new RenderText(AssetManager.WindowFont, "Solo Defeat Boss [" + playerTeam + "]")
                    }
                }, 2, HorizontalAlignment.Centered
            ), TeamUtility.DetermineTeamWindowColor(playerTeam), HorizontalAlignment.Centered);
        }

        public override bool ConditionsMet
        {
            get
            {
                if (!AllCreepCommandersAreDead && !AllPlayerUnitsAreDead) return false;

                if (AllPlayerUnitsAreDead)
                {
                    resultText = "DEFEAT!";
                    AllPlayersLose = true;
                    return AllPlayersLose;
                }

                resultText = "ALL BOSSES DEFEATED!";
                switch (playerTeam)
                {
                    case Team.Blue:
                        BlueTeamWins = true;
                        return BlueTeamWins;
                    case Team.Red:
                        RedTeamWins = true;
                        return RedTeamWins;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static bool AllCreepCommandersAreDead
        {
            get
            {
                List<GameUnit> creepCommanders =
                    GlobalContext.Units.FindAll(unit => unit.Team == Team.Creep && unit.IsCommander);

                return !creepCommanders.Any(boss => boss.IsAlive);
            }
        }

        private bool AllPlayerUnitsAreDead
        {
            get { return GlobalContext.Units.FindAll(unit => unit.Team == playerTeam).TrueForAll(unit => !unit.IsAlive); }
        }
    }
}