using System.Collections.Generic;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class RoutArmy : Objective
    {

        protected override IRenderable VictoryLabelContent
        {
            get { return new RenderText(AssetManager.ResultsFont, "ARMY ROUTED"); }
        }

        public override bool ConditionsMet(GameContext gameContext)
        {
            List<GameUnit> blueTeam = GameContext.Units.FindAll(unit => unit.Team == Team.Blue);
            List<GameUnit> redTeam = GameContext.Units.FindAll(unit => unit.Team == Team.Red);

            if (AllUnitsDeadExceptMonarch(blueTeam) && AllUnitsDeadExceptMonarch(redTeam))
            {
                GameIsADraw = true;
                return GameIsADraw;
            }

            if (AllUnitsDeadExceptMonarch(blueTeam))
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            if (AllUnitsDeadExceptMonarch(redTeam))
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            return false;
        }


        private static bool AllUnitsDeadExceptMonarch(List<GameUnit> team)
        {
            List<GameUnit> teamMonarchs = team.FindAll(unit => unit.Role != Role.Monarch);

            foreach (GameUnit nonmonarch in teamMonarchs)
            {
                //Return false if any non-Monarchs are alive.
                if (nonmonarch.Stats.Hp > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}