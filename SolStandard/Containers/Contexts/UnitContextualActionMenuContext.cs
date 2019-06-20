using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Containers.Contexts
{
    public static class UnitContextualActionMenuContext
    {
        private static readonly int[] InteractionRangeLimit = {0, 1, 2};
        private static List<UnitAction> _contextualActions;

        public static MenuOption[] GenerateActionMenuOptions(Color windowColour)
        {
            _contextualActions = FetchContextualActionsInRange();
            foreach (UnitAction activeUnitSkill in GameContext.ActiveUnit.Actions)
            {
                _contextualActions.Add(activeUnitSkill);
            }

            MenuOption[] options = new MenuOption[_contextualActions.Count];
            for (int i = 0; i < _contextualActions.Count; i++)
            {
                options[i] = new ActionOption(windowColour, _contextualActions[i]);
            }

            return options;
        }

        public static IRenderable GetActionDescriptionAtIndex(IMenu actionMenu)
        {
            ActionOption action = actionMenu.CurrentOption as ActionOption;
            if (action != null)
            {
                return action.Action.Description;
            }

            throw new SkillDescriptionNotFoundException();
        }

        public static MenuOption[,] GenerateInventoryMenuOptions(Color windowColour)
        {
            const int columns = 2;
            MenuOption[,] options = new MenuOption[GameContext.ActiveUnit.Inventory.Count + 1, columns];

            options[0, 0] = new ActionOption(windowColour, new DropGiveGoldAction());
            options[0, 1] = new ActionOption(windowColour, new Wait());

            for (int i = 0; i < GameContext.ActiveUnit.Inventory.Count; i++)
            {
                options[i + 1, 0] = new ActionOption(windowColour, GameContext.ActiveUnit.Inventory[i].UseAction());
                options[i + 1, 1] = new ActionOption(windowColour, GameContext.ActiveUnit.Inventory[i].DropAction());
            }

            return options;
        }

        private static List<UnitAction> FetchContextualActionsInRange()
        {
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