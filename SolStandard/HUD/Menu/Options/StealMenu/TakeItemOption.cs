using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.HUD.Menu.Options.StealMenu
{
    public class TakeItemOption : MenuOption
    {
        private readonly GameUnit target;
        private readonly IItem itemToTake;
        private readonly bool freeAction;

        public TakeItemOption(GameUnit target, IItem itemToTake, Color color, bool freeAction) : base(
            GetOptionWindowForItem(itemToTake, color),
            color
        )
        {
            this.target = target;
            this.itemToTake = itemToTake;
            this.freeAction = freeAction;
        }

        private static IRenderable GetOptionWindowForItem(IItem item, Color color)
        {
            return new WindowContentGrid(
                new[,]
                {
                    {
                        item.Icon.Clone(),
                        new Window.Window(
                            new RenderText(AssetManager.WindowFont, $"Take: {item.Name}"),
                            color
                        ),
                        new Window.Window(item.UseAction().Description, color)
                    }
                }
            );
        }

        public override void Execute()
        {
            GlobalContext.WorldContext.ClearStealItemMenu();
            TakeItemAction.TakeItemFromInventory(GlobalContext.ActiveUnit, target, itemToTake);
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));

            if (freeAction)
            {
                GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
            }
            else
            {
                GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
            }
        }

        public override IRenderable Clone()
        {
            return new TakeItemOption(target, itemToTake, DefaultColor, freeAction);
        }
    }
}