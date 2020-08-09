using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility;

namespace SolStandard.Containers.Components.World.SubContext.ActionMenu
{
    public static class ContextMenuUtils
    {
        public static List<ActionOption> ActiveUnitContextOptions(Color windowColor)
        {
            return FetchContextualActionsInRange()
                .Select(contextAction => new ActionOption(windowColor, contextAction))
                .ToList();
        }

        public static List<ActionOption> ActiveUnitSkillOptions(Color windowColor)
        {
            var options = new List<ActionOption>();
            foreach (UnitAction skillAction in GlobalContext.ActiveUnit.Actions)
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

            return RenderBlank.Blank;
        }

        public static MenuOption[,] GenerateInventoryMenuOptions(Color windowColor)
        {
            const int columns = 2;
            List<IItem> activeUnitInventory = GlobalContext.ActiveUnit.Inventory;
            var options = new MenuOption[activeUnitInventory.Count, columns];

            for (int i = 0; i < activeUnitInventory.Count; i++)
            {
                IItem currentItem = activeUnitInventory[i];
                options[i, 0] = new ItemActionOption(currentItem, windowColor);
                options[i, 1] = new ActionOption(windowColor, currentItem.DropAction());
            }

            return options;
        }

        private static IEnumerable<UnitAction> FetchContextualActionsInRange()
        {
            List<IActionTile> mapActionTiles = MapContainer.GetMapEntities()
                .Where(entity => entity is IActionTile)
                .Cast<IActionTile>()
                .ToList();

            var contextActions = new List<UnitAction>();

            foreach (IActionTile actionTile in mapActionTiles.Where(actionTile =>
                RangeComparison.TargetIsWithinRangeOfOrigin(
                    actionTile.MapCoordinates,
                    actionTile.InteractRange,
                    GlobalContext.ActiveUnit.UnitEntity.MapCoordinates
                ))
            )
            {
                contextActions.AddRange(actionTile.TileActions());
            }

            UnitAction takeAction = TakeActionIfAllyInRange();
            if (takeAction != null) contextActions.Add(takeAction);

            return contextActions;
        }

        private static UnitAction TakeActionIfAllyInRange()
        {
            int[] meleeRange = {1};

            List<GameUnit> alliesInRange = GlobalContext.Units
                .Where(unit => unit.Team == GlobalContext.ActiveTeam && unit.IsAlive)
                .Where(ally => RangeComparison.TargetIsWithinRangeOfOrigin(
                    GlobalContext.ActiveUnit.UnitEntity.MapCoordinates,
                    meleeRange,
                    ally.UnitEntity.MapCoordinates
                ))
                .ToList();

            return alliesInRange.Any(ally => ally.Inventory.Count > 0) ? new TakeItemAction() : null;
        }
    }
}