using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NLog;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;

namespace SolStandard.Containers.Scenario
{
    public class Scenario
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Dictionary<VictoryConditions, Objective> Objectives { get; }
        public bool GameIsOver { get; private set; }

        public Scenario(Dictionary<VictoryConditions, Objective> objectives)
        {
            Objectives = objectives;
            GameIsOver = false;
        }

        public Window ScenarioInfo(HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            var objectives = new IRenderable[Objectives.Count, 1];

            for (int i = 0; i < Objectives.Count; i++)
            {
                objectives[i, 0] = Objectives.Values.ToList()[i].ObjectiveInfo;
            }

            return new Window(
                new WindowContentGrid(objectives, 1, alignment),
                Color.Transparent,
                HorizontalAlignment.Centered
            );
        }

        public void CheckForWinState()
        {
            foreach (Objective objective in Objectives.Values)
            {
                if (objective.ConditionsMet)
                {
                    Logger.Trace("Win condition has been met for {}!", objective);
                    GameIsOver = true;
                    objective.EndGame();
                }
            }
        }
    }
}