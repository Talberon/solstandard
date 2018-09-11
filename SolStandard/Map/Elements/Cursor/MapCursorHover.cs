﻿using SolStandard.Containers.Contexts;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.Map.Elements.Cursor
{
    public static class MapCursorHover
    {
        public static void Hover(MapContext.TurnState turnState, MapSlice hoverTiles, MapUI mapUI)
        {
            GameUnit hoverMapUnit = UnitSelector.SelectUnit(hoverTiles.UnitEntity);


            if (turnState != MapContext.TurnState.SelectUnit)
            {
                if (hoverMapUnit != GameContext.ActiveUnit)
                {
                    mapUI.UpdateRightPortraitAndDetailWindows(hoverMapUnit);
                }
                else
                {
                    mapUI.UpdateRightPortraitAndDetailWindows(null);
                }
            }
            else
            {
                mapUI.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                mapUI.UpdateRightPortraitAndDetailWindows(null);
            }

            //Terrain (Entity) Window
            mapUI.GenerateTerrainWindow(hoverTiles.TerrainEntity);
        }
    }
}