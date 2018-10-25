using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts
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
                    "Objectives",
                    AssetManager.WindowTexture,
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

        public void CheckForWinState(GameContext gameContext)
        {
            foreach (Objective scenario in Objectives.Values)
            {
                if (scenario.ConditionsMet(gameContext))
                {
                    scenario.EndGame(gameContext);
                }
            }
        }
    }
}