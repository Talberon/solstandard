using System.Collections.Generic;
using SolStandard.Containers.Contexts.WinConditions;

namespace SolStandard.Containers.Contexts
{
    public class GameScenario
    {
        public Surrender Surrender { get; private set; }

        private List<WinCondition> WinConditions { get; set; }

        public GameScenario(List<WinCondition> winConditions)
        {
            WinConditions = winConditions;
            Surrender = new Surrender();
        }

        public void CheckForWinState(GameContext gameContext)
        {
            if (Surrender.ConditionsMet(gameContext))
            {
                Surrender.EndGame(gameContext);
            }

            foreach (WinCondition scenario in WinConditions)
            {
                if (scenario.ConditionsMet(gameContext))
                {
                    scenario.EndGame(gameContext);
                }
            }
        }
    }
}