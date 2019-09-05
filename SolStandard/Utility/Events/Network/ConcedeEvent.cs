using System;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ConcedeEvent : NetworkEvent
    {
        public override void Continue()
        {
            if (GameContext.Scenario.Objectives[VictoryConditions.Surrender] is Surrender surrender)
            {
                switch (GameContext.ActiveTeam)
                {
                    case Team.Red:
                        surrender.RedConcedes = true;
                        break;
                    case Team.Blue:
                        surrender.BlueConcedes = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //Exit the menu and end the turn
                GameContext.CurrentGameState = GameContext.GameState.InGame;
            }
            else
            {
                throw new ScenarioNotFoundException();
            }
            
            Complete = true;
        }
    }
}