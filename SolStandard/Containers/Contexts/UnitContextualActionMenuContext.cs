using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Containers.Contexts
{
    public static class UnitContextualActionMenuContext
    {
        //FIXME Remove this limit and check from action tiles instead 
        private static readonly int[] InteractionRangeLimit = {0, 1, 2};

        public static List<ActionOption> ActiveUnitContextOptions(Color windowColor)
        {
            List<ActionOption> options = new List<ActionOption>();
            foreach (UnitAction contextAction in FetchContextualActionsInRange())
            {
                options.Add(new ActionOption(windowColor, contextAction));
            }

            return options;
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

        private static List<UnitAction> FetchContextualActionsInRange()
        {
            //TODO Rework this to not use a range limit

            new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action))
                .GenerateTargetingGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates, InteractionRangeLimit);

            List<MapSlice> mapSlicesInRange = new List<MapSlice>();
            List<MapDistanceTile> distanceTiles = new List<MapDistanceTile>();

            foreach (MapElement mapElement in MapContainer.GameGrid[(int) Layer.Dynamic])
            {
                if (mapElement == null) continue;

                distanceTiles.Add(mapElement as MapDistanceTile);
                mapSlicesInRange.Add(MapContainer.GetMapSliceAtCoordinates(mapElement.MapCoordinates));
            }

            List<UnitAction> contextualSkills = new List<UnitAction>();

            foreach (MapSlice slice in mapSlicesInRange)
            {
                IActionTile entityActionTile = slice.TerrainEntity as IActionTile;
                AddEntityAction(entityActionTile, distanceTiles, contextualSkills);

                IActionTile itemActionTile = slice.ItemEntity as IActionTile;
                AddEntityAction(itemActionTile, distanceTiles, contextualSkills);

                IActionTile unitActionTile = slice.UnitEntity;
                AddEntityAction(unitActionTile, distanceTiles, contextualSkills);
            }

            MapContainer.ClearDynamicAndPreviewGrids();

            return contextualSkills;
        }

        private static void AddEntityAction(IActionTile entityActionTile, List<MapDistanceTile> distanceTiles,
            List<UnitAction> contextualSkills)
        {
            if (entityActionTile == null) return;

            foreach (MapDistanceTile distanceTile in distanceTiles)
            {
                foreach (int range in entityActionTile.InteractRange)
                {
                    //If the tile's range aligns with the current range of the unit, add the action to the action list
                    if (distanceTile.MapCoordinates != entityActionTile.MapCoordinates) continue;
                    if (distanceTile.Distance == range)
                    {
                        contextualSkills.AddRange(entityActionTile.TileActions());
                    }
                }
            }
        }
    }
}