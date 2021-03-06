using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Containers.Scenario.Objectives;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ConcedeEvent : NetworkEvent
    {
        public override void Continue()
        {
            if (GlobalContext.Scenario.Objectives[VictoryConditions.Surrender] is Surrender surrender)
            {
                switch (GlobalContext.ActiveTeam)
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
                GlobalContext.CurrentGameState = GlobalContext.GameState.InGame;
            }
            else
            {
                throw new ScenarioNotFoundException();
            }
            
            Complete = true;
        }
    }
}