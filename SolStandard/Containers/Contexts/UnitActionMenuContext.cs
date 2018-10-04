﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Skills;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;

namespace SolStandard.Containers.Contexts
{
    public static class UnitActionMenuContext
    {
        private static readonly int[] InteractionRange = {0, 1};
        private static List<UnitSkill> _contextualActions;

        public static MenuOption[] GenerateActionMenuOptions(Color windowColour)
        {
            _contextualActions = FetchContextualActionsInRange();
            foreach (UnitSkill activeUnitSkill in GameContext.ActiveUnit.Skills)
            {
                _contextualActions.Add(activeUnitSkill);
            }

            MenuOption[] options = new MenuOption[_contextualActions.Count];
            for (int i = 0; i < _contextualActions.Count; i++)
            {
                options[i] = new SkillOption(windowColour, _contextualActions[i]);
            }

            return options;
        }

        public static string GetActionDescriptionAtIndex(int currentOptionIndex)
        {
            return _contextualActions[currentOptionIndex].Description;
        }

        private static List<UnitSkill> FetchContextualActionsInRange()
        {
            new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action))
                .GenerateRealTargetingGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates, InteractionRange);

            List<MapSlice> mapSlicesInRange = new List<MapSlice>();
            List<MapDistanceTile> distanceTiles = new List<MapDistanceTile>();
            foreach (MapElement mapElement in MapContainer.GameGrid[(int) Layer.Dynamic])
            {
                if (mapElement != null)
                {
                    distanceTiles.Add(mapElement as MapDistanceTile);
                    mapSlicesInRange.Add(MapContainer.GetMapSliceAtCoordinates(mapElement.MapCoordinates));
                }
            }

            List<UnitSkill> contextualSkills = new List<UnitSkill>();

            foreach (MapSlice slice in mapSlicesInRange)
            {
                IActionTile actionTile = slice.TerrainEntity as IActionTile;
                if (actionTile == null) continue;

                foreach (MapDistanceTile distanceTile in distanceTiles)
                {
                    foreach (int range in actionTile.Range)
                    {
                        //If the tile's range aligns with the current range of the unit, add the skill to the action list
                        if (distanceTile.MapCoordinates != actionTile.MapCoordinates) continue;
                        if (distanceTile.Distance == range)
                        {
                            contextualSkills.Add(actionTile.TileAction());
                        }
                    }
                }
            }

            MapContainer.ClearDynamicAndPreviewGrids();

            return contextualSkills;
        }
    }
}