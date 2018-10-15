using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ConcedeOption : MenuOption
    {
        private GameMapContext gameMapContext;

        public ConcedeOption(Color color, GameMapContext gameMapContext) :
            base(new RenderText(AssetManager.MainMenuFont, "Surrender"), color)
        {
            this.gameMapContext = gameMapContext;
        }

        public override void Execute()
        {
            switch (GameContext.ActiveUnit.Team)
            {
                case Team.Red:
                    GameContext.GameScenario.Surrender.RedConcedes = true;
                    break;
                case Team.Blue:
                    GameContext.GameScenario.Surrender.BlueConcedes = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Exit the menu and end the turn
            GameContext.CurrentGameState = GameContext.GameState.InGame;
            Queue<IEvent> eventsToQueue = new Queue<IEvent>();
            eventsToQueue.Enqueue(new EndTurnEvent(ref gameMapContext));
            GlobalEventQueue.QueueEvents(eventsToQueue);
        }
    }
}