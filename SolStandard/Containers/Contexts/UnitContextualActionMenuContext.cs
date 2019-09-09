using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;

namespace SolStandard.Containers.Contexts
{
    public static class UnitContextualActionMenuContext
    {
        public static List<ActionOption> ActiveUnitContextOptions(Color windowColor)
        {
            return FetchContextualActionsInRange()
                .Select(contextAction => new ActionOption(windowColor, contextAction))
                .ToList();
        }

        public static List<ActionOption> ActiveUnitSkillOptions(Color windowColor)
        {
            List<ActionOption> options = new List<ActionOption>();
            foreach (UnitAction skillAction in GameContext.ActiveUnit.Actions)
            {
                options.Add(new ActionOption(windowColor, skillAction));
            }

            return options;
        }

        public static IRenderable GetActionDescriptionForCurrentMenuOption(IMenu actionMenu)
        {
            if (actionMenu.CurrentOption is IOptionDescription descriptiveOption)
            {
                return descriptiveOption.Description;
            }

            return new RenderBlank();
        }

        public static MenuOption[,] GenerateInventoryMenuOptions(Color windowColor)
        {
            const int columns = 2;
            MenuOption[,] options = new MenuOption[GameContext.ActiveUnit.Inventory.Count, columns];

            for (int i = 0; i < GameContext.ActiveUnit.Inventory.Count; i++)
            {
                options[i, 0] = new ActionOption(windowColor, GameContext.ActiveUnit.Inventory[i].UseAction());
                options[i, 1] = new ActionOption(windowColor, GameContext.ActiveUnit.Inventory[i].DropAction());
            }

            return options;
        }

        private static IEnumerable<UnitAction> FetchContextualActionsInRange()
        {
            List<IActionTile> mapActionTiles = MapContainer.GetMapEntities()
                .Where(entity => entity is IActionTile)
                .Cast<IActionTile>()
                .ToList();

            List<UnitAction> contextActions = new List<UnitAction>();

            foreach (IActionTile actionTile in mapActionTiles)
            {
                if (
                    RangeComparison.TargetIsWithinRangeOfOrigin(
                        actionTile.MapCoordinates,
                        actionTile.InteractRange,
                        GameContext.ActiveUnit.UnitEntity.MapCoordinates
                    )
                )
                {
                    contextActions.AddRange(actionTile.TileActions());
                }
            }

            return contextActions;
        }
    }
}