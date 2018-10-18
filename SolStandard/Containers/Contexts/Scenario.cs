using System.Collections.Generic;
using SolStandard.Containers.Contexts.WinConditions;

namespace SolStandard.Containers.Contexts
{
    public enum VictoryConditions
    {
        Surrender,
        DefeatCommander,
        Taxes
    }
    
    public class Scenario
    {
        public Dictionary<VictoryConditions, Objective> Objectives { get; private set; }

        public Scenario(Dictionary<VictoryConditions, Objective> objectives)
        {
            Objectives = objectives;
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