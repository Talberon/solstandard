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

        public Scenario(Dictionary<VictoryConditions, Objective> objectives)
        {
            Objectives = objectives;
        }

        public Window ScenarioInfo
        {
            get
            {
                IRenderable[,] objectives = new IRenderable[Objectives.Count, 1];

                for (int i = 0; i < objectives.Length; i++)
                {
                    objectives[i, 0] = Objectives.Values.ToList()[i].ObjectiveInfo;
                }

                return new Window(
                    new WindowContentGrid(
                        objectives,
                        0,
                        HorizontalAlignment.Centered
                    ),
                    Color.Transparent,
                    HorizontalAlignment.Centered
                );
            }
        }

        public void CheckForWinState()
        {
            foreach (Objective scenario in Objectives.Values)
            {
                if (scenario.ConditionsMet())
                {
                    scenario.EndGame();
                }
            }
        }
    }
}