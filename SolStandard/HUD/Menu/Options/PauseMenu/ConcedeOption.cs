using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Exceptions;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ConcedeOption : MenuOption
    {
        public ConcedeOption(Color color) : base(new RenderText(AssetManager.MainMenuFont, "Surrender"), color)
        {
        }

        public override void Execute()
        {
            Surrender surrender = GameContext.Scenario.Objectives[VictoryConditions.Surrender] as Surrender;

            if (surrender != null)
            {
                switch (GameContext.ActiveUnit.Team)
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
                Queue<IEvent> eventsToQueue = new Queue<IEvent>();
                eventsToQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventsToQueue);
            }
            else
            {
                throw new ScenarioNotFoundException();
            }
        }
    }
}