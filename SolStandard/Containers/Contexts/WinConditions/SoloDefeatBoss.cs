using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
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

        protected override IRenderable VictoryLabelContent
        {
            get { return new RenderText(AssetManager.ResultsFont, resultText); }
        }

        public override IRenderable ObjectiveInfo
        {
            get { return objectiveWindow ?? (objectiveWindow = BuildObjectiveWindow()); }
        }

        private static Window BuildObjectiveWindow()
        {
            return new Window(new WindowContentGrid(new IRenderable[,]
                {
                    {
                        ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.SoloDefeatBoss,
                            new Vector2(GameDriver.CellSize)),
                        new RenderText(AssetManager.WindowFont, "Solo Defeat Boss"),
                    }
                }, 2, HorizontalAlignment.Centered
            ), ObjectiveWindowColor, HorizontalAlignment.Centered);
        }

        public override bool ConditionsMet()
        {
            if (!AllCreepCommandersAreDead && !AllPlayerUnitsAreDead) return false;

            if (AllPlayerUnitsAreDead)
            {
                resultText = "DEFEAT!";
                SoloGameLoss = true;
                return SoloGameLoss;
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

        private static bool AllCreepCommandersAreDead
        {
            get
            {
                List<GameUnit> creepCommanders =
                    GameContext.Units.FindAll(unit => unit.Team == Team.Creep && unit.IsCommander);

                return !creepCommanders.Any(boss => boss.IsAlive);
            }
        }

        private bool AllPlayerUnitsAreDead
        {
            get { return GameContext.Units.FindAll(unit => unit.Team == playerTeam).TrueForAll(unit => !unit.IsAlive); }
        }
    }
}