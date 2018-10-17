using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class EndTurnOption : MenuOption
    {
        private GameMapContext gameMapContext;

        public EndTurnOption(Color color, GameMapContext gameMapContext) :
            base(new RenderText(AssetManager.MainMenuFont, "End Turn"), color)
        {
            this.gameMapContext = gameMapContext;
        }

        public override void Execute()
        {
            //Exit the menu and end the turn
            GameContext.CurrentGameState = GameContext.GameState.InGame;
            Queue<IEvent> eventsToQueue = new Queue<IEvent>();
            eventsToQueue.Enqueue(new EndTurnEvent(ref gameMapContext));
            GlobalEventQueue.QueueEvents(eventsToQueue);
        }
    }
}