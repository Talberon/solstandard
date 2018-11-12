using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class EndTurnOption : MenuOption
    {

        public EndTurnOption(Color color) :
            base(new RenderText(AssetManager.MainMenuFont, "End Turn"), color)
        {
        }

        public override void Execute()
        {
            //Exit the menu and end the turn
            GameContext.CurrentGameState = GameContext.GameState.InGame;
            Queue<IEvent> eventsToQueue = new Queue<IEvent>();
            eventsToQueue.Enqueue(new EndTurnEvent());
            GlobalEventQueue.QueueEvents(eventsToQueue);
        }
        
        public override IRenderable Clone()
        {
            return new EndTurnOption(Color);
        }
    }
}