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
    public class Escape : Objective
    {
        private Window objectiveWindow;
        private readonly Team escapeTeam;
        private readonly Team hunterTeam;

        public readonly List<GameUnit> EscapedUnits;

        public Escape(Team escapeTeam, Team hunterTeam)
        {
            this.escapeTeam = escapeTeam;
            this.hunterTeam = hunterTeam;
            EscapedUnits = new List<GameUnit>();
        }

        protected override IRenderable VictoryLabelContent
        {
            get
            {
                string victoryText = "UNDECIDED";
                if (RedTeamWins)
                {
                    switch (escapeTeam)
                    {
                        case Team.Blue:
                            victoryText = "ESCAPEES ROUTED";
                            break;
                        case Team.Red:
                            victoryText = "ARMY ESCAPED";
                            break;
                        case Team.Creep:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (BlueTeamWins)
                {
                    switch (escapeTeam)
                    {
                        case Team.Blue:
                            victoryText = "ARMY ESCAPED";
                            break;
                        case Team.Red:
                            victoryText = "ESCAPEES ROUTED";
                            break;
                        case Team.Creep:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return new RenderText(AssetManager.ResultsFont, victoryText);
            }
        }

        public override IRenderable ObjectiveInfo => objectiveWindow ??= BuildObjectiveWindow();

        private Window BuildObjectiveWindow()
        {
            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new Window(new RenderText(AssetManager.WindowFont, "Escape [" + escapeTeam + "]"),
                                TeamUtility.DetermineTeamWindowColor(escapeTeam)),
                            ObjectiveIconProvider.GetObjectiveIcon(
                                VictoryConditions.Escape, GameDriver.CellSizeVector
                            ),
                            new Window(new RenderText(AssetManager.WindowFont, "Rout [" + hunterTeam + "]"),
                                TeamUtility.DetermineTeamWindowColor(hunterTeam))
                        }
                    },
                    2,
                    HorizontalAlignment.Centered
                ),
                ObjectiveWindowColor,
                HorizontalAlignment.Centered
            );
        }

        public void AddUnitToEscapeeList(GameUnit unit)
        {
            EscapedUnits.Add(unit);
        }

        public override bool ConditionsMet
        {
            get
            {
                //Escaping player must have their commander exit the map via an escape point
                //Commander is not allowed to escape until all other units on the team are defeated or have escaped
                List<GameUnit> remainingEscapeTeamUnits =
                    GlobalContext.Units.Where(unit => unit.Team == escapeTeam).ToList();
                EscapedUnits.ForEach(unit => remainingEscapeTeamUnits.Remove(unit));
                bool allNonCommandersEscapedOrDefeated =
                    remainingEscapeTeamUnits.TrueForAll(unit => !unit.IsAlive && !unit.IsCommander);

                if (allNonCommandersEscapedOrDefeated)
                {
                    switch (escapeTeam)
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

                //Hunter player must defeat all escaping commanders before they escape
                if (remainingEscapeTeamUnits.Where(unit => unit.IsCommander).All(unit => !unit.IsAlive))
                {
                    switch (hunterTeam)
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

                return false;
            }
        }
    }
}