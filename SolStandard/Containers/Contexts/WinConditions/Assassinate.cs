using System.Collections.Generic;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Assassinate : Objective
    {
        protected override IRenderable VictoryLabelContent
        {
            get { return new RenderText(AssetManager.ResultsFont, "COMMANDER DEFEATED"); }
        }

        public override bool ConditionsMet(GameContext gameContext)
        {
            List<GameUnit> blueTeam = GameContext.Units.FindAll(unit => unit.Team == Team.Blue);
            List<GameUnit> redTeam = GameContext.Units.FindAll(unit => unit.Team == Team.Red);

            if (TeamMonarchsAreAllDead(blueTeam) && TeamMonarchsAreAllDead(redTeam))
            {
                GameIsADraw = true;
                return GameIsADraw;
            }

            if (TeamMonarchsAreAllDead(blueTeam))
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            if (TeamMonarchsAreAllDead(redTeam))
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            return false;
        }


        private static bool TeamMonarchsAreAllDead(List<GameUnit> team)
        {
            List<GameUnit> teamMonarchs = team.FindAll(unit => unit.Role == Role.Monarch);

            foreach (GameUnit monarch in teamMonarchs)
            {
                //Return false if any Monarchs are alive.
                if (monarch.Stats.Hp > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}