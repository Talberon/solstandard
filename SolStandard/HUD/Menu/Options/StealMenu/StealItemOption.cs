using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions.Rogue;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.HUD.Menu.Options.StealMenu
{
    public class StealItemOption : MenuOption
    {
        private readonly GameUnit target;
        private readonly IItem itemToSteal;

        public StealItemOption(GameUnit target, IItem itemToSteal, Color color) : base(
            GetOptionWindowForItem(itemToSteal, color),
            color
        )
        {
            this.target = target;
            this.itemToSteal = itemToSteal;
        }

        private static IRenderable GetOptionWindowForItem(IItem item, Color color)
        {
            return new WindowContentGrid(
                new[,]
                {
                    {
                        item.Icon.Clone(),
                        new Window.Window(
                            new RenderText(AssetManager.WindowFont, $"Steal: {item.Name}"),
                            color
                        ),
                        new Window.Window(item.UseAction().Description, color)
                    }
                },
                1
            );
        }

        public override void Execute()
        {
            GameContext.GameMapContext.ClearStealItemMenu();
            Rob.StealItemFromInventory(GameContext.ActiveUnit, target, itemToSteal);
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
            GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
        }

        public override IRenderable Clone()
        {
            return new StealItemOption(target, itemToSteal, DefaultColor);
        }
    }
}