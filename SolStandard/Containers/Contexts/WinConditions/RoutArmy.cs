using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class RoutArmy : Objective
    {
        private Window objectiveWindow;

        protected override IRenderable VictoryLabelContent
        {
            get { return new RenderText(AssetManager.ResultsFont, "ARMY ROUTED"); }
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
                                (int) VictoryConditions.RoutArmy,
                                Color.White
                            ),
                            new RenderText(AssetManager.WindowFont, "Rout Army"),
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

            if (AllUnitsDeadExceptMonarch(blueTeam) && AllUnitsDeadExceptMonarch(redTeam))
            {
                //Let the Commanders fight it out.
                return false;
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
            List<GameUnit> teamMonarchs = team.FindAll(unit => unit.Role != Role.Bard);

            foreach (GameUnit nonmonarch in teamMonarchs)
            {
                //Return false if any non-Monarchs are alive.
                if (nonmonarch.Stats.CurrentHP > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}