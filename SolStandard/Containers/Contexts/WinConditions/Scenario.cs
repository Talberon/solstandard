using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Scenario
    {
        public Dictionary<VictoryConditions, Objective> Objectives { get; private set; }
        public bool GameIsOver { get; private set; }

        public Scenario(Dictionary<VictoryConditions, Objective> objectives)
        {
            Objectives = objectives;
            GameIsOver = false;
        }

        public Window ScenarioInfo
        {
            get
            {
                IRenderable[,] objectives = new IRenderable[Objectives.Count, 1];

                for (int i = 0; i < Objectives.Count; i++)
                {
                    objectives[i, 0] = Objectives.Values.ToList()[i].ObjectiveInfo;
                }

                return new Window(
                    new WindowContentGrid(objectives, 1),
                    Color.Transparent,
                    HorizontalAlignment.Centered
                );
            }
        }

        public void CheckForWinState()
        {
            foreach (Objective objective in Objectives.Values)
            {
                if (objective.ConditionsMet())
                {
                    GameIsOver = true;
                    objective.EndGame();
                }
            }
        }
    }
}