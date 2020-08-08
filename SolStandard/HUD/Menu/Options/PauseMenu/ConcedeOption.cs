using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ConcedeOption : MenuOption
    {
        public ConcedeOption(Color color) : base(new RenderText(AssetManager.MainMenuFont, "Surrender"), color)
        {
        }

        public override void Execute()
        {
            var eventsToQueue = new Queue<IEvent>();
            eventsToQueue.Enqueue(new ConcedeEvent());
            eventsToQueue.Enqueue(new EndTurnEvent());
            GlobalEventQueue.QueueEvents(eventsToQueue);
        }

        public override IRenderable Clone()
        {
            return new ConcedeOption(DefaultColor);
        }
    }
}