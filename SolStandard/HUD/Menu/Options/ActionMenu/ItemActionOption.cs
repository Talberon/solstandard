using Microsoft.Xna.Framework;
using SolStandard.Entity;

namespace SolStandard.HUD.Menu.Options.ActionMenu
{
    public class ItemActionOption : ActionOption
    {
        public ItemActionOption(IItem item, Color windowColor) : base(
            $"{item.Name} | {item.UseAction().Name}",
            windowColor,
            item.UseAction()
        )
        {
        }
    }
}