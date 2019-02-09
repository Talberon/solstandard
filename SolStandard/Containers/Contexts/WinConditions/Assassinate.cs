using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Assassinate : Objective
    {
        private Window objectiveWindow;

        protected override IRenderable VictoryLabelContent
        {
            get { return new RenderText(AssetManager.ResultsFont, "COMMANDER DEFEATED"); }
        }

        public override IRenderable ObjectiveInfo
        {
            get { return objectiveWindow ?? (objectiveWindow = BuildObjectiveWindow()); }
        }

        private static Window BuildObjectiveWindow()
        {
            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new SpriteAtlas(
                                AssetManager.ObjectiveIcons,
                                new Vector2(16),
                                new Vector2(GameDriver.CellSize),
                                (int) VictoryConditions.Assassinate,
                                Color.White
                            ),
                            new RenderText(AssetManager.WindowFont, "Assassinate"),
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
            List<GameUnit> teamMonarchs = team.FindAll(unit => unit.Role == Role.Bard);

            foreach (GameUnit monarch in teamMonarchs)
            {
                //Return false if any Monarchs are alive.
                if (monarch.Stats.CurrentHP > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}